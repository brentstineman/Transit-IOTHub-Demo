using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using TransitFunctionApp;
using Transportation.Demo.Functions;
using Transportation.Demo.Shared.Models;

namespace TransitFunctionAppTest
{
    [TestFixture]
    class SetGateDirectionTest
    {
        FakeLogger logger;
        [SetUp]
        public void SetUp()
        {
            logger = new FakeLogger();
        }

        [Test]
        public void SetDirectionTest()
        {
            FakeInvokeDeviceMethod serviceClient = new FakeInvokeDeviceMethod();

            // define parameters
            string deviceId = "mydevice";
            string direction = GateDirection.In.ToString();
            string deviceMethod = "ReceiveCommandGateChange";

            SetGateDirectionAction action = new SetGateDirectionAction(serviceClient, this.logger);

            var result = action.Run(deviceId, direction).Result;

            // Assert that serviceClient.invocations count is 1
            Assert.AreEqual(1, serviceClient.invocations.Count);

            cmdGateDirectionUpdate expectedResponse = new cmdGateDirectionUpdate()
            {
                Direction = GateDirection.In
            };

            // Assert that the method name details from the invocation is as expected
            StringAssert.AreEqualIgnoringCase(deviceMethod, serviceClient.invocations[0].method.MethodName);

            // get response to object 
            cmdGateDirectionUpdate actualResponse = JsonConvert.DeserializeObject<cmdGateDirectionUpdate>(serviceClient.invocations[0].method.GetPayloadAsJson());

            // check the expected against the actuals
            Assert.AreEqual(deviceId, serviceClient.invocations[0].device, "Device IDs did not match");
            Assert.AreEqual(expectedResponse.Direction, actualResponse.Direction, "Direction did not match");
        }
    }
}
