using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TransitFunctionApp.Models;
using Microsoft.Azure.Devices;

namespace TransitFunctionApp
{
    public static class PurchaseTicket
    {

        private static ServiceClient s_serviceClient;
        // TO DO: Change input message type to EventData and recieve array or messages
        // instead of one message per function
        [FunctionName("PurchaseTicket")]
        public static void Run(
            [
            EventHubTrigger("purchaseticketeventhub", 
            Connection = "receiverConnectionString")
            ]
        PurchaseTicketRequest ticketRequestMessage, ILogger log)
        {
            string methodName = ticketRequestMessage.MethodName;
            string iotHubName = "TransportationOneWeekHub";
            string deviceId = ticketRequestMessage.DeviceId;
            string responseUrl = $"https://{iotHubName}.azure-devices.net/twins/{deviceId}/methods?api-version=2018-06-30";
            string transactionId = ticketRequestMessage.TransactionId;
            var response = new PurchaseTicketResponse()
            {
                MethodName = methodName,
                ResponseTimeoutInSeconds = 200,
                Payload = new PurchaseTicketPayload()
                {
                    TransactionId = transactionId,
                    IsApproved = true,
                    DeviceId = ticketRequestMessage.DeviceId,
                    DeviceType = ticketRequestMessage.DeviceType,
                    MessageType = ticketRequestMessage.MessageType,
                }

            };
            log.LogInformation($"Response Transcation ID: {response.Payload.TransactionId}");
            log.LogInformation($"Response Approval: {response.Payload.IsApproved}");
            log.LogInformation($"Response Method: {methodName}");

            s_serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));
            InvokeMethod(response).GetAwaiter().GetResult();

        }

        // Invoke the direct method on the device, passing the payload
        private static async Task InvokeMethod(PurchaseTicketResponse purchaseTicketResponse)
        {
            var methodInvocation = new CloudToDeviceMethod("SetTelemetryInterval") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            methodInvocation.SetPayloadJson("10");



            // Invoke the direct method asynchronously and get the response from the simulated device.
            var response = await s_serviceClient.InvokeDeviceMethodAsync("MyDotnetDevice", methodInvocation);

            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
        }

    }
}
