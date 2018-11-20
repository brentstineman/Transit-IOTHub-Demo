using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Base.Interfaces;
using Transportation.Demo.Devices.Base.Interfaces;

namespace Transportation.Demo.Devices.Base
{
    public class BaseDevice
    {
        protected IEventScheduler eventScheduler;

        protected IDeviceClient deviceClient;

        protected string deviceId;
        protected string deviceType;

        public BaseDevice(string deviceId, IDeviceClient client, IEventScheduler eventScheduler)
        {
            this.deviceId = deviceId;
            this.eventScheduler = eventScheduler;

            // Connect to the IoT hub using the MQTT protocol
            this.deviceClient = client;

            // ?? validate device ID on instantiation ?? 
        }

        public void StartAllEvents()
        {
            eventScheduler.StartAll(); 
        }

        public void StopAllEvents()
        {
            eventScheduler.StopAll();
        }
    }
}
