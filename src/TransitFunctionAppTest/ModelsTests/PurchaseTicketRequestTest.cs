using Newtonsoft.Json;
using NUnit.Framework;
using TransitFunctionApp;
using Transportation.Demo.Shared.Models;

namespace TransitFunctionAppTest
{
    [TestFixture]
    class PurchaseTicketRequestTest
    {
        FakeLogger logger;
        [SetUp]
        public void SetUp()
        {
            logger = new FakeLogger();
        }

        [Test]
        public void PurchaseTicketTest()
        {
            // sample payload to test 
            PurchaseTicketRequest sampleRequest = new PurchaseTicketRequest()
            {
                DeviceId = "rjTest",
                DeviceType = DeviceType.TicketKiosk,
                TransactionId = "c3d8b13c - 3e8f - 4e57 - a7d5 - ae8adaa8c2b2",
                CreateTime = System.DateTime.UtcNow,
                Price = 54,
                MethodName = "ReceivePurchaseTicketResponse"
            };
            string sampleRequestString = JsonConvert.SerializeObject(sampleRequest);

            // expected response 
            PurchaseTicketPayload expectedResponse = new PurchaseTicketPayload()
            {
                DeviceId = sampleRequest.DeviceId,
                TransactionId = sampleRequest.TransactionId,
                DeviceType = sampleRequest.DeviceType,
                MessageType = MessageType.cmdPurchaseTicket,
                IsApproved = true
            };
            string expectedResponseString = JsonConvert.SerializeObject(expectedResponse);


            FakeInvokeDeviceMethod serviceClient = new FakeInvokeDeviceMethod();
            PurchaseTicketAction action = new PurchaseTicketAction(serviceClient, 0, sampleRequestString, logger);
            action.Run();
            // Assert that serviceClient.invocations count is 1
            Assert.That(Equals(serviceClient.invocations.Count, 1));
            // Assert that the device id from the invocation is as expected
            Assert.That(string.Equals(serviceClient.invocations[0].device, "rjTest"));
            // Assert that the method name details from the invocation is as expected
            StringAssert.AreEqualIgnoringCase(sampleRequest.MethodName, serviceClient.invocations[0].method.MethodName);

            // get response to object 
            PurchaseTicketPayload actualResponse = JsonConvert.DeserializeObject<PurchaseTicketPayload>(serviceClient.invocations[0].method.GetPayloadAsJson());

            // check the expected against the actuals
            Assert.AreEqual(expectedResponse.DeviceId, actualResponse.DeviceId, "Device IDs do not match");
            Assert.AreEqual(expectedResponse.DeviceType, actualResponse.DeviceType, "Device Types do not match");
            Assert.AreEqual(expectedResponse.TransactionId, actualResponse.TransactionId, "Transaction IDs do not match");
            Assert.AreEqual(expectedResponse.MessageType, actualResponse.MessageType, "Message Types do not match");
            Assert.IsTrue(actualResponse.IsApproved, "Expected Ticket to be approved, but it was rejected");
        }
    }
}
