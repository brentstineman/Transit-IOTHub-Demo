using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Shared.Models;
using TransportationDemoTests;

namespace TransitFunctionAppTest
{
    [TestFixture]
    class BaseDeviceTest
    {
        private BaseDeviceConfig deviceconfig;
        private Twin fakeTwin;

        public BaseDeviceTest()
        {
            deviceconfig = new BaseDeviceConfig()
            {
                DeviceId = "myFakeDevice",
                DeviceType = DeviceType.TicketKiosk
            };

            // create a fake device twin
            // set up device propeties
            JObject myReportedProperties = new JObject();
            // set reported properties
            myReportedProperties.Add("status", DeviceStatus.disabled.ToString());

            fakeTwin = new Microsoft.Azure.Devices.Shared.Twin(deviceconfig.DeviceId);
            fakeTwin.Properties.Reported = new TwinCollection(myReportedProperties, null);
        }

        [Test]
        public void TestCreateBaseDevice()
        {
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
            fakeTwin = new Microsoft.Azure.Devices.Shared.Twin(deviceconfig.DeviceId);
            fakeTwin.Properties.Reported = new TwinCollection(new JObject(), null);

            FakeDeviceClient myClient = new FakeDeviceClient(fakeTwin);
            FakeEventScheduler myScheduler = new FakeEventScheduler();
            FakeTimedSimulatedEvent simulatedEvent = new FakeTimedSimulatedEvent();
            myScheduler.Add(simulatedEvent);
            BaseDevice device = new BaseDevice(deviceconfig, myClient, myScheduler);

            // make sure the default device status is disabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.disabled);
            
            // enable the device and start all its events
            device.SetDeviceStatusAsync(DeviceStatus.enabled).Wait();
            device.StartAllEvents();
            // make sure the device status is enabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.enabled);
            Assert.AreEqual(DeviceStatus.enabled, (DeviceStatus)Enum.Parse(typeof(DeviceStatus), fakeTwin.Properties.Reported["status"].ToString()));
            
            // disable the device and make sure events are not running
            device.SetDeviceStatusAsync(DeviceStatus.disabled).Wait();
            Assert.IsFalse(simulatedEvent.IsRunning);
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.disabled);
            Assert.AreEqual(DeviceStatus.disabled, (DeviceStatus)Enum.Parse(typeof(DeviceStatus), fakeTwin.Properties.Reported["status"].ToString()));

            // make sure the device responds to desired state changes
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.enabled);
        }

        [Test]
        public void TestDesiredPropertyHandler()
        {
            fakeTwin = new Microsoft.Azure.Devices.Shared.Twin(deviceconfig.DeviceId);
            fakeTwin.Properties.Reported = new TwinCollection(new JObject(), null);
            fakeTwin.Properties.Desired = new TwinCollection(new JObject(), null);

            FakeDeviceClient myClient = new FakeDeviceClient(fakeTwin);
            FakeEventScheduler myScheduler = new FakeEventScheduler();
            FakeTimedSimulatedEvent simulatedEvent = new FakeTimedSimulatedEvent();
            myScheduler.Add(simulatedEvent);
            BaseDevice device = new BaseDevice(deviceconfig, myClient, myScheduler);


            // enable the device and start all its events
            device.SetDeviceStatusAsync(DeviceStatus.enabled).Wait();
            device.StartAllEvents();
            // make sure the device status is enabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.enabled);
            Assert.AreEqual(DeviceStatus.enabled, (DeviceStatus)Enum.Parse(typeof(DeviceStatus), fakeTwin.Properties.Reported["status"].ToString()));

            // disable the device and make sure events are not running
            device.SetDeviceStatusAsync(DeviceStatus.disabled).Wait();
            Assert.IsFalse(simulatedEvent.IsRunning);
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.disabled);
            Assert.AreEqual(DeviceStatus.disabled, (DeviceStatus)Enum.Parse(typeof(DeviceStatus), fakeTwin.Properties.Reported["status"].ToString()));

        }

        private bool EmptyTimeEvent()
        {
            return true;
        }
    }
}
