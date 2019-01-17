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
    /// <summary>
    /// This HTTP triggered function allows you to set the direction of a GateReader device
    /// </summary>
    public static class SetGateDirection
    {
        [FunctionName("SetGateDirection")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "devices/{deviceId}/setdirection={direction}"),] HttpRequest req, string deviceId, string direction,
            ILogger log)
        {
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));

            SetGateDirectionAction action = new SetGateDirectionAction(new ServiceClientInvokeDeviceMethod(serviceClient), log);
            var result = await action.Run(deviceId, direction);

            return result;
        }
    }
}
