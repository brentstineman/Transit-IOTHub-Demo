using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public class ValidateTicketResponse
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; set; } = Transportation.Demo.Shared.Models.MessageType.cmdValidateTicket;
        public string TransactionId { get; set; }
        public bool IsApproved { get; set; }
    }
}
