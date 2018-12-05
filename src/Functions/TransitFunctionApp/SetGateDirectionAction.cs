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
using TransitFunctionApp;

namespace Transportation.Demo.Functions
{
    public class SetGateDirectionAction
    {
        IInvokeDeviceMethod serviceClient;
        ILogger log;

        // Use the IInvokeDeviceMethod from the caller to allow for dependency injection.
        public SetGateDirectionAction(IInvokeDeviceMethod client, ILogger logger)
        {
            serviceClient = client;
            log = logger;
        }

        public async Task<IActionResult> Run(string deviceId, string direction)
        {
            // create command message
            CloudToDeviceMethod methodInvocation = new CloudToDeviceMethod("ReceiveCommandGateChange")
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
        }
    }
}
