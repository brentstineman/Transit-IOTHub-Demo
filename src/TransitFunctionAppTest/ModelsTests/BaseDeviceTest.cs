using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Devices.Base.Interfaces;

namespace TransportationDemoTests.ModelsTests
{
    [TestFixture]
    class BaseDeviceTest
    {

        [Test]
        public void TestCreateBaseDevice()
        {
            string myDeviceId = "myFakeDevice";
            FakeDeviceClient myClient = new FakeDeviceClient();

            TimedSimulatedEvent simulatedEvent1 = new TimedSimulatedEvent(2000, 1000, this.EmptyTimeEvent);
            TimedSimulatedEvent simulatedEvent2 = new TimedSimulatedEvent(2000, 1000, this.EmptyTimeEvent);

            EventScheduler myScheduler = new EventScheduler();
            myScheduler.Add(simulatedEvent1);
            myScheduler.Add(simulatedEvent2);


            BaseDevice device = new BaseDevice(myDeviceId, myClient, myScheduler);
            device.StartAllEvents();
            // make sure the events have started
            Assert.IsTrue(simulatedEvent1.IsRunning);
            Assert.IsTrue(simulatedEvent2.IsRunning);
            // Testing that StopAllEvents actually stops the simulated events.
            device.StopAllEvents();
            Assert.IsFalse(simulatedEvent1.IsRunning);
            Assert.IsFalse(simulatedEvent2.IsRunning);
        }

        private bool EmptyTimeEvent()
        {
            return true;
        }
    }
}
