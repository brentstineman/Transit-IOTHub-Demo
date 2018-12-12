using System;

namespace Transportation.Demo.Shared.Models
{
    public enum DeviceStatus { enabled, disabled };

    public interface IDeviceConfig
    {
        string DeviceId { get; set; }
        string DeviceType { get; set; }
        string Status { get; set; }

    }
}