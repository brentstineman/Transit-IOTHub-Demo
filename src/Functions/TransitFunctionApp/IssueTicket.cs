using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using Transportation.Demo.Shared.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Microsoft.Azure.EventHubs;
using System.Text;

namespace Transportation.Demo.Functions
{
    public static class IssueTicket
    {

        [FunctionName("IssueTicket")]
        public static void Run(
            [
            EventHubTrigger("%IssueTicketEventHubName%",
            Connection = "receiverConnectionString")
            ]
            EventData[] eventHubMessages, ILogger log)
        {
            // process messages
            foreach (EventData message in eventHubMessages)
            {
                string messagePayload = Encoding.UTF8.GetString(message.Body.Array);

                // process each message
                IssueTicketRequest ticketRequestMessage = JsonConvert.DeserializeObject<IssueTicketRequest>(messagePayload);

                try
                {
                    string methodName = ticketRequestMessage.MethodName;
                    string deviceId = ticketRequestMessage.DeviceId;
                    string transactionId = ticketRequestMessage.TransactionId;
                    var payload = new IssueTicketRequest()
                    {
                        TransactionId = transactionId,
                        DeviceId = ticketRequestMessage.DeviceId,
                        DeviceType = ticketRequestMessage.DeviceType
                    };

                    log.LogInformation($"Response Method: {methodName}");

                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message);
                }

            }
            
        }
    }
}
