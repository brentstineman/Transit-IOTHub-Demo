﻿using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Base.Interfaces;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Devices.Base.Interfaces;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Devices.GateReader
{
    class GateReaderDevice : BaseDevice
    {
        public GateReaderDevice(IDeviceConfig deviceConfig, IDeviceClient client, IEventScheduler eventScheduler) 
            : base(deviceConfig, client, eventScheduler)
        {

            TimedSimulatedEvent simulatedEvent = new TimedSimulatedEvent(5000, 2500, this.DoSomething);

            // set up any simulated events for this device
            this.eventScheduler.Add(simulatedEvent);
        }

        private bool DoSomething()
        {
            Console.WriteLine("Doing Something..");

            return true;
        }
    }
}