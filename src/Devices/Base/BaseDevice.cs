﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Transportation.Demo.Shared.Interfaces;
using Transportation.Demo.Devices.Base.Interfaces;
using Transportation.Demo.Shared.Models;
using Microsoft.Azure.Devices.Shared;

namespace Transportation.Demo.Devices.Base
{
    public class BaseDevice : ISimulatedDevice
    {
        protected IEventScheduler _EventScheduler;
        protected IDeviceClient _DeviceClient;
        protected IDeviceConfig _deviceConfig;

        protected string deviceId;
        protected string deviceType;
        protected DeviceStatus? status = null;

        public BaseDevice(IDeviceConfig deviceConfig, IDeviceClient client, IEventScheduler eventScheduler)
        {
            if (deviceConfig == null || client == null || eventScheduler == null)
            {
                throw new ArgumentNullException("one or more parameters are null. All parameters must be provided");
            }

            this._EventScheduler = eventScheduler;
            this._DeviceClient = client;  // Connect to the IoT hub using the MQTT protocol

            this._deviceConfig = deviceConfig; // save for later
            this.deviceId = deviceConfig.DeviceId;
            this.deviceType = deviceConfig.DeviceType;     
            // device status is set during initialization
        }

        public Task InitializeAsync()
        {

            // get twin 
            var myTwin = _DeviceClient.GetDynamicDigitalTwinAsync().Result;

            // set initial status
            if (myTwin != null)
            {
                // check if a status was previously reported
                if (myTwin.Properties.Reported.Contains("status"))
                {
                    // update the status of this device, but don't update device twin
                    this.status = (DeviceStatus)Enum.Parse(typeof(DeviceStatus), myTwin.Properties.Reported["status"].ToString());
                }
                // check if there's a desired status that hasn't been applied yet
                if (myTwin.Properties.Desired.Contains("status"))
                {
                    string desirdeStatus = myTwin.Properties.Desired["status"].ToString(); // get desired state
                                                                                          // if desired state is different then last reported state
                    if (myTwin.Properties.Reported.Contains("status") && myTwin.Properties.Reported["status"] != desirdeStatus)
                    {
                        // use full setter which will also update the device twin
                        this.SetDeviceStatusAsync((DeviceStatus)Enum.Parse(typeof(DeviceStatus), desirdeStatus)).Wait();
                    }
                }
            }
            if (this.status == null) // if no status was set, must be first time startup for this device
            {
                // use full setter which will also update the device twin and the initial status from the config file
                this.SetDeviceStatusAsync((DeviceStatus)Enum.Parse(typeof(DeviceStatus), this._deviceConfig.Status)).Wait();

            }

            _DeviceClient.RegisterDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged);

            return Task.CompletedTask;
        }

        public void StartAllEvents()
        {
            // only start the events if the device is enabled. 
            if (this.status == DeviceStatus.enabled)
            {
                _EventScheduler.StartAll(); 
            }
            else {
                Console.WriteLine("Device disabled, event start aborted.");
            }
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

        public async Task SetDeviceStatusAsync(DeviceStatus newStatus)
        {
            // current status should always match last reported
            if (status != newStatus)  // only act if new status is different then existing
            {
                this.status = newStatus; // change device status

                //Call the helper method to update the status property
                await this._DeviceClient.SetReportedDigitalTwinPropertyAsync(new KeyValuePair<string, object>("status", this.status.ToString()));
                if (status == DeviceStatus.disabled)
                {
                    Console.WriteLine("Device disabled, stopping all events.");

                    StopAllEvents();
                }
                else // we're enabling the device
                {
                    Console.WriteLine("Device enabled, starting all events.");

                    StartAllEvents();
                }
            }
            else // status was the same, disregard
            {
                Console.Write("Device status change ignored, new status is same as old");
            }
               
        }

        public DeviceStatus? GetDeviceStatus()
        {
            return this.status;
        }

        /// <summary>
        /// This method is a callback method that will be triggered when a desired property change event is raised
        /// </summary>
        /// <param name="desiredProperties">a collection of the desired properties</param>
        /// <param name="userContext">the user context of the property changes</param>
        /// <returns></returns>
        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine("Desired property change occured.");

            // if there's a "status" desired state, process it. 
            if (desiredProperties.Contains("status"))
            {
                // uset the SetDeviceStatus method which will handle any reported updates property changes that need to be made
                await this.SetDeviceStatusAsync((DeviceStatus)Enum.Parse(typeof(DeviceStatus), desiredProperties["status"].ToString()));
            }
            //Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));
        }

    }
}
