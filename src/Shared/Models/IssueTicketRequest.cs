﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public class IssueTicketRequest : IBaseDeviceEvent
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; } = Transportation.Demo.Shared.Models.MessageType.eventTicketIssued;
        public string TransactionId { get; set; }
        public DateTime CreateTime { get; set; }
        public string MethodName { get; set; }

        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public long Price { get; set; }
        public string Version { get; } = "1.0.0";

        public IssueTicketRequest()
        {
            this.MethodName = "ProcessIssueTicketResponse";
        }
    }
}
