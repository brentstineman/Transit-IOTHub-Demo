﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public class GateOpenedRequest : IValidateTicketRequest
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreateTime { get; set; }
        public string TicketNbr { get; set; }
        public string MethodName { get; set; }
    }
}