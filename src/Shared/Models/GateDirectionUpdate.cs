using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public enum GateDirection { In, Out };

    public class GateDirectionUpdate
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; set; } = Transportation.Demo.Shared.Models.MessageType.cmdGateDirectionChange;
        public string TransactionId { get; set; }
        public GateDirection Direction { get; set; }
    }
}
