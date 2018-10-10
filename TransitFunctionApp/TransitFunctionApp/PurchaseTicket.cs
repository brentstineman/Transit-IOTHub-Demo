using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TransitFunctionApp.Models;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Microsoft.Azure.EventHubs;
using System.Text;

namespace TransitFunctionApp
{
    public static class PurchaseTicket
    {

        private static ServiceClient serviceClient;
        [FunctionName("PurchaseTicket")]
        public static void Run(
            [
            EventHubTrigger("purchaseticketeventhub", 
            Connection = "receiverConnectionString")
            ]
        EventData[] eventHubMessages, ILogger log)
        {
            // process messages
            foreach (EventData message in eventHubMessages)
            {
                string messagePayload = Encoding.UTF8.GetString(message.Body.Array);

                // process each message
                PurchaseTicketRequest ticketRequestMessage = JsonConvert.DeserializeObject<PurchaseTicketRequest>(messagePayload);

                try
                {
                    string methodName = ticketRequestMessage.MethodName;
                    //string iotHubName = Environment.GetEnvironmentVariable("IotHubName");
                    string deviceId = ticketRequestMessage.DeviceId;
                    //string responseUrl = $"https://{iotHubName}.azure-devices.net/twins/{deviceId}/methods?api-version=2018-06-30";
                    string transactionId = ticketRequestMessage.TransactionId;
                    var payload = new PurchaseTicketPayload()
                    {
                        TransactionId = transactionId,
                        IsApproved = true,
                        DeviceId = ticketRequestMessage.DeviceId,
                        DeviceType = ticketRequestMessage.DeviceType,
                        MessageType = ticketRequestMessage.MessageType,
                    };

                    log.LogInformation($"Response Method: {methodName}");

                    serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));
                    InvokeMethod(methodName, payload).GetAwaiter().GetResult();
                }
                catch(Exception ex)
                {
                    log.LogError(ex.Message);
                }
                
            }
        }

        // Invoke the direct method on the device, passing the payload
        private static async Task InvokeMethod(string methodName, PurchaseTicketPayload purchaseTicketPayload)
        {
            TimeSpan responseTimeoutInSeconds = TimeSpan.FromSeconds(30);
            var methodInvocation = new CloudToDeviceMethod
                (methodName) {
                ResponseTimeout =  responseTimeoutInSeconds
            };
            var payload = JsonConvert.SerializeObject(purchaseTicketPayload);
            methodInvocation.SetPayloadJson(payload);

            try
            {
                // Invoke the direct method asynchronously and get the response from the simulated device.
                var response = await serviceClient.InvokeDeviceMethodAsync(purchaseTicketPayload.DeviceId, methodInvocation);

                Console.WriteLine("Response status: {0}, payload:", response.Status);
                Console.WriteLine(response.GetPayloadAsJson());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }

    }
}
