using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Shared.Models
{
    public class GateReaderDeviceConfig : IDeviceConfig
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string initialDirection { get; set; }
        public int PercentOfWrongWay { get; set; }
    }
}
