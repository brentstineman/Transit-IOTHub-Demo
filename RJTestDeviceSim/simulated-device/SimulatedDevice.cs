// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using ClientMessage = Microsoft.Azure.Devices.Client.Message;
using TransitFunctionApp.Models;

namespace simulated_device
{
    class SimulatedDevice
    {
        private static DeviceClient s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private readonly static string s_connectionString = "HostName=TransportationOneWeekHub.azure-devices.net;DeviceId=rjTest;SharedAccessKey=H9leaMcoVkvr8vAlHfKVHR4ww5yaUFhi2OvYWY5RT2E=";

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

                PurchaseTicketRequest purchaseTicketRequest = new PurchaseTicketRequest()
                {
                    DeviceId = "rjTest",
                    //DeviceId = new Guid().ToString(),
                    DeviceType = "Kiosk",
                    MessageType = "Purchase",
                    TransactionId = "123",
                    CreateTime = System.DateTime.UtcNow,
                };


                // Create JSON message
                var telemetryDataPoint = new
                {
                    DeviceId = "rjTest",
                    //DeviceId = new Guid(),
                    deviceType = "Kiosk",
                    MessageType = "",
                    TransactionId = "123",
                    temperature = currentTemperature,
                    humidity = currentHumidity,
                    
                };
                var messageString = JsonConvert.SerializeObject(purchaseTicketRequest);

                var eventJsonBytes = Encoding.UTF8.GetBytes(messageString);
                var message = new ClientMessage(eventJsonBytes)
                {
                    ContentEncoding = "utf-8",
                    ContentType = "application/json"
                };

                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.
                var messageProperties = message.Properties;
                messageProperties.Add("deviceId", "rjTest");

                // Send the telemetry message
                await s_deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #1 - Simulated device. Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
    }
}
