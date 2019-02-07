using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Transportation.Demo.Shared;
using Transportation.Demo.Shared.Interfaces;

namespace TransportationDemoTests
{
    public class FakeDeviceClient : IDeviceClient
    {
        private Queue<Message> messages = new Queue<Message>();
        public List<string> sendMessageLog = new List<string>();
        private Twin fakeTwin;
        public List<MethodCallback> directMethods = new List<MethodCallback>();
        public static List<DesiredPropertyUpdateCallback> desiredCallbacks = new List<DesiredPropertyUpdateCallback>();


        public FakeDeviceClient(Twin deviceTwin = null)
        {

            if (deviceTwin == null)
            {
                fakeTwin = new Microsoft.Azure.Devices.Shared.Twin("fakeDevice");
            }
            else
            {
                fakeTwin = deviceTwin;
            }

        }

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

        public Task SetReportedDigitalTwinPropertyAsync(KeyValuePair<string, object> property)
        {
            // if the property already exists, update it
            if (fakeTwin.Properties.Reported.Contains(property.Key))
            {
                fakeTwin.Properties.Reported[property.Key] = property.Value;
            }
            else
            {
                // get current properties
                ExpandoObject newReportedProperties = fakeTwin.Properties.Reported.ToExpandoObject();
                // add in our new one
                newReportedProperties.TryAdd(property.Key, property.Value);
                // save the properties back out
                fakeTwin.Properties.Reported = new TwinCollection(JsonConvert.SerializeObject(newReportedProperties));
            }

            return Task.CompletedTask;
        }

        public Task<string> GetDigitalTwinAsync()
        {
            return Task<string>.Factory.StartNew(() => JsonConvert.SerializeObject(fakeTwin, Formatting.Indented));
        }

        public Task<dynamic> GetDynamicDigitalTwinAsync()
        {
            return Task<dynamic>.Factory.StartNew(() => fakeTwin.ToExpandoObject());
        }

        // collects the various handlers
        public async void RegisterDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callbackHandler)
        {
            // add the new handler
            desiredCallbacks.Add(callbackHandler);

            // Method must be flagged async due to interface signature, this line eliminates code warning 
            await Task.Run(() => { });
        }

        // public method to allow the fake client to manually "trigger" the event handler callback methods
        public async Task OnDesiredPropertyChangedAsync(TwinCollection desiredProperties, object userContext)
        {
            List<Task> taskList = new List<Task>();

            // add each handler to a task list for processing
            foreach (DesiredPropertyUpdateCallback callbackMethod in desiredCallbacks)
            {
                taskList.Add(callbackMethod(desiredProperties, userContext));
            }

            // execute all tasks and wait for them to complete
            await Task.Run(() => Task.WaitAll(taskList.ToArray()));
        }
    }
}
