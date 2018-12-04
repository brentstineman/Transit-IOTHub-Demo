using System;

namespace Transportation.Demo.Shared.Models
{
    public interface IBaseDeviceEvent
    {
        DateTime CreateTime { get; set; }
        string DeviceId { get; set; }
        string DeviceType { get; set; }
        string MessageType { get; }
        string TransactionId { get; set; }
        string MethodName { get; set; }
        string Version { get; }
    }
}