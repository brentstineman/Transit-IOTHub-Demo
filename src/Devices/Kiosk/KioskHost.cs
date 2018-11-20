
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

            TransportationDeviceClient myClient = new TransportationDeviceClient(ConfigurationHandler.getConfig("AppSettings", "IoTConnectionString"));
            EventScheduler myScheduler = new EventScheduler();
            string myDeviceId = ConfigurationHandler.getConfig("AppSettings", "deviceId");

            // create our simulated device
            myKiosk = new KioskDevice(myDeviceId, myClient, myScheduler);
            // start the device running
            myKiosk.StartAllEvents();

            while (true)
            {
                Thread.Sleep(30000);
            }
        }

 
    }
}