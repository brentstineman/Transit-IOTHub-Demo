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
        IDeviceClient client;
        ILogger logger;

        public SetDeviceStatusAction(IDeviceClient deviceClient, ILogger log)
        {
            this.client = deviceClient;
            this.logger = log;
        }

        public async Task SetDeviceStatus(string status)
        {
            DeviceStatus newStatus;
            if (!Enum.TryParse(status, out newStatus))
            {
                throw new ArgumentException($"Device Status [{status}] is unknown.");
            }

            logger.LogInformation($"Setting device status to {newStatus}");
            await client.SetDigitalTwinPropertyAsync(new KeyValuePair<string, object>("status", newStatus));
        }
    }
}
