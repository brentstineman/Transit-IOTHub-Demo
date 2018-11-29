using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Transportation.Demo.Shared.Models;

namespace TransitFunctionApp
{
    // Class containing the core logic  for 'PurchaseTicket' Azure function.
    public class ValidateTicketAction
    {
        IInvokeDeviceMethod serviceClient;
        string payloadMessage;
        ILogger log;

        // Use the IInvokeDeviceMethod from the caller to allow for dependency injection.
        public ValidateTicketAction(IInvokeDeviceMethod client, string payload, ILogger logger)
        {
            serviceClient = client;
            payloadMessage = payload;
            log = logger;
        }

        public void Run()
        {
            // 95% chance that ticket validation is successful is approved
            Random gen = new Random();
            bool purchaseApproved = gen.Next(100) <= 95;

            // process each message
            ValidateTicketRequest ticketRequestMessage = JsonConvert.DeserializeObject<ValidateTicketRequest>(payloadMessage);

            try
            {
                string methodName = ticketRequestMessage.MethodName;
                string deviceId = ticketRequestMessage.DeviceId;
                string transactionId = ticketRequestMessage.TransactionId;

                var payload = new PurchaseTicketPayload()
                {
                    TransactionId = transactionId,
                    IsApproved = purchaseApproved,
                    DeviceId = ticketRequestMessage.DeviceId,
                    DeviceType = ticketRequestMessage.DeviceType,
                    MessageType = ticketRequestMessage.MessageType,
                };

                log.LogInformation($"Response Method: {methodName}");

                InvokeMethod(methodName, payload).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }

        private async Task InvokeMethod(
            string methodName,
            PurchaseTicketPayload purchaseTicketPayload)
        {
            TimeSpan responseTimeoutInSeconds = TimeSpan.FromSeconds(30);
            var methodInvocation = new CloudToDeviceMethod(methodName)
            {
                ResponseTimeout = responseTimeoutInSeconds
            };
            var payload = JsonConvert.SerializeObject(purchaseTicketPayload);
            methodInvocation.SetPayloadJson(payload);

            try
            {
                // Invoke the direct method asynchronously and get the response from the simulated device.
                var response = await serviceClient.InvokeDeviceMethodAsync(purchaseTicketPayload.DeviceId, methodInvocation);
                if (response != null)
                {
                    Console.WriteLine("Response status: {0}, payload:", response.Status);
                    Console.WriteLine(response.GetPayloadAsJson());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
