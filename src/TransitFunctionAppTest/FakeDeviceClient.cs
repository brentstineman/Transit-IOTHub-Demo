using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Transportation.Demo.Base.Interfaces;

namespace TransportationDemoTests
{
    public class FakeDeviceClient : IDeviceClient
    {
        private Queue<Message> messages = new Queue<Message>();
        public List<string> sendMessageLog = new List<string>();
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

            return Task.Factory.StartNew(() => { });
        }

        public Task SendMessageAsync(string msg)
        {
            sendMessageLog.Add(msg);
            return Task.Factory.StartNew(()=> { });
        }

        public Task SendMessageBatchAsync(IEnumerable<string> msgs)
        {
            foreach(var msg in msgs)
            {
                sendMessageLog.Add(msg);
            }
            return Task.Factory.StartNew(() => { });
        }
    }
}
