using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Devices.Kiosk
{
    public class KioskDevice : BaseDevice
    {
        SimulatedEvent purchaseEvent;

        long TicketStockCount;
        bool LowStockNotificationSent = false; 

        public KioskDevice(string deviceId, string connectionString) : base(deviceId, connectionString)
        {
            this.deviceType = "Kiosk";
            this.TicketStockCount = 100; 


            // set up any simulated events for this device
            purchaseEvent = new SimulatedEvent(5000, 2500, this.SendPurchaseTicketMessageToCloud);
            this.EventList.Add(purchaseEvent);

            // register any callbacks
            this.deviceClient.RegisterDirectMethodAsync(ReceivePurchaseTicketResponse).Wait();
        }

        private bool SendPurchaseTicketMessageToCloud()
        {
            var random = new Random();
            PurchaseTicketRequest purchaseTicketRequest = new PurchaseTicketRequest()
            {
                DeviceId = this.deviceId,
                DeviceType = this.deviceType,
                MessageType = "Purchase",
                TransactionId = Guid.NewGuid().ToString(),
                CreateTime = System.DateTime.UtcNow,
                Price = random.Next(2, 100),
                MethodName = "ReceivePurchaseTicketResponse" // must match callback method
            };

            var messageString = JsonConvert.SerializeObject(purchaseTicketRequest);
            SendMessageToCloud(messageString);

            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            Console.WriteLine();

            return false; // don't restart timer
        }

        // Handle the direct method call back from Azure
        private Task<MethodResponse> ReceivePurchaseTicketResponse(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            PurchaseTicketPayload purchaseResponse = JsonConvert.DeserializeObject<PurchaseTicketPayload>(data);

            var json = JObject.Parse(data); //methodRequest.DataAsJson;

            Console.WriteLine("Executed direct method: " + methodRequest.Name);
            Console.WriteLine($"Transaction Id: {purchaseResponse.TransactionId}");
            Console.WriteLine($"IsApproved: {purchaseResponse.IsApproved}");
            Console.WriteLine();

            if (purchaseResponse.IsApproved)
            {
                SendTicketIssuedMessageToCloud(purchaseResponse); 
            }

            // Acknowlege the direct method call with a 200 success message
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";

            // restart timer
            purchaseEvent.Start();

            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        private bool SendTicketIssuedMessageToCloud(PurchaseTicketPayload requestpayload)
        {
            this.TicketStockCount--; 

            var random = new Random();
            IssueTicketRequest issueTicketRequest = new IssueTicketRequest()
            {
                DeviceId = this.deviceId,
                DeviceType = this.deviceType,
                MessageType = "TicketIssued",
                TransactionId = requestpayload.TransactionId,
                CreateTime = System.DateTime.UtcNow
            };

            var messageString = JsonConvert.SerializeObject(issueTicketRequest);
            SendMessageToCloud(messageString);

            this.deviceClient.SendMessageAsync(messageString).Wait();

            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            Console.WriteLine();

            // if we've fallen below the threshold, send notification
            if (this.TicketStockCount < 95 && !this.LowStockNotificationSent)
            {
                SendTLowStockMessageToCloud(); 
            }

            return false; // don't restart timer
        }

        private bool SendTLowStockMessageToCloud()
        {
            LowStockRequest lowStockNotification = new LowStockRequest()
            {
                DeviceId = this.deviceId,
                DeviceType = this.deviceType,
                MessageType = "LowStock",
                StockLevel = this.TicketStockCount,
                CreateTime = System.DateTime.UtcNow
            };

            var messageString = JsonConvert.SerializeObject(lowStockNotification);
            SendMessageToCloud(messageString);

            this.LowStockNotificationSent = true;

            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            Console.WriteLine();

            return false; // don't restart timer
        }
    }
}
