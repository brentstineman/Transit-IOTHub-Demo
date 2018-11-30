using Newtonsoft.Json;
using NUnit.Framework;
using TransitFunctionApp;
using Transportation.Demo.Shared.Models;

namespace TransitFunctionAppTest
{
    [TestFixture]
    class TicketValidationRequestTest
    {
        FakeLogger logger;
        [SetUp]
        public void SetUp()
        {
            logger = new FakeLogger();
        }

        [Test]
        public void TicketValidationTest()
        {
            // sample payload to test 
            ValidateTicketRequest testRequest = new ValidateTicketRequest()
            {
                DeviceId = "testID",
                DeviceType = "GateReader",
                MessageType = "ValdiateTicket",
                TransactionId = "fakeTransactionId",
                CreateTime = System.DateTime.UtcNow,
                MethodName = "ReceiveTicketValidationResponse"
            };
            string requestPayload = JsonConvert.SerializeObject(testRequest);

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the ticket validation method invocation.");

            FakeInvokeDeviceMethod serviceClient = new FakeInvokeDeviceMethod();
            ValidateTicketAction action = new ValidateTicketAction(serviceClient, requestPayload, logger);
            action.Run();

            // Assert that serviceClient.invocations count is 1
            Assert.That(Equals(1, serviceClient.invocations.Count));

            TestContext.WriteLine(string.Empty);
            TestContext.WriteLine(">> Testing the ticket validation method response.");

            // get the result and deserialize is back into our response object
            ValidateTicketResponse actualResponse = JsonConvert.DeserializeObject<ValidateTicketResponse>(serviceClient.invocations[0].method.GetPayloadAsJson());

            // Assert that various response values against expected
            Assert.AreEqual(testRequest.DeviceId, actualResponse.DeviceId, "Device IDs do not match");
            Assert.AreEqual(testRequest.DeviceType, actualResponse.DeviceType, "Device Types do not match");
            Assert.AreEqual(testRequest.MessageType, actualResponse.MessageType, "Message Types do not match");
            Assert.AreEqual(testRequest.TransactionId, actualResponse.TransactionId, "Trahsaction IDs do not match");
            Assert.AreEqual(testRequest.DeviceId, actualResponse.DeviceId, "Device IDs do not match");
            Assert.IsTrue((actualResponse.IsApproved == true || actualResponse.IsApproved == false), "IsApproved need not match range of allowed values, must be true or false");
            // Assert that the return MethodName is also correct
            Assert.AreEqual(testRequest.MethodName, serviceClient.invocations[0].method.MethodName);
        }
    }
}
