using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Microsoft.Azure.EventHubs;
using System.Text;
using Transportation.Demo.Shared.Models;

namespace TransitFunctionApp
{
    public static class PurchaseTicket
    {

        private static ServiceClient serviceClient;
        [FunctionName("PurchaseTicket")]
        public static void Run(
            [
            EventHubTrigger("%PurchaseTicketEventhubName%",
            Connection = "receiverConnectionString")
            ]
            EventData[] eventHubMessages, ILogger log)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));

            // process messages
            foreach (EventData message in eventHubMessages)
            {
                string messagePayload = Encoding.UTF8.GetString(message.Body.Array);

                PurchaseTicketAction action = new PurchaseTicketAction(new ServiceClientInvokeDeviceMethod(serviceClient), 5, messagePayload, log);
                action.Run();

            }
        }

        // Invoke the direct method on the device, passing the payload
        private static async Task InvokeMethod(string methodName, PurchaseTicketPayload purchaseTicketPayload)
        {
            TimeSpan responseTimeoutInSeconds = TimeSpan.FromSeconds(30);
            var methodInvocation = new CloudToDeviceMethod
                (methodName)
            {
                ResponseTimeout = responseTimeoutInSeconds
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
