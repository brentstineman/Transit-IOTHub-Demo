﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Transportation.Demo.Shared;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace Transportation.Demo.Devices.Base
{
    public class TransportationDeviceClient
    {
        public TransportationDeviceClient(string connectionString)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
        }
        private static DeviceClient deviceClient;
        
        public async Task SendMessageAsync(string msg)
        {
            var message = new Message(Encoding.UTF8.GetBytes(msg)) { ContentEncoding="utf-8", ContentType="application/json"};
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
            await deviceClient.SetMethodHandlerAsync(nameof(methodHandler), methodHandler, null);
        }

        public async Task SetDigitalTwinPropertyAsync(KeyValuePair<string, object> property)
        {
            var reportedProperty = new TwinCollection();
            reportedProperty[property.Key] = property.Value;
            await deviceClient.UpdateReportedPropertiesAsync(reportedProperty);
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