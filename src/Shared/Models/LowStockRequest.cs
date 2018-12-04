using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public class LowStockRequest : IBaseDeviceEvent
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; set; } = Transportation.Demo.Shared.Models.MessageType.eventLowStock;
        public string TransactionId { get; set; }
        public DateTime CreateTime { get; set; }
        public string MethodName { get; set; }
        public long StockLevel { get; set; }

    }
}
