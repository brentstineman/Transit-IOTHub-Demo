using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;
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
            TestContext.WriteLine(">>Testing the Kiosk Device..");

            string myDeviceId = "myFakeDevice";

            KioskDevice device = new KioskDevice(myDeviceId, fakeDeviceClient, fakeScheduler);

            // Make sure the device has one event callback registered.
            Assert.AreEqual(fakeScheduler.EventList.Count, 1);

            TestContext.WriteLine("\n>> Testing the purchase ticket simulated event..");

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
            // skipping the TransactionID and CreationTime
            Assert.IsTrue(actualRequest.Price > 2);
            Assert.IsTrue(actualRequest.Price < 100);
            Assert.AreEqual(actualRequest.MethodName, expectedRequest.MethodName);

            //
            /// test the CloudToEvent PurchaseResponse call we expect back
            //
            TestContext.WriteLine("\n>> Testing the ticket approval direct method..");

            // we should have one direct method registered right now
            Assert.AreEqual(fakeDeviceClient.directMethods.Count, 1);
            // test the direct method itself
            PurchaseTicketPayload approvePurchaseMethodkRequest = new PurchaseTicketPayload()
            {
                IsApproved = true,
                DeviceId = expectedRequest.DeviceId,
                DeviceType = expectedRequest.DeviceType,
                MessageType = expectedRequest.MessageType,
            };
            string requestString = JsonConvert.SerializeObject(approvePurchaseMethodkRequest);
            // execute the method
            MethodRequest methodRequest = new MethodRequest(expectedRequest.MethodName, Encoding.UTF8.GetBytes(requestString));
            MethodResponse methodresult = fakeDeviceClient.directMethods[0](methodRequest, null).Result;
            // check results
            Assert.AreEqual(methodresult.Status, 200); // got back an ok
        }

    }
}
