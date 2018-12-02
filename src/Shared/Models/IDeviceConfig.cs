using System;

namespace Transportation.Demo.Shared.Models
{
    
    public interface IDeviceConfig
    {
        string DeviceId { get; set; }
        string DeviceType { get; set; }

    }
}