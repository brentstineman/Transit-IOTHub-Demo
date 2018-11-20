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
using System.Collections.Generic;

namespace Transportation.Demo.Devices.Kiosk
{
    public class TransportationDeviceKioskClient
    {
        public static TransportationDeviceClient deviceClient;
        public static string connectionString;
        
        private static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated device. Ctrl-C to exit.\n");
            
            // Connect to the IoT hub using the MQTT protocol
            connectionString = getConfig("AppSettings", "IoTConnectionString");
            deviceClient = new TransportationDeviceClient(connectionString);
            RegisterDirectMethods();

            var act = new Action(()=> { SendMessage(GeneratePurchaseTicket()); });
            var ps = new ProcessScheduler(act, 30, 2);

            Console.ReadLine();
        }

        private static string GeneratePurchaseTicket()
        {
            var random = new Random();
            PurchaseTicketRequest purchaseTicketRequest = new PurchaseTicketRequest()
            {
                DeviceId = "rjTest",
                DeviceType = "Kiosk",
                MessageType = "Purchase",
                TransactionId = Guid.NewGuid().ToString(),
                CreateTime = DateTime.UtcNow,
                Price = random.Next(2, 100),
                MethodName = "ReceivePurchaseTicketResponse"
            };

            var messageString = JsonConvert.SerializeObject(purchaseTicketRequest);

            return messageString;
            
        }

        private static async void SendMessage(string message)
        {
            await deviceClient.SendMessageAsync(message);
            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
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
            await deviceClient.RegisterDirectMethodAsync(ReceivePurchaseTicketResponse);
        }

        private static Task<MethodResponse> ReceivePurchaseTicketResponse (MethodRequest methodRequest, object userContext)
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