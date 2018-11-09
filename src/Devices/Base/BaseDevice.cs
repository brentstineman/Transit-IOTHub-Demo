using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Devices.Base
{
    public class BaseDevice
    {
        public List<SimulatedEvent> EventList = new List<SimulatedEvent>();

        protected TransportationDeviceClient deviceClient;
        private string connectionString;

        public BaseDevice(string connectionString)
        {
            this.connectionString = connectionString; // save this for later

            // Connect to the IoT hub using the MQTT protocol
            deviceClient = new TransportationDeviceClient(connectionString);
        }

        public void StartAllEvents()
        {
            foreach(SimulatedEvent myevent in EventList)
            {
                myevent.Start();
            }
        }

        public void StopAllEvents()
        {
            foreach (SimulatedEvent myevent in EventList)
            {
                myevent.Stop();
            }

        }
    }
}
