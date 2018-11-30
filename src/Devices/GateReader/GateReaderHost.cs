using Newtonsoft.Json;
using System;
using System.Threading;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Devices.GateReader
{
    class GateReaderHost
    {
        public static GateReaderDevice myGateReader;

        static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated Gate/Reader. Ctrl-C to exit.\n");

            // setup the items used by the simulated device
            TransportationDeviceClient myClient = new TransportationDeviceClient(ConfigurationHandler.getConfig("AppSettings", "IoTConnectionString"));
            EventScheduler myScheduler = new EventScheduler();

            // get device configuration details from JSON file
            GateReaderDeviceConfig deviceConfig = JsonConvert.DeserializeObject<GateReaderDeviceConfig>(ConfigurationHandler.GetDeviceRuntimeSettings("deviceConfig"));
            // create our simulated device
            myGateReader = new GateReaderDevice(deviceConfig, myClient, myScheduler);

            // start the device running
            myGateReader.StartAllEvents(); 

            // put the host into a loop
            while (true)
            {
                //do stuff
                Thread.Sleep(30000);
            }

            myGateReader.StopAllEvents();

            Console.ReadLine();
        }

    }
}
