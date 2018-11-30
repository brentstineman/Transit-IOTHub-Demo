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


/// <summary>
/// 
/// This class is a simulation of a gate/reader device. Its job would be to scan/read the tickets 
/// or transit cards of riders and then allow them on or off the platform. The gate can be configured 
/// to let folks either on off off the platform to help control the flow of traffic. 
/// 
/// The ticket/card swipes are triggered by a timer. If the swipe is in the direction that the gate is 
/// current configured, the device will send a message to the cloud asking for the ticket to be validated. 
/// The device then waits for a response to this request before the timer restarts and a new event can be triggered. 
/// 
/// Once a response is received back from the cloud (via the 'ReceiveTicketValidationResponse' method), its result
/// is checked and if approved, the gate is opened. In both cases, the timer is restarted so another swipe event can
/// be triggered.
/// 
/// </summary>
namespace Transportation.Demo.Devices.GateReader
{
    public class GateReaderDevice : BaseDevice
    {
        public enum GateDirection { In, Out };

        private GateDirection CurrentDirection;
        private GateReaderDeviceConfig deviceConfig;

        public GateReaderDevice(GateReaderDeviceConfig deviceConfig, IDeviceClient client, IEventScheduler eventScheduler) 
            : base(deviceConfig, client, eventScheduler)
        {
            // save device configuration
            this.deviceConfig = deviceConfig;

            // set initial direction
            this.SetDirectionAsync((GateDirection)Enum.Parse(typeof(GateDirection), deviceConfig.initialDirection)).Wait();

            TimedSimulatedEvent simulatedEvent = new TimedSimulatedEvent(2500, 1000, this.SimulatedTicketSwipeOccured);

            // set up any simulated events for this device
            this._EventScheduler.Add(simulatedEvent);

            // register any direct methods we're to recieve
            this._DeviceClient.RegisterDirectMethodAsync(ReceiveTicketValidationResponse).Wait();

        }

        public GateDirection Direction
        {
            get { return this.CurrentDirection;  }
        }

        /// <summary>
        /// This method updates the direction of the Gate Reader to either In or Out
        /// This updates not only the internal state of the device, but also its IOT Hub Device Twin
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetDirectionAsync(GateDirection value)
        {
            // set device twin property
            await this._DeviceClient.SetDigitalTwinPropertyAsync(new KeyValuePair<string, object>("GateDirection", value.ToString()));

            // set local cached value. Done after the device twin update so if it failed, we didn't update our local value
            this.CurrentDirection = value;

            return;
        }


    /// <summary>
    /// This event is called by a timer to simulate a card/ticket being scanned. It checks the direction of the gate (in/out)
    /// And if the swipe is in the right direction will ask the "cloud" to validate the card and wait for that result.
    /// </summary>
    private bool SimulatedTicketSwipeOccured()
        {
            GateDirection randomDirection = this.Direction; // default to the 'right' direction

            // randomize if swipe was from 'right' direction, if so we need to validate the ticket
            Random gen = new Random();
            if (gen.Next(100) > (100-this.deviceConfig.PercentOfWrongWay)) // if from 'wrong' direction
            {
                // get other direction
                randomDirection = (this.Direction == GateDirection.In ? GateDirection.Out : GateDirection.In);
            }
            bool needtovalidate = randomDirection == this.Direction; // don't restart timer

            // if the swipe was from the right direction, as for the ticket to be validated
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
            // if the swipe was from the wrong direction, we ignore it
            else
            {
                Console.WriteLine();
                Console.WriteLine("Swipe from Wrong Direction, ignoring");
            }

            // if we asked for the ticket to be validated, return false so we don't trigger another swipe
            return needtovalidate == false; 

        }

        /// <summary>
        /// This method is called by Azure IOT Hub and represents a direct method call to the device
        /// This is the response to 'SimulatedTicketSwipeOccured' request for a ticket validation
        /// </summary>
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
            this._EventScheduler.Start(0);

            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        /// <summary>
        /// This is a fire and forget method that sends a message to the cloud to let it know the gate was opened.
        /// This message will be sent if the 'ReceivedTicketValidationResponse' method got a "ticket approved" response
        /// </summary>
        private bool SendGateOpenedMessageToCloud(ValidateTicketResponse responsePayload)
        {
            GateOpenedNotification issueTicketRequest = new GateOpenedNotification()
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
