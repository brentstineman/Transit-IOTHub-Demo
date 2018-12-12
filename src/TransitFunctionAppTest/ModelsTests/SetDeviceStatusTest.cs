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
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void SetStatusTest()
        {
            var fakeClient = new FakeDeviceClient();
            var action = new SetDeviceStatusAction(fakeClient, new FakeLogger());

            action.SetDeviceStatus(DeviceStatus.enabled.ToString()).Wait();
            Assert.IsTrue(fakeClient.twinProperties.ContainsKey("status"));
            Assert.AreEqual(fakeClient.twinProperties["status"], DeviceStatus.enabled);

            Assert.ThrowsAsync<ArgumentException>(() => action.SetDeviceStatus("Invalid Status"));
        }
    }
}
