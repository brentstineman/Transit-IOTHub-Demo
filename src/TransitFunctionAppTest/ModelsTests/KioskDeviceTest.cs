using NUnit.Framework;
using TransitFunctionApp;
using Transportation.Demo.Devices.Kiosk;

namespace TransportationDemoTests
{
    [TestFixture]
    class KioskDeviceTest
    {
        TestSimulatedEvent simulatedEvent = new TestSimulatedEvent();
        FakeDeviceClient fakeDeviceClient = new FakeDeviceClient();

        [Test]
        public void TestDeviceEvent()
        {
            KioskDevice device = new KioskDevice("device1", "connection1", fakeDeviceClient, simulatedEvent);

            // Make sure the device has one event callback registered.
            Assert.AreEqual(device.EventList.Count, 1);

            // Start the event.
            device.StartAllEvents();

            // make sure the event has started, and that you have one callback completed
            Assert.IsTrue(simulatedEvent.started);
            Assert.AreEqual(simulatedEvent.callbackLog.Count, 1);

            // The Kiosk Device return false on the callback to not reset the timer
            Assert.IsFalse(simulatedEvent.callbackLog[0]);

            Assert.AreEqual(fakeDeviceClient.sendMessageLog.Count, 1);

            // message log has random values in TransactionId and Price, as well as utc now time in CreateTime. Can't test the full payload.
            Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"DeviceId\":\"device1\""));
            Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"DeviceType\":\"Kiosk\""));
            Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"MessageType\":\"Purchase\""));
            Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"MethodName\":\"ReceivePurchaseTicketResponse\""));

            // Testing that StopAllEvents actually stops the simulated event.
            device.StopAllEvents();
            Assert.IsFalse(simulatedEvent.started);
        }

    }
}
