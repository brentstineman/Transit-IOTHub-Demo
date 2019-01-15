using System;
using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Transportation.Demo.Shared;
using Transportation.Demo.Shared.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace Transportation.Demo.Shared
{
    public class TransportationDeviceClient : IDeviceClient
    {
        public TransportationDeviceClient(string connectionString)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
        }

        public TransportationDeviceClient(string connectionString, string deviceId)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, deviceId, TransportType.Mqtt);
            if (deviceClient == null)
            {
                throw new ArgumentException($"Could not create the device client for device id [{deviceId}].");
            }
        }
        private static DeviceClient deviceClient;

        public async Task SendMessageAsync(string msg)
        {
            var message = new Message(Encoding.UTF8.GetBytes(msg))
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json"
            };
            await deviceClient.SendEventAsync(message);
        }

        public async Task SendMessageBatchAsync(IEnumerable<string> msgs)
        {
            var messages = msgs.ForEach((x) => { return new Message(x.GetBytes()); });
            deviceClient.SendEventBatchAsync(messages);
        }

        public async Task<Message> ReceiveMessageAsync()
        {
            var message = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(60 * 1));
            return message;
        }

        public async Task RegisterDirectMethodAsync(MethodCallback methodHandler)
        {
            await deviceClient.SetMethodHandlerAsync(methodHandler.Method.Name, methodHandler, null);
        }


        public async Task SetReportedDigitalTwinPropertyAsync(KeyValuePair<string, object> property)
        {
            var tmpCollection = new TwinCollection();
            tmpCollection[property.Key] = property.Value;
            await deviceClient.UpdateReportedPropertiesAsync(tmpCollection);
        }

        public async Task<string> GetDigitalTwinAsync()
        {
            var twin = await deviceClient.GetTwinAsync();
            var json = JsonConvert.SerializeObject(twin, Formatting.Indented);
            return json;
        }
        public async Task<dynamic> GetDynamicDigitalTwinAsync()
        {
            var twin = await deviceClient.GetTwinAsync();
            var dyn = twin.ToExpandoObject();
            return dyn;
        }

    }
}