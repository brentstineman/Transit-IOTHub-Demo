using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Shared.Models;
using TransportationDemoTests;

namespace TransitFunctionAppTest
{
    [TestFixture]
    class BaseDeviceTest
    {

        [Test]
        public void TestCreateBaseDevice()
        {
            BaseDeviceConfig deviceconfig = new BaseDeviceConfig()
            {
                DeviceId = "myFakeDevice",
                DeviceType = "base"

            };
            FakeDeviceClient myClient = new FakeDeviceClient();

            TimedSimulatedEvent simulatedEvent1 = new TimedSimulatedEvent(2000, 1000, this.EmptyTimeEvent);
            TimedSimulatedEvent simulatedEvent2 = new TimedSimulatedEvent(2000, 1000, this.EmptyTimeEvent);

            EventScheduler myScheduler = new EventScheduler();
            myScheduler.Add(simulatedEvent1);
            myScheduler.Add(simulatedEvent2);


            BaseDevice device = new BaseDevice(deviceconfig, myClient, myScheduler);
            device.StartAllEvents();
            // make sure the events have started
            Assert.IsTrue(simulatedEvent1.IsRunning);
            Assert.IsTrue(simulatedEvent2.IsRunning);
            // Testing that StopAllEvents actually stops the simulated events.
            device.StopAllEvents();
            Assert.IsFalse(simulatedEvent1.IsRunning);
            Assert.IsFalse(simulatedEvent2.IsRunning);
        }
        [Test]
        public void TestSetDeviceStatus()
        {
            BaseDeviceConfig deviceconfig = new BaseDeviceConfig()
            {
                DeviceId = "myFakeDevice",
                DeviceType = "base"

            };
            FakeDeviceClient myClient = new FakeDeviceClient();
            FakeEventScheduler myScheduler = new FakeEventScheduler();
            FakeTimedSimulatedEvent simulatedEvent = new FakeTimedSimulatedEvent();
            myScheduler.Add(simulatedEvent);
            BaseDevice device = new BaseDevice(deviceconfig, myClient, myScheduler);
            // make sure the default device status is disabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.disabled);
            device.SetDeviceStatus(DeviceStatus.enabled).Wait();
            device.StartAllEvents();
            // make sure the device status is enabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.enabled);
            Assert.AreEqual(myClient.twinProperties["status"], DeviceStatus.enabled);
            // make sure StopAllEvents is called after setting device status to disabled
            device.SetDeviceStatus(DeviceStatus.disabled).Wait();
            Assert.IsFalse(simulatedEvent.IsRunning);
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.disabled);
            Assert.AreEqual(myClient.twinProperties["status"], DeviceStatus.disabled);

        }
        private bool EmptyTimeEvent()
        {
            return true;
        }
    }
}
