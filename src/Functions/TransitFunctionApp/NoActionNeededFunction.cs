using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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
        [FunctionName("LowStockFunction")]
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

                switch (msgType)
                {
                    case "LowStock":
                        // parse a low stock message
                        LowStockRequest lowStockMsg = JsonConvert.DeserializeObject<LowStockRequest>(messagePayload);

                        log.LogWarning($" !!!!! Low Stock Alert !!!! \n\t DeviceID - {lowStockMsg.DeviceId} " +
                            $"\n\t Time - {lowStockMsg.CreateTime}" +
                            $"\n\t Current Stock Level - {lowStockMsg.StockLevel}");

                        break;
                    case "GateOpened":
                        // parse a GateOpened
                        GateOpenedNotification gateOpenedMsg = JsonConvert.DeserializeObject<GateOpenedNotification>(messagePayload);

                        log.LogInformation($" Gate Opened on Device !!!! \n\t DeviceID - {gateOpenedMsg.DeviceId} ");

                        break;
                    default:
                        break;
                }

            }
        }
    }
}
