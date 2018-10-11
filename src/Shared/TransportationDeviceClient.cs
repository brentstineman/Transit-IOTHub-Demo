using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation.IoTCore
{
    [Obsolete("Use implementation located in Transportation.Demo.Devices.Base")]
    public class TransportationDeviceClient
    {
        private DeviceClient deviceClient;
        public TransportationDeviceClient(string connectionString)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
        }

        public async Task SendMessageAsync(string msg)
        {
            var message = new Message(msg.GetBytes());
            await deviceClient.SendEventAsync(message);
        }

        public async Task SendMessageBatchAsync(IEnumerable<string> msgs)
        {
            var messages = new List<Message>();
            foreach (var item in msgs)
            {
                messages.Add(new Message(item.GetBytes()));
            }
            await deviceClient.SendEventBatchAsync(messages);
        }
        
        public async Task<Message> ReceiveMessageAsync()
        {
            var message = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(60 * 1));
            return message;
        }
        
    }
}
