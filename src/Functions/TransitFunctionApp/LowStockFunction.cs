using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Functions
{
    public static class LowStockFunction
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

                LowStockRequest ticketRequestMessage = JsonConvert.DeserializeObject<LowStockRequest>(messagePayload);

                log.LogWarning($" !!!!! Low Stock Alert !!!! \n\t DeviceID - {ticketRequestMessage.DeviceId} " +
                    $"\n\t Time - {ticketRequestMessage.CreateTime}" +
                    $"\n\t Current Stock Level - {ticketRequestMessage.StockLevel}");
            }
        }
    }
}
