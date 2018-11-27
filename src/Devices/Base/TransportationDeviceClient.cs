using System;
using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Transportation.Demo.Shared;
using Transportation.Demo.Base.Interfaces;

namespace Transportation.Demo.Devices.Base
{
    public class TransportationDeviceClient : IDeviceClient
    {
        public TransportationDeviceClient(string connectionString)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
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
    }
}