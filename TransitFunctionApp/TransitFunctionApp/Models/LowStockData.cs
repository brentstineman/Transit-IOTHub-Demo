using System;
using System.Collections.Generic;
using System.Text;

namespace TransitFunctionApp.Models
{
    public class LowStockData : IMessageData
    {
        public string TransactionId { get; set; }
    }
}
