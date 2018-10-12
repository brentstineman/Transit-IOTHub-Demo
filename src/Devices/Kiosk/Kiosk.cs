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
using Newtonsoft.Json.Linq;

namespace Transportation.Demo.Devices.Kiosk
{
    public class TransportationDeviceKioskClient
    {
        public static TransportationDeviceClient _deviceClient;
        public static string _connectionString;

        private static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated device. Ctrl-C to exit.\n");
            
            // Connect to the IoT hub using the MQTT protocol
            _connectionString = getConfig("AppSettings", "IoTConnectionString");
            _deviceClient = new TransportationDeviceClient(_connectionString);
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
            //await _deviceClient.SendMessageAsync(messageString);

            //DeviceClient client = DeviceClient.CreateFromConnectionString(_connectionString, TransportType.Mqtt);
            //client.SetMethodHandlerAsync("ReceivePurchaseTicketResponse", ReceivePurchaseTicketResponse, null).Wait();
            await _deviceClient.SendMessageAsync(messageString);
            //await client.SendEventAsync(message);
            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            Console.WriteLine();
        }

        public static string getConfig(string section, string key)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            IConfigurationSection configurationSection = configuration.GetSection(section).GetSection(key);
            return configurationSection.Value;

        }

        private static async void RegisterDirectMethods()
        {
            await _deviceClient.RegisterDirectMethodAsync(ReceivePurchaseTicketResponse);
        }

        // Handle the direct method call back from Azure
        private static Task<MethodResponse> ReceivePurchaseTicketResponse(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            var json = JObject.Parse(data); //methodRequest.DataAsJson;
            string transactionId = json["TransactionId"].ToString();
            bool isApproved = (bool)json["IsApproved"];

            Console.WriteLine("Executed direct method: " + methodRequest.Name);
            Console.WriteLine($"Transaction Id: {transactionId}");
            Console.WriteLine($"IsApproved: {isApproved}");
            Console.WriteLine();

            // Acknowlege the direct method call with a 200 success message
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));

        }
    }
}