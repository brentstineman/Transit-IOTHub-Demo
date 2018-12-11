using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public class BaseDeviceConfig : IDeviceConfig
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Status { get; set; }
    }
}
