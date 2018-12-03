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
using System.Text;

namespace Transportation.Demo.Functions
{
    public static class QueryDevices
    {
        [FunctionName("QueryDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHubConnectionString"));

            StringBuilder deviceQuery = new StringBuilder("select * from devices where status='enabled'");

            if (req.Query.ContainsKey("id")) {
                Guid tempId;
                if (!Guid.TryParse(req.Query["id"], out tempId))
                {
                    return new BadRequestResult();
                }
                deviceQuery.Append($" and deviceId = '{req.Query["id"]}'");
                
            }
            // TODO: validate device type. For type, we have a list of allowed device types we can check
            if (req.Query.ContainsKey("type")) {
                deviceQuery.Append($" and deviceType = '{req.Query["type"]}'");
            }
            if (req.Query.ContainsKey("location"))
            {
                // Check for SQL injection --disallow the location from ending its SQL string
                if (((string)req.Query["location"]).Contains("'"))
                {
                    return new BadRequestResult();
                }
                deviceQuery.Append($"and tags.location = '{req.Query["location"]}'");
            }

            var query = registryManager.CreateQuery(deviceQuery.ToString());

            var devices = await query.GetNextAsTwinAsync();

            return new OkObjectResult(JsonConvert.SerializeObject(devices));
        }
    }
}
