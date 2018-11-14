using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Base.Interfaces;
using Transportation.Demo.Devices.Base.Interfaces;

namespace Transportation.Demo.Devices.Base
{
    public class BaseDevice
    {
        public List<ISimulatedEvent> EventList = new List<ISimulatedEvent>();

        protected IDeviceClient deviceClient;
        private string connectionString;

        protected string deviceId;
        protected string deviceType;

        public BaseDevice(string deviceId, string connectionString)
            : this(deviceId, connectionString, new TransportationDeviceClient(connectionString))
        {
        }

        public BaseDevice(string deviceId, string connectionString, IDeviceClient client)
        {
            this.deviceId = deviceId;
            this.connectionString = connectionString; // save this for later

            // Connect to the IoT hub using the MQTT protocol
            deviceClient = client;

            // ?? validate device ID on instantiation ?? 
        }

        public void StartAllEvents()
        {
            foreach(var myevent in EventList)
            {
                myevent.Start();
            }
        }

        public void StopAllEvents()
        {
            foreach (var myevent in EventList)
            {
                myevent.Stop();
            }

        }
    }
}
