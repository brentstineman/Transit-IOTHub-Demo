using System;

namespace Transportation.Demo.Shared.Models
{
    public interface IValidateTicketRequest : IBaseDeviceEvent
    {
        string TicketNbr { get; set; }
    }
}