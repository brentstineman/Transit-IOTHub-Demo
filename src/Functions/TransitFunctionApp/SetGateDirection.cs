using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Functions
{
    /// <summary>
    /// This HTTP triggered function allows you to set the direction of a GateReader device
    /// </summary>
    public static class SetGateDirection
    {
        [FunctionName("SetGateDirection")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "devices/{deviceId}/direction={direction}"),] HttpRequest req, string deviceId, string direction,
            ILogger log)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));

            // create command message
            var methodInvocation = new CloudToDeviceMethod("ReceiveCommandGateChange")
            {
                ResponseTimeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                cmdGateDirectionUpdate command = new cmdGateDirectionUpdate()
                {
                    Direction = (GateDirection)Enum.Parse(typeof(GateDirection), direction)
                };
                methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(command));
            }
            catch (System.ArgumentException)
            {
                return new BadRequestObjectResult("value of 'direction' must be 'In' or 'Out'");
            }


            try
            {
                // Invoke the direct method and get the response from the simulated device.
                var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

                Console.WriteLine("Response status: {0}, payload:", response.Status);
                Console.WriteLine(response.GetPayloadAsJson());
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException ex)
            {
                return new BadRequestObjectResult($"Device '{deviceId}' either does not exist or is not responding");
            }

            return new OkObjectResult($"Device {deviceId} updated to new direction: {direction}");
            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
