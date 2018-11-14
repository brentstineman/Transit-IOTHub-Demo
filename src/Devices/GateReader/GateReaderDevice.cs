using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Base.Interfaces;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Devices.Base.Interfaces;

namespace Transportation.Demo.Devices.GateReader
{
    class GateReaderDevice : BaseDevice
    {
        public GateReaderDevice(string deviceId, string connectionString)
            : this(deviceId, connectionString, new TransportationDeviceClient(connectionString),
                  new TimedSimulatedEvent(5000, 2500))
        {
        }
        public GateReaderDevice(string deviceId, string connectionString, IDeviceClient client, ISimulatedEventWithSetter simulatedEvent) 
            : base(deviceId, connectionString, client)
        {
            simulatedEvent.SetCallback(this.DoSomething);
            // set up any simulated events for this device
            this.EventList.Add(simulatedEvent);
        }

        private bool DoSomething()
        {
            Console.WriteLine("Doing Something..");

            return true;
        }
    }
}
