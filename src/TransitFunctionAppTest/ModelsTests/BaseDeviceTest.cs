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
                DeviceType = DeviceType.TicketKiosk,
                Status = DeviceStatus.enabled.ToString()
            };

            // create a fake device twin
            // set up device propeties
            JObject myReportedProperties = new JObject();
            // set reported properties
            myReportedProperties.Add("status", deviceconfig.Status);

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
            device.InitializeAsync().Wait(); // initialize the device 
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
            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing device status..");

            fakeTwin = new Microsoft.Azure.Devices.Shared.Twin(deviceconfig.DeviceId);
            fakeTwin.Properties.Reported = new TwinCollection(new JObject(), null);

            FakeDeviceClient myClient = new FakeDeviceClient(fakeTwin);
            FakeEventScheduler myScheduler = new FakeEventScheduler();
            FakeTimedSimulatedEvent simulatedEvent = new FakeTimedSimulatedEvent();
            myScheduler.Add(simulatedEvent);
            deviceconfig.Status = DeviceStatus.disabled.ToString();

            BaseDevice device = new BaseDevice(deviceconfig, myClient, myScheduler);
            device.InitializeAsync().Wait(); // initialize the device 

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing default device status when there's no reported/desired device twin state..");

            // make sure the default device status is disabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.disabled);

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> enable the device and start its events..");

            // enable the device and start all its events
            device.SetDeviceStatusAsync(DeviceStatus.enabled).Wait();
            device.StartAllEvents();
            // make sure the device status is enabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.enabled);
            Assert.AreEqual(DeviceStatus.enabled, (DeviceStatus)Enum.Parse(typeof(DeviceStatus), fakeTwin.Properties.Reported["status"].ToString()));

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> disable the device and make sure events aren't running..");

            // disable the device and make sure events are not running
            device.SetDeviceStatusAsync(DeviceStatus.disabled).Wait();
            Assert.IsFalse(simulatedEvent.IsRunning);
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.disabled);
            Assert.AreEqual(DeviceStatus.disabled, (DeviceStatus)Enum.Parse(typeof(DeviceStatus), fakeTwin.Properties.Reported["status"].ToString()));
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

            // setup device to start as "disabled"
            deviceconfig.Status = DeviceStatus.disabled.ToString(); 
            BaseDevice device = new BaseDevice(deviceconfig, myClient, myScheduler);
            device.InitializeAsync().Wait();

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

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> update device state via device twin desired property update..");

            // build a set of desired state properties
            JObject desiredProperties = new JObject();
            desiredProperties.Add("status", DeviceStatus.enabled.ToString());
            // call the base device handler, simulating the triggering of a change event
            myClient.OnDesiredPropertyChangedAsync(new TwinCollection(desiredProperties, null), null).Wait();

            // make sure device is enabled
            Assert.AreEqual(device.GetDeviceStatus(), DeviceStatus.enabled);
        }

        private bool EmptyTimeEvent()
        {
            return true;
        }
    }
}
