using System;
using System.Collections.Generic;
using System.Text;

namespace TransitFunctionApp.Models
{
    public class PurchaseTicketPayload
    {
        public bool IsApproved { get; set; }
        public string TransactionId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; set; }

    }
}
