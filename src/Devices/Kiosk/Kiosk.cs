// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Transportation.Demo.Devices.Base;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using System.Text;
using Transportation.Demo.Shared.Models;
using System.Threading;

namespace Transportation.Demo.Devices.Kiosk
{
    public class TransportationDeviceKioskClient
    {
        public static TransportationDeviceClient deviceClient;
        public static string connectionString;
        //private static Microsoft.Azure.Devices.Client.DeviceClient _deviceClient;

        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {

            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            Random random = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + random.NextDouble() * 15;
                double currentHumidity = minHumidity + random.NextDouble() * 20;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                // Send the message
                await deviceClient.SendMessageAsync(messageString);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated device. Ctrl-C to exit.\n");
            
            // Connect to the IoT hub using the MQTT protocol
            connectionString = getConfig("AppSettings", "IoTConnectionString");
            deviceClient = new TransportationDeviceClient(connectionString);
            RegisterDirectMethods();
            while (true)
            {
                SendPurchaseTicketMessageToCloudAsync();

                Thread.Sleep(30000);

            }
        }

        private static async void SendPurchaseTicketMessageToCloudAsync()
        {
            var random = new Random();
            PurchaseTicketRequest purchaseTicketRequest = new PurchaseTicketRequest()
            {
                DeviceId = "rjTest",
                DeviceType = "Kiosk",
                MessageType = "Purchase",
                TransactionId = Guid.NewGuid().ToString(),
                CreateTime = System.DateTime.UtcNow,
                Price = random.Next(2, 100),
                MethodName = "ReceivePurchaseTicketResponse"
            };

            var messageString = JsonConvert.SerializeObject(purchaseTicketRequest);

            var eventJsonBytes = Encoding.UTF8.GetBytes(messageString);
            var message = new Microsoft.Azure.Devices.Client.Message(eventJsonBytes)
            {
                ContentEncoding = "utf-8",
                ContentType = "application/json"
            };

            // Add a custom application property to the message.
            // An IoT hub can filter on these properties without access to the message body.
            var messageProperties = message.Properties;
            messageProperties.Add("deviceId", "rjTest");

            // Send the telemetry message
            //await deviceClient.SendMessageAsync(messageString);

            DeviceClient client = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
            client.SetMethodHandlerAsync("ReceivePurchaseTicketResponse", ReceivePurchaseTicketResponse, null).Wait();
            await client.SendEventAsync(message);

            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            Console.WriteLine();
        }

        public static string getConfig(string section, string key)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            // configurationSection.Key => FilePath
            // configurationSection.Value => C:\\temp\\logs\\output.txt
            IConfigurationSection configurationSection = configuration.GetSection(section).GetSection(key);
            return configurationSection.Value;

        }

        private static async void RegisterDirectMethods()
        {
            await deviceClient.RegisterDirectMethodAsync(ReceivePurchaseTicketResponse);
        }

        /// <summary>
        /// Provides an Example of a Direct Method
        /// Feel free to use this as a template and then delete this once we have this implemented
        /// 
        /// The name of the method should exactly match what the direct method string is being called from the IoT Hub.
        /// </summary>
        /// <param name="methodRequest"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        private static Task<MethodResponse> DirectMethodExample(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t *** {nameof(DirectMethodExample)} was called.");

            Console.WriteLine();
            Console.WriteLine("\t{0}", methodRequest.DataAsJson);
            Console.WriteLine();

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        // Handle the direct method call
        private static Task<MethodResponse> ReceivePurchaseTicketResponse(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            var json = methodRequest.DataAsJson;

            Console.WriteLine("Executed direct method: " + methodRequest.Name);

            // Acknowlege the direct method call with a 200 success message
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));

        }
    }
}