using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public class KioskDeviceConfig : IDeviceConfig
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public long InitialStockCount { get; set; }
        public long LowStockThreshold { get; set; }
        public string Status { get; set; } = DeviceStatus.disabled.ToString();
    }
}
