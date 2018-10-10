using System;
using System.Collections.Generic;
using System.Text;

namespace TransitFunctionApp.Models
{
    public class PurchaseTicketResponse
    {
        public bool Approved { get; set; }
        public string TransactionId { get; set; }

    }
}
