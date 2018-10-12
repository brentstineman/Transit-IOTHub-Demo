using System;
using System.Threading;
using Transportation.Demo.Devices.Base;

namespace Transportation.Demo.Devices.GateReader
{
    class TransportationDeviceGateReaderClient
    {
        public static TransportationDeviceClient deviceClient;
        public static string connectionString;

        static void Main(string[] args)
        {
            Console.WriteLine("Transportation Demo- Simulated Gate/Reader. Ctrl-C to exit.\n");

            // Connect to the IoT hub using the MQTT protocol
            connectionString = ConfigurationHandler.getConfig("AppSettings", "IoTConnectionString");
            deviceClient = new TransportationDeviceClient(connectionString);

            while (true)
            {
                //do stuff

                Thread.Sleep(30000);

            }

            Console.ReadLine();
        }
    }
}
