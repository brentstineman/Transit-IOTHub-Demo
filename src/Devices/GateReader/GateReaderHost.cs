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

            // create our simulated device
            myGateReader = new GateReaderDevice(ConfigurationHandler.getConfig("AppSettings", "deviceConfig"), ConfigurationHandler.getConfig("AppSettings", "IoTConnectionString"));
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
