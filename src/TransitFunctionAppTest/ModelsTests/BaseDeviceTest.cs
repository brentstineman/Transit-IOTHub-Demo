using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Transportation.Demo.Devices.Base;

namespace TransportationDemoTests.ModelsTests
{
    [TestFixture]
    class BaseDeviceTest
    {
        TestSimulatedEvent simulatedEvent1 = new TestSimulatedEvent();
        TestSimulatedEvent simulatedEvent2 = new TestSimulatedEvent();

        [Test]
        public void TestCreateBaseDevice()
        {
            BaseDevice device = new BaseDevice("device1", "connection1", null);
            device.EventList.Add(simulatedEvent1);
            device.EventList.Add(simulatedEvent2);
            device.StartAllEvents();
            // make sure the events have started
            Assert.IsTrue(simulatedEvent1.started);
            Assert.IsTrue(simulatedEvent2.started);
            // Testing that StopAllEvents actually stops the simulated events.
            device.StopAllEvents();
            Assert.IsFalse(simulatedEvent1.started);
            Assert.IsFalse(simulatedEvent2.started);
        }
    }
}
