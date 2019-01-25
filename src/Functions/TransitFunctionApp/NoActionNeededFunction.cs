using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Functions
{
    /// <summary>
    /// This function will handle the various messages such as 'LowStock' where no action is needed.
    /// The purpose of this function is to demonstrate how a single function could handle multiple 
    /// message types. 
    /// </summary>
    public static class NoActionNeededFunction
    {
        [FunctionName("NoActionNeededFunction")]
        public static void Run(
            [
            EventHubTrigger("%LowStockEventHub%", 
            Connection = "receiverConnectionString")
            ]
        EventData[] eventHubMessages, ILogger log)
        {
            foreach (var message in eventHubMessages)
            {
                 string messagePayload = Encoding.UTF8.GetString(message.Body.Array);

                // note, this approaches parses the JSON twice, which isn't very efficient but does help 
                // demonstrate what we're trying to accomplish
                var obj = JObject.Parse(messagePayload); // get a dynamic object based on the msg payload
                string msgType = (string)obj["MessageType"];

                if (message.Properties.ContainsKey("opType") && (message.Properties["opType"].ToString() == "updateTwin"))
                {
                    log.LogInformation($" Device Twin update event.");

                    string deviceId = message.Properties["deviceId"].ToString();
                    
                    // are there desired properties present?
                    if (obj["properties"]["reported"] != null)
                    {
                        if (obj["properties"]["reported"]["GateDirection"] != null)
                        {
                            string reportedGateDirection = obj["properties"]["reported"]["GateDirection"].ToString();
                            log.LogInformation($" Gate Direction on Device {deviceId} reported change to {reportedGateDirection}.");
                        }
                        if (obj["properties"]["reported"]["status"] != null)
                        {
                            log.LogInformation($" Desired Status of Device {deviceId} reported as {obj["properties"]["reported"]["status"].ToString()}.");
                        }
                    }
                    else if (obj["properties"]["desired"] != null)
                    {
                        if (obj["properties"]["desired"]["status"] != null)
                        {
                            log.LogInformation($" Desired Status of Device {deviceId} set to {obj["properties"]["desired"]["status"].ToString()}.");
                        }
                    }

                }
                else
                {
                    switch (msgType)
                    {
                        case MessageType.eventLowStock:
                            // parse a low stock message
                            LowStockRequest lowStockMsg = JsonConvert.DeserializeObject<LowStockRequest>(messagePayload);

                            log.LogWarning($" !!!!! Low Stock Alert !!!! \n\t DeviceID - {lowStockMsg.DeviceId} " +
                                $"\n\t Time - {lowStockMsg.CreateTime}" +
                                $"\n\t Current Stock Level - {lowStockMsg.StockLevel}");

                            break;
                        case MessageType.eventTicketIssued:
                            // parse a ticket issued event
                            IssueTicketRequest ticketIssued = JsonConvert.DeserializeObject<IssueTicketRequest>(messagePayload);

                            log.LogInformation($" Ticket Issued by device {ticketIssued.DeviceId} ");

                            break;
                        case MessageType.eventGateOpened:
                            // parse a GateOpened
                            GateOpenedNotification gateOpenedMsg = JsonConvert.DeserializeObject<GateOpenedNotification>(messagePayload);

                            log.LogInformation($" Gate Opened on Device {gateOpenedMsg.DeviceId} ");

                            break;
                        default:
                            log.LogError($" Unknown message recieved !!!! \n\t {messagePayload} ");

                            break;
                    }
                }
            }
        }
    }
}
