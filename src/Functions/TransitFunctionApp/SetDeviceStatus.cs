using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Client;
using System.Collections.Generic;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Transportation.Demo.Shared;

namespace Transportation.Demo.Functions
{
    public static class SetDeviceStatus
    {
        [FunctionName("SetDeviceStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SetDeviceStatus/{DeviceId:guid}/{status}")] HttpRequest req,
            string deviceId,
            string status,
            ILogger log)
        {
            var client = new TransportationDeviceClient(Environment.GetEnvironmentVariable("IotHubConnectionString"), deviceId);
            var action = new SetDeviceStatusAction(client, log);
            await action.SetDeviceStatus(status);
            return new OkResult();
        }
    }
}
