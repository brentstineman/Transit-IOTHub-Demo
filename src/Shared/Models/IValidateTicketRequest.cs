using System;

namespace Transportation.Demo.Shared.Models
{
    public interface IValidateTicketRequest
    {
        DateTime CreateTime { get; set; }
        string DeviceId { get; set; }
        string DeviceType { get; set; }
        string MessageType { get; set; }
        string TransactionId { get; set; }
        string TicketNbr { get; set; }
        string MethodName { get; set; }

    }
}