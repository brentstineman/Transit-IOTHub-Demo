using System;
using System.Collections.Generic;
using System.Text;

namespace TransitFunctionApp.Models
{
    public class LowStockRequest : ITicketRequest
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreateTime { get; set; }
        public string MethodName { get; set; }
        public bool IsLowStock { get; set; }

        public LowStockRequest()
        {
            this.MethodName = "ProcessLowStockResponse";
        }

    }
}
