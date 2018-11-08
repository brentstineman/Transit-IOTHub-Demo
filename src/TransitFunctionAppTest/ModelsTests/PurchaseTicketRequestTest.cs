using NUnit.Framework;
using TransitFunctionApp;

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
            var payload = @"{'DeviceId':'rjTest','DeviceType':'Kiosk','MessageType':'Purchase','TransactionId':'c3d8b13c - 3e8f - 4e57 - a7d5 - ae8adaa8c2b2','CreateTime':'2018 - 10 - 11T20: 49:58.0667867Z','OriginLocation':null,'DestinationLocation' :null,'DepartureTime':'0001 - 01 - 01T00: 00:00','Price':'54','MethodName':'SetTelemetryInterval'}";
            FakeInvokeDeviceMethod serviceClient = new FakeInvokeDeviceMethod();
            PurchaseTicketAction action = new PurchaseTicketAction(serviceClient, payload, logger);
            action.Run();
            // Assert that serviceClient.invocations count is 1
            Assert.That(Equals(serviceClient.invocations.Count, 1));
            // Assert that the device id from the invocation is as expected
            Assert.That(string.Equals(serviceClient.invocations[0].device, "rjTest"));
            // Assert that the method name details from the invocation is as expected
            StringAssert.AreEqualIgnoringCase(serviceClient.invocations[0].method.MethodName, "SetTelemetryInterval");
            // Assert that the method payload details from the invocation is as expected
            StringAssert.AreEqualIgnoringCase(serviceClient.invocations[0].method.GetPayloadAsJson(), @"{""IsApproved"":true,""TransactionId"":""c3d8b13c - 3e8f - 4e57 - a7d5 - ae8adaa8c2b2"",""DeviceId"":""rjTest"",""DeviceType"":""Kiosk"",""MessageType"":""Purchase""}");

        }
    }
}
