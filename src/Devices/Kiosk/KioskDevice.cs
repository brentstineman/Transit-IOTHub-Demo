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

        public KioskDevice(string connectionString) : base(connectionString)
        {
            // set up any simulated events for this device
            this.EventList.Add(new SimulatedEvent(5000, 2500, this.SendPurchaseTicketMessageToCloudAsync));

            // register any callbacks
            this.deviceClient.RegisterDirectMethodAsync(ReceivePurchaseTicketResponse).Wait();
        }

        private async void SendPurchaseTicketMessageToCloudAsync()
        {
            var random = new Random();
            PurchaseTicketRequest purchaseTicketRequest = new PurchaseTicketRequest()
            {
                DeviceId = "rjTest",
                DeviceType = "Kiosk",
                MessageType = "Purchase",
                TransactionId = Guid.NewGuid().ToString(),
                CreateTime = System.DateTime.UtcNow,
                Price = random.Next(2, 100),
                MethodName = "ReceivePurchaseTicketResponse"
            };

            var messageString = JsonConvert.SerializeObject(purchaseTicketRequest);

            var eventJsonBytes = Encoding.UTF8.GetBytes(messageString);
            var message = new Microsoft.Azure.Devices.Client.Message(eventJsonBytes)
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json"
            };

            // Add a custom application property to the message.
            // An IoT hub can filter on these properties without access to the message body.
            var messageProperties = message.Properties;
            messageProperties.Add("deviceId", "rjTest");

            // Send the telemetry message
            //await _deviceClient.SendMessageAsync(messageString);

            //DeviceClient client = DeviceClient.CreateFromConnectionString(_connectionString, TransportType.Mqtt);
            //client.SetMethodHandlerAsync("ReceivePurchaseTicketResponse", ReceivePurchaseTicketResponse, null).Wait();
            await this.deviceClient.SendMessageAsync(messageString);
            //await client.SendEventAsync(message);
            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            Console.WriteLine();
        }

        // Handle the direct method call back from Azure
        private Task<MethodResponse> ReceivePurchaseTicketResponse(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            var json = JObject.Parse(data); //methodRequest.DataAsJson;
            string transactionId = json["TransactionId"].ToString();
            bool isApproved = (bool)json["IsApproved"];

            Console.WriteLine("Executed direct method: " + methodRequest.Name);
            Console.WriteLine($"Transaction Id: {transactionId}");
            Console.WriteLine($"IsApproved: {isApproved}");
            Console.WriteLine();

            // Acknowlege the direct method call with a 200 success message
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));

        }

    }
}
