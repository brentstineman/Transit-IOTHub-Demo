using System;
using System.Threading;
using Transportation.Demo.Devices.Base;

namespace Transportation.Demo.Devices.GateReader
{
    class GateReaderHost
    {
        public static GateReaderDevice myGateReader;

        static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated Gate/Reader. Ctrl-C to exit.\n");

            TransportationDeviceClient myClient = new TransportationDeviceClient(ConfigurationHandler.getConfig("AppSettings", "IoTConnectionString"));
            EventScheduler myScheduler = new EventScheduler();


            // create our simulated device
            myGateReader = new GateReaderDevice(ConfigurationHandler.getConfig("AppSettings", "deviceId"), myClient, myScheduler);
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
