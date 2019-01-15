using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transportation.Demo.Shared.Interfaces;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Functions
{
    public class SetDeviceStatusAction
    {
        ITransportationJobClient client;
        ILogger logger;

        public SetDeviceStatusAction(ITransportationJobClient jobClient, ILogger log)
        {
            this.client = jobClient;
            this.logger = log;
        }

        public async Task<IActionResult> SetDeviceStatus(string deviceId, string status)
        {
            Transportation.Demo.Shared.Models.DeviceStatus newStatus;

            if (!Enum.TryParse(status, out newStatus))
            {
                throw new ArgumentException("device status must be  of 'direction' must be 'enabled' or 'disabled'");
            }

            logger.LogInformation($" Queuing up job to set device status to {newStatus}");

            // create twin
            Twin twin = new Twin(deviceId);
            twin.Properties.Desired["status"] = status.ToLower();

            // submit job
            string JobId = await client.StartTwinUpdateJobAsync($"DeviceId IN ['{deviceId}']", twin, DateTime.UtcNow);

            // monitor job progress, will wait until finished
            JobStatus result = await client.MonitorJobStatusAsync(JobId);

            // check for success/fail
            ObjectResult returnVal;
            // only possible return values are completed or failed
            if ((result == JobStatus.Completed))
            {
                returnVal = new OkObjectResult($"Job {JobId} completed.");
            }
            else
            {
                returnVal = new BadRequestObjectResult($"Job {JobId} Failed");
            }

            return returnVal;
        }
    }
}
