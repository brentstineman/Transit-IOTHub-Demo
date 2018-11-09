using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Devices.Base;

namespace Transportation.Demo.Devices.GateReader
{
    class GateReaderDevice : BaseDevice
    {
        public GateReaderDevice(string connectionString) : base(connectionString)
        {
            // set up any simulated events for this device
            this.EventList.Add(new SimulatedEvent(5000, 2500, this.DoSomething));
        }

        private void DoSomething()
        {
            Console.WriteLine("Doing Something..");
        }
    }
}
