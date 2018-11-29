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
    public static class ValidateTicket
    {

        private static ServiceClient serviceClient;
        [FunctionName("ValidateTicket")]
        public static void Run(
            [
            EventHubTrigger("%ValidateTicketEventhubName%",
            Connection = "receiverConnectionString")
            ]
            EventData[] eventHubMessages, ILogger log)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));

            // process messages
            foreach (EventData message in eventHubMessages)
            {
                string messagePayload = Encoding.UTF8.GetString(message.Body.Array);

                ValidateTicketAction action = new ValidateTicketAction(new ServiceClientInvokeDeviceMethod(serviceClient), messagePayload, log);
                action.Run();

            }
        }
    }
}
