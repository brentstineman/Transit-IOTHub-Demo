using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using Transportation.Demo.Base.Interfaces;
using Transportation.Demo.Devices.Base.Interfaces;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Devices.Base
{
    public class BaseDevice
    {
        protected IEventScheduler eventScheduler;
        protected IDeviceClient deviceClient;

        protected string deviceId;
        protected string deviceType;

        public BaseDevice(IDeviceConfig deviceConfig, IDeviceClient client, IEventScheduler eventScheduler)
        {
            Contract.Requires<ArgumentNullException>(deviceConfig != null, "deviceConfig parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(client != null, "client parameter cannot be null.");
            Contract.Requires<ArgumentNullException>(eventScheduler != null, "eventScheduler parameter cannot be null.");

            this.eventScheduler = eventScheduler;
            this.deviceClient = client;  // Connect to the IoT hub using the MQTT protocol

            this.deviceId = deviceConfig.DeviceId;
            this.deviceType = deviceConfig.DeviceType;

            // ?? validate device ID on instantiation ?? 
        }

        public void StartAllEvents()
        {
            eventScheduler.StartAll(); 
        }

        public void StopAllEvents()
        {
            eventScheduler.StopAll();
        }

        public void SendMessageToCloud (string messageString)
        {
            var eventJsonBytes = Encoding.UTF8.GetBytes(messageString);
            var clientMessage = new Microsoft.Azure.Devices.Client.Message(eventJsonBytes)
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json"
            };

            // Add a custom application property to the message.
            // An IoT hub can filter on these properties without access to the message body.
            var messageProperties = clientMessage.Properties;
            messageProperties.Add("deviceId", this.deviceId);

            // Send the telemetry message
            this.deviceClient.SendMessageAsync(messageString).Wait();

        }

    }
}
