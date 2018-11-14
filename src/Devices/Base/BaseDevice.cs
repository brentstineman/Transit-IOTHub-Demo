using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Transportation.Demo.Devices.Base
{
    public class BaseDevice
    {
        public List<SimulatedEvent> EventList = new List<SimulatedEvent>();

        protected TransportationDeviceClient deviceClient;
        private string connectionString;
        protected dynamic deviceSettings; 

        protected string deviceId;
        protected string deviceType;

        public BaseDevice(string deviceConfigFile, string connectionString)
        {
            this.connectionString = connectionString; // save this for later

            // Connect to the IoT hub using the MQTT protocol
            deviceClient = new TransportationDeviceClient(connectionString);

            deviceSettings = GetRuntimeSettings(deviceConfigFile);
            this.deviceId = deviceSettings.deviceId;
            this.deviceType = deviceSettings.deviceType;

            // ?? validate device ID on instantiation ?? 
        }

        public void StartAllEvents()
        {
            foreach(SimulatedEvent myevent in EventList)
            {
                myevent.Start();
            }
        }

        public void StopAllEvents()
        {
            foreach (SimulatedEvent myevent in EventList)
            {
                myevent.Stop();
            }

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

        private dynamic GetRuntimeSettings(string fileLocation)
        {
            dynamic resultValue; 

            // read JSON directly from a file
            using (StreamReader file = File.OpenText(fileLocation))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                resultValue = (JObject)JToken.ReadFrom(reader);
            }

            return resultValue;
        }
    }
}
