using Newtonsoft.Json;
using NUnit.Framework;
using TransitFunctionApp;
using Transportation.Demo.Devices.Kiosk;
using Transportation.Demo.Shared.Models;

namespace TransportationDemoTests
{
    [TestFixture]
    class KioskDeviceTest
    {
        FakeDeviceClient fakeDeviceClient = new FakeDeviceClient();
        FakeEventScheduler fakeScheduler = new FakeEventScheduler();

        [Test]
        public void TestDeviceEvent()
        {
            string myDeviceId = "myFakeDevice";

            KioskDevice device = new KioskDevice(myDeviceId, fakeDeviceClient, fakeScheduler);

            // Make sure the device has one event callback registered.
            Assert.AreEqual(fakeScheduler.EventList.Count, 1);

            //
            /// execute the first event delegate
            //
            // it should return false so it doesn't restart the event timer
            Assert.IsFalse(fakeScheduler.EventList[0]());
            // that delegate should have sent one message to the cloud
            Assert.AreEqual(fakeDeviceClient.sendMessageLog.Count, 1);

            // check the message sent to make sure its correct
            // create a sample request for comparison
            PurchaseTicketRequest expectedRequest = new PurchaseTicketRequest()
            {
                DeviceId = myDeviceId,
                DeviceType = "Kiosk",
                MessageType = "Purchase",
                TransactionId = "fakeId",
                CreateTime = System.DateTime.UtcNow,
                Price = 1,
                MethodName = "ReceivePurchaseTicketResponse"
            };
            // get request message into an object so we can compare it
            PurchaseTicketRequest actualRequest = JsonConvert.DeserializeObject<PurchaseTicketRequest>(fakeDeviceClient.sendMessageLog[0]);
            // compare properties to make sure they're valid. 
            Assert.AreEqual(actualRequest.DeviceId, expectedRequest.DeviceId);
            Assert.AreEqual(actualRequest.DeviceType, expectedRequest.DeviceType);
            Assert.AreEqual(actualRequest.MessageType, expectedRequest.MessageType);
            Assert.IsTrue(actualRequest.Price > 2);
            Assert.IsTrue(actualRequest.Price < 100);
            Assert.AreEqual(actualRequest.MethodName, expectedRequest.MethodName);

            //
            /// test the CloudToEvent PurchaseResponse call we expect back
            //
            // we should have one direct method registered right now
            Assert.AreEqual(fakeDeviceClient.directMethods.Count, 1);
            //

            // make sure the event has started, and that you have one callback completed
            //Assert.IsTrue(simulatedEvent.started);
            //Assert.AreEqual(simulatedEvent.callbackLog.Count, 1);

            //// The Kiosk Device return false on the callback to not reset the timer
            //Assert.IsFalse(simulatedEvent.callbackLog[0]);

            //Assert.AreEqual(fakeDeviceClient.sendMessageLog.Count, 1);

            //// message log has random values in TransactionId and Price, as well as utc now time in CreateTime. Can't test the full payload.
            //Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"DeviceId\":\"device1\""));
            //Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"DeviceType\":\"Kiosk\""));
            //Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"MessageType\":\"Purchase\""));
            //Assert.IsTrue(fakeDeviceClient.sendMessageLog[0].Contains("\"MethodName\":\"ReceivePurchaseTicketResponse\""));

            //// Testing that StopAllEvents actually stops the simulated event.
            //device.StopAllEvents();
            //Assert.IsFalse(simulatedEvent.started);
        }

    }
}
