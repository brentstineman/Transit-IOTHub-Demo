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
        private BaseDeviceConfig deviceconfig;

        // test initialization
        public GateReaderTest()
        {
            deviceconfig = new BaseDeviceConfig()
            {
                DeviceId = "myFakeDevice",
                DeviceType = "GateReader"
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

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the purchase ticket simulated event..");

            GateReaderDevice device = new GateReaderDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            // execute a validate ticket event and check the result, it should always be false
            Assert.IsFalse(fakeScheduler.EventList[0].EventDelegate());
            // that delegate should have sent one message to the cloud
            Assert.AreEqual(fakeDeviceClient.sendMessageLog.Count, 1);

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
            Assert.AreEqual(actualRequest.DeviceId, expectedRequest.DeviceId);
            Assert.AreEqual(actualRequest.DeviceType, expectedRequest.DeviceType);
            Assert.AreEqual(actualRequest.MessageType, expectedRequest.MessageType);
            Assert.AreEqual(actualRequest.MethodName, expectedRequest.MethodName);

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
            Assert.AreEqual(methodresult.Status, 200); // got back an ok
        }

        [Test]
        public void TestValidationFailed()
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

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> validating ticket, shouldn't throw event");
            string requestString = JsonConvert.SerializeObject(approvePurchaseMethodkRequest);
            MethodRequest methodRequest = new MethodRequest("ReceivePurchaseTicketResponse", Encoding.UTF8.GetBytes(requestString));

            
            MethodResponse myresult = fakeDeviceClient.directMethods[0](methodRequest, null).Result;

            // no ticket was issued, no message sent to cloud
            Assert.AreEqual(fakeDeviceClient.sendMessageLog.Count, 0);
        }

    }
}
