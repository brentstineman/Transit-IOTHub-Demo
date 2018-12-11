﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Transportation.Demo.Base.Interfaces;
using Transportation.Demo.Devices.Base.Interfaces;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Devices.Base
{
    public enum DeviceStatus { enabled, disabled };

    public class BaseDevice
    {
        protected IEventScheduler _EventScheduler;
        protected IDeviceClient _DeviceClient;

        protected string deviceId;
        protected string deviceType;
        protected DeviceStatus status;

        public BaseDevice(IDeviceConfig deviceConfig, IDeviceClient client, IEventScheduler eventScheduler)
        {
            if (deviceConfig == null || client == null || eventScheduler == null)
            {
                throw new ArgumentNullException("one or more parameters are null. All parameters must be provided");
            }

            this._EventScheduler = eventScheduler;
            this._DeviceClient = client;  // Connect to the IoT hub using the MQTT protocol

            this.deviceId = deviceConfig.DeviceId;
            this.deviceType = deviceConfig.DeviceType;
            // When the device first registers, it should default to a "disabled" state
            InitializeStatus(deviceConfig);
            // ?? validate device ID on instantiation ?? 
        }

        public new void InitializeStatus(IDeviceConfig deviceConfig)
        {
            // set initial status. Use configuration value as default
            string intialStatus = deviceConfig.Status;

            // get twin 
            var myTwin = _DeviceClient.GetDynamicDigitalTwinAsync().Result;

            if (myTwin.Properties.Reported.Contains("status"))
            {
                // if there is a status property, set the value status, don't use the Set method so we don't trigger a device twin property udpate
                this.status = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), myTwin.Properties.Reported["status"]);
            }
            else // direction wasn't already set
            {
                // set direction of device via the setter so we update the device twin
                this.SetDeviceStatus((DeviceStatus)Enum.Parse(typeof(DeviceStatus), intialStatus)).Wait();
            }
        }
            public void StartAllEvents()
        {
            _EventScheduler.StartAll(); 
        }

        public void StopAllEvents()
        {
            _EventScheduler.StopAll();
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
            this._DeviceClient.SendMessageAsync(messageString).Wait();

        }

        public async Task SetDeviceStatus(DeviceStatus status)
        {
             this.status = status;
            //Call the helper method to update the status property
            await this._DeviceClient.SetDigitalTwinPropertyAsync(new KeyValuePair<string, object>("status", this.status));
            if (status == DeviceStatus.disabled)
            {
                StopAllEvents();
            }
        }
        public DeviceStatus GetDeviceStatus()
        {
            return this.status;
        }
    }
}
