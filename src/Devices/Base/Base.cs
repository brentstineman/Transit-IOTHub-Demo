// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Transportation.Demo.Devices.Base
{
    public class TransportationDeviceClient
    {
        private static DeviceClient deviceClient;
        
        private readonly static string connectionString = "HostName=TransportationOneWeekHub.azure-devices.net;DeviceId=BenDevice;SharedAccessKey=W2HdWm8wKlosvB0tR2oKyV7FQ4vLfjjue9SXlQgikwM=";

        private static int telemetryInterval = 1; // Seconds

        public async Task SendMessageAsync(string msg)
        {
            var message = new Message(Encoding.UTF8.GetBytes(msg));
            deviceClient.SendEventAsync(message);
        }

        public async Task SendMessageBatchAsync(IEnumerable<string> msgs)
        {
            var messages = new List<Message>();
            foreach (var item in msgs)
            {
                messages.Add(new Message(Encoding.UTF8.GetBytes(item)));
            }
            deviceClient.SendEventBatchAsync(messages);
        }
        
        public async Task<Message> ReceiveMessageAsync()
        {
            var message = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(60 * 1));
            return message;
        }
        
        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                // Send the tlemetry message
                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(telemetryInterval * 1000);
            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated device. Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

            // Create a handler for the direct method call
            //deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryInterval, null).Wait();
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
        
    }
}