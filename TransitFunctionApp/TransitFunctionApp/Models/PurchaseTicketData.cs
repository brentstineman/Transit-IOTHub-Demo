using System;
using System.Collections.Generic;
using System.Text;

namespace TransitFunctionApp.Models
{
    public class PurchaseTicketData 
    {
        public string TransactionId { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public long Price { get; set; }

    }
}
