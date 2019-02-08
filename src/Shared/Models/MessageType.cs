using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public static class MessageType
    {
        public const string cmdValidateTicket = "cmdValidateTicket";
        public const string cmdPurchaseTicket = "cmdPurchaseTicket";
        public const string cmdGateDirectionChange = "cmdGateDirectionChange";
        public const string eventTicketIssued = "eventTicketIssued";
        public const string eventLowStock = "eventLowStock";
        public const string eventGateOpened = "eventGateOpened";
        public const string eventGateDirectionChange = "eventGateDirectionChange";
    }

}
