using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;
using TransitFunctionApp;
using Transportation.Demo.Devices.Kiosk;
using Transportation.Demo.Shared.Models;
using Transportation.Demo.Devices.GateReader;

namespace TransportationDemoTests
{
    [TestFixture]
    class GateReaderTest
    {
        private GateReaderDeviceConfig deviceconfig;

        // test initialization
        public GateReaderTest()
        {
            deviceconfig = new GateReaderDeviceConfig()
            {
                DeviceId = "myFakeDevice",
                DeviceType = "GateReader",
                initialDirection = "In",
                PercentOfWrongWay = 0
            };

        }

        [Test]
        public void TestBaseGateReaderDevice()
        {
            FakeDeviceClient fakeDeviceClient = new FakeDeviceClient();
            FakeEventScheduler fakeScheduler = new FakeEventScheduler();

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the GateReader Device's base functionality..");

            GateReaderDevice device = new GateReaderDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            // should only have 1 scheduled event
            Assert.AreEqual(1, fakeScheduler.EventList.Count, "Incorrect number of scheduled events");

            // should only have 1 callback method
            Assert.AreEqual(1, fakeDeviceClient.directMethods.Count, "Incorrect number of callback methods");
        }

        [Test]
        public void TestGateReaderTicketValidation()
        {
            FakeDeviceClient fakeDeviceClient = new FakeDeviceClient();
            FakeEventScheduler fakeScheduler = new FakeEventScheduler();
            deviceconfig.PercentOfWrongWay = 0; // make sure we don't have any wrong way swipes

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the purchase ticket simulated event..");

            GateReaderDevice device = new GateReaderDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            // execute a validate ticket event and check the result, it should always be false
            Assert.IsFalse(fakeScheduler.EventList[0].EventDelegate());
            // that delegate should have sent one message to the cloud
            Assert.AreEqual(1, fakeDeviceClient.sendMessageLog.Count);

            // check the message sent to make sure its correct
            // create a sample request for comparison
            ValidateTicketRequest expectedRequest = new ValidateTicketRequest()
            {
                DeviceId = deviceconfig.DeviceId,
                DeviceType = deviceconfig.DeviceType,
                MessageType = "ValdiateTicket",
                TransactionId = "fakeId",
                CreateTime = System.DateTime.UtcNow,
                MethodName = "ReceiveTicketValidationResponse"
            };
            // get request message into an object so we can compare it
            ValidateTicketRequest actualRequest = JsonConvert.DeserializeObject<ValidateTicketRequest>(fakeDeviceClient.sendMessageLog[0]);
            // compare properties to make sure they're valid. 
            Assert.AreEqual(expectedRequest.DeviceId, actualRequest.DeviceId);
            Assert.AreEqual(expectedRequest.DeviceType, actualRequest.DeviceType);
            Assert.AreEqual(expectedRequest.MessageType, actualRequest.MessageType);
            Assert.AreEqual(expectedRequest.MethodName, actualRequest.MethodName);

            //
            /// test the CloudToEvent PurchaseResponse call we expect back
            //
            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the ticket successfully validated direct method..");

            // test the direct method itself
            ValidateTicketResponse approvePurchaseMethodkRequest = new ValidateTicketResponse()
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
            Assert.AreEqual(200, methodresult.Status); // got back an ok
        }

        [Test]
        public void TestInvalidTicketResponse()
        {
            FakeDeviceClient fakeDeviceClient = new FakeDeviceClient();
            FakeEventScheduler fakeScheduler = new FakeEventScheduler();

            TestContext.WriteLine(">> Testing the Device's Ticket Validation failed notification..");

            ValidateTicketResponse approvePurchaseMethodkRequest = new ValidateTicketResponse()
            {
                DeviceId = deviceconfig.DeviceId,
                DeviceType = deviceconfig.DeviceType,
                MessageType = "ValdiateTicket",
                IsApproved = false
            };

            // create our test device
            GateReaderDevice device = new GateReaderDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            // send a ticket NOT approved event to the callback method
            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> validating ticket, shouldn't throw event");
            string requestString = JsonConvert.SerializeObject(approvePurchaseMethodkRequest);
            MethodRequest methodRequest = new MethodRequest("ReceivePurchaseTicketResponse", Encoding.UTF8.GetBytes(requestString));
            
            MethodResponse myresult = fakeDeviceClient.directMethods[0](methodRequest, null).Result;

            // ticket validation failed, so no "gate opened" message sent
            Assert.AreEqual(0, fakeDeviceClient.sendMessageLog.Count);
        }

        [Test]
        public void TestWrongWaySwipe()
        {
            TestContext.WriteLine(">> Testing the Device's wrong way swipe..");

            FakeDeviceClient fakeDeviceClient = new FakeDeviceClient();
            FakeEventScheduler fakeScheduler = new FakeEventScheduler();
            deviceconfig.PercentOfWrongWay = 100; // gurantee a wrong way swipe

            // create our test device
            GateReaderDevice device = new GateReaderDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            // check the ticket, should return false so we can restart the timer
            Assert.IsTrue(fakeScheduler.EventList[0].EventDelegate());
            // that delegate should have sent NO messages to the cloud
            Assert.AreEqual(0, fakeDeviceClient.sendMessageLog.Count);
        }

    }
}
