// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using TransitFunctionApp.Models;
using ClientMessage = Microsoft.Azure.Devices.Client.Message;

namespace simulated_device
{
    class SimulatedDevice
    {
        private static DeviceClient s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private readonly static string s_connectionString = "HostName=TransportationOneWeekHub.azure-devices.net;DeviceId=rjTest;SharedAccessKey=H9leaMcoVkvr8vAlHfKVHR4ww5yaUFhi2OvYWY5RT2E=";

        private static int s_telemetryInterval = 1; // Seconds

        // Handle the direct method call
        private static Task<MethodResponse> SetTelemetryInterval(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);

            Console.WriteLine("Executed direct method: " + methodRequest.Name);

            // Acknowlege the direct method call with a 200 success message
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));

            //// Check the payload is a single integer value
            //if (Int32.TryParse(data, out s_telemetryInterval))
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Telemetry interval set to {0} seconds", data);
            //    Console.ResetColor();

            //    // Acknowlege the direct method call with a 200 success message
            //    string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
            //    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            //}
            //else
            //{
            //    // Acknowlege the direct method call with a 400 error message
            //    string result = "{\"result\":\"Invalid parameter\"}";
            //    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            //}
        }

        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            //double minTemperature = 20;
            //double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                // Create JSON message
                PurchaseTicketRequest purchaseTicketRequest = new PurchaseTicketRequest()
                {
                    DeviceId = "rjTest",
                    //DeviceId = new Guid().ToString(),
                    DeviceType = "Kiosk",
                    MessageType = "Purchase",
                    TransactionId = Guid.NewGuid().ToString(),
                    CreateTime = System.DateTime.UtcNow,
                    Price = rand.Next(2,100), 
                    MethodName = "SetTelemetryInterval"
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

                await Task.Delay(10000);
            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #2 - Simulated device. Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);

            // Create a handler for the direct method call
            s_deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryInterval, null).Wait();
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
    }
}
