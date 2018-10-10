using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TransitFunctionApp.Models;

namespace TransitFunctionApp
{
    public static class PurchaseTicket
    {

        [FunctionName("PurchaseTicket")]
        public static void Run(
            [
            EventHubTrigger("purchaseticketeventhub", 
            Connection = "receiverConnectionString")
            ]
        TicketRequestMessage ticketRequestMessage, ILogger log)
        {
            string transactionId = ticketRequestMessage.TransactionId;
            var response = new PurchaseTicketResponse()
            {
                TransactionId = transactionId,
                Approved = true
            };
            log.LogInformation($"Response Transcation ID: {response.TransactionId}");
            log.LogInformation($"Response Approval: {response.Approved}");

            //try
            //{
            //    var transactionId = ticketRequestMessage.TransactionId;
            //    response = new PurchaseTicketResponse()
            //    {
            //        TransactionId = transactionId,
            //        Approved = true
            //    };
            //    log.LogInformation($"Response: {response}");
            //}
            //catch (Exception ex)
            //{
            //    log.LogInformation($"Error: {ex.Message}");

            //    throw;
            //}
        }

        //public static void Run([EventHubTrigger("purchaseticketeventhub", Connection = "receiverConnectionString")]string myEventHubMessage, ILogger log)
        //{

        //    log.LogInformation($"Ticket purchased: {myEventHubMessage}");
        //}
    }
}
