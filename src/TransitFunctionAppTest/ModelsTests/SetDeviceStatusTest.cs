using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using TransitFunctionAppTest;
using Transportation.Demo.Functions;
using Transportation.Demo.Shared.Models;

namespace TransportationDemoTests.ModelsTests
{
    [TestFixture]
    class SetDeviceStatusTest
    {
        private GateReaderDeviceConfig deviceconfig;
        private Twin fakeTwin;

        [SetUp]
        public void SetUp()
        {
            deviceconfig = new GateReaderDeviceConfig()
            {
                DeviceId = "myFakeDevice",
                DeviceType = DeviceType.GateReader
            };

            // create a fake device twin
            // set up device propeties
            JObject myDesiredProperties = new JObject();
            // set reported properties
            myDesiredProperties.Add("status", DeviceStatus.enabled.ToString());

            fakeTwin = new Microsoft.Azure.Devices.Shared.Twin(deviceconfig.DeviceId);
            fakeTwin.Properties.Desired = new TwinCollection(myDesiredProperties, null);
        }

        [Test]
        public void SetStatusTest()
        {
            var fakeClient = new FakeJobClient(fakeTwin);
            var action = new SetDeviceStatusAction(fakeClient, new FakeLogger());

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the gets run and the twin was set up propertly..");
            action.SetDeviceStatus(deviceconfig.DeviceId, DeviceStatus.enabled.ToString()).Wait();
            Assert.IsTrue(fakeTwin.Properties.Desired.Contains("status"), "'status' value not found");
            Assert.AreEqual(DeviceStatus.enabled, (DeviceStatus)Enum.Parse(typeof(DeviceStatus), fakeTwin.Properties.Desired["status"].ToString()));

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the device status validation works..");
            Assert.ThrowsAsync<ArgumentException>(() => action.SetDeviceStatus(deviceconfig.DeviceId, "Invalid Status"));
        }
    }
}
