using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transportation.Demo.Base.Interfaces;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Devices.Base.Interfaces;
using Transportation.Demo.Shared.Models;

namespace Transportation.Demo.Devices.GateReader
{
    public class GateReaderDevice : BaseDevice
    {
        public enum GateDirection { In, Out };

        private GateDirection CurrentDirection;

        public GateReaderDevice(IDeviceConfig deviceConfig, IDeviceClient client, IEventScheduler eventScheduler) 
            : base(deviceConfig, client, eventScheduler)
        {
            TimedSimulatedEvent simulatedEvent = new TimedSimulatedEvent(2500, 1000, this.ValidateTicketEvent);

            // set up any simulated events for this device
            this.eventScheduler.Add(simulatedEvent);

            // register any direct methods we're to recieve
            this.deviceClient.RegisterDirectMethodAsync(ReceiveTicketValidationResponse).Wait();

            // set initial direction
            this.Direction = GateDirection.In;
        }

        public GateDirection Direction
        {
            get { return this.CurrentDirection;  }
            set
            {
                // set device twin property
                this.deviceClient.SetDigitalTwinPropertyAsync(new KeyValuePair<string, object>("GateDirection", value.ToString()));
                // set local cached value
                this.CurrentDirection = value;
            }       
        }

        private bool ValidateTicketEvent()
        {
            GateDirection randomDirection = this.Direction; // default to the 'right' direction

            // calculate if scan was from 'right' direction
            Random gen = new Random();
            if (gen.Next(100) > (100-50)) // if from 'wrong' direction
            {
                // get other direction
                randomDirection = (this.Direction == GateDirection.In ? GateDirection.Out : GateDirection.In);
            }

            bool needtovalidate = randomDirection == this.Direction; // don't restart timer

            if (needtovalidate)
            {
                ValidateTicketRequest cloudRequest = new ValidateTicketRequest()
                {
                    DeviceId = this.deviceId,
                    DeviceType = this.deviceType,
                    MessageType = "ValdiateTicket",
                    TransactionId = Guid.NewGuid().ToString(),
                    CreateTime = System.DateTime.UtcNow,
                    MethodName = "ReceiveTicketValidationResponse" // must match callback method
                };

                var messageString = JsonConvert.SerializeObject(cloudRequest);
                SendMessageToCloud(messageString);

                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Wrong Direction Scan");
                Console.WriteLine();

            }

            return needtovalidate;

        }

        private Task<MethodResponse> ReceiveTicketValidationResponse(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            ValidateTicketResponse validationResponse = JsonConvert.DeserializeObject<ValidateTicketResponse>(data);

            var json = JObject.Parse(data);

            Console.WriteLine("Executed direct method: " + methodRequest.Name);
            Console.WriteLine($"Transaction Id: {validationResponse.TransactionId}");
            Console.WriteLine($"IsApproved: {validationResponse.IsApproved}");
            Console.WriteLine();

            if (validationResponse.IsApproved)
            {
                SendGateOpenedMessageToCloud(validationResponse);
            }

            // Acknowlege the direct method call with a 200 success message
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";

            // restart the purchase ticket event
            this.eventScheduler.Start(0);

            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        private bool SendGateOpenedMessageToCloud(ValidateTicketResponse responsePayload)
        {
            GateOpenedRequest issueTicketRequest = new GateOpenedRequest()
            {
                DeviceId = this.deviceId,
                DeviceType = this.deviceType,
                MessageType = "GateOpened",
                TransactionId = responsePayload.TransactionId,
                CreateTime = System.DateTime.UtcNow
            };

            var messageString = JsonConvert.SerializeObject(issueTicketRequest);
            SendMessageToCloud(messageString);

            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            Console.WriteLine();

            return false; // don't restart timer
        }
    }
}
