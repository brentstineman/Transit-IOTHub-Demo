using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Transportation.Demo.Shared.Interfaces;

namespace TransportationDemoTests
{
    public class FakeDeviceClient : IDeviceClient
    {
        private Queue<Message> messages = new Queue<Message>();
        public List<string> sendMessageLog = new List<string>();
        public Dictionary<string, object> twinProperties = new Dictionary<string, object>();
        public List<MethodCallback> directMethods = new List<MethodCallback>();


        public void AddFakeMessage(Stream stream)
        {
            messages.Enqueue(new Message(stream));
        }

        public Task<Message> ReceiveMessageAsync()
        {
            if (messages.Count > 0)
            {
                return Task<Message>.Factory.StartNew(() => messages.Dequeue());
            }
            return Task<Message>.Factory.StartNew(() => null);
        }

        public Task RegisterDirectMethodAsync(MethodCallback methodHandler)
        {
            directMethods.Add(methodHandler);

            return Task.CompletedTask;
        }

        public Task SendMessageAsync(string msg)
        {
            sendMessageLog.Add(msg);

            return Task.CompletedTask;
        }

        public Task SendMessageBatchAsync(IEnumerable<string> msgs)
        {
            foreach(var msg in msgs)
            {
                sendMessageLog.Add(msg);
            }

            return Task.CompletedTask;
        }

        public Task SetDigitalTwinPropertyAsync(KeyValuePair<string, object> property)
        {
            if (twinProperties.ContainsKey(property.Key))
            {
                twinProperties[property.Key] = property.Value;
            }
            else
            {
                twinProperties.Add(property.Key, property.Value);
            }

            return Task.CompletedTask;
        }

        public Task<string> GetDigitalTwinAsync()
        {
            //TODO: implement method

            return Task<string>.Factory.StartNew(() => string.Empty);
        }

        public Task<dynamic> GetDynamicDigitalTwinAsync()
        {
            //TODO: implement method

            return Task<dynamic>.Factory.StartNew(() => new System.Dynamic.ExpandoObject());
        }
    }
}
