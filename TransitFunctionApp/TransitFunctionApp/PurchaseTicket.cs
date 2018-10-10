using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;

namespace TransitFunctionApp
{
    public static class PurchaseTicket
    {

        [FunctionName("PurchaseTicket")]
        public static void Run([EventHubTrigger("purchaseticketeventhub", Connection = "receiverConnectionString")]string myEventHubMessage, ILogger log)
        {
            //var connectionString = Environment.GetEnvironmentVariable("TicketConnectionString")
            log.LogInformation($"Ticket purchased: {myEventHubMessage}");
        }
    }
}
