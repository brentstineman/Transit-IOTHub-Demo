using System;
using System.Collections.Generic;
using System.Text;

namespace TransitFunctionApp.Models
{
    public class PurchaseTicketResponse
    {
        public string MethodName { get; set; }
        public int ResponseTimeoutInSeconds { get; set; }
        public PurchaseTicketPayload Payload { get; set; }

    }
}
