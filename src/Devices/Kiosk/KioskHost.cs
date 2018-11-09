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
    public class KioskHost
    {
        public static KioskDevice myKiosk;

        private static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated device. Ctrl-C to exit.\n");

            // create our simulated device
            myKiosk = new KioskDevice(ConfigurationHandler.getConfig("AppSettings", "IoTConnectionString"));
            // start the device running
            myKiosk.StartAllEvents();

            while (true)
            {
                Thread.Sleep(30000);
            }
        }

 
    }
}