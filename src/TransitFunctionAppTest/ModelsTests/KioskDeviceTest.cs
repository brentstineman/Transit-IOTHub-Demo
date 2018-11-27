﻿using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;
using TransitFunctionApp;
using Transportation.Demo.Devices.Kiosk;
using Transportation.Demo.Shared.Models;

namespace TransportationDemoTests
{
    [TestFixture]
    class KioskDeviceTest
    {
        FakeDeviceClient fakeDeviceClient = new FakeDeviceClient();
        FakeEventScheduler fakeScheduler = new FakeEventScheduler();

        KioskDeviceConfig deviceconfig;

        // test initialization
        public KioskDeviceTest()
        {
            deviceconfig = new KioskDeviceConfig()
            {
                DeviceId = "myFakeDevice",
                DeviceType = "Kiosk",
                InitialStockCount = 100,
                LowStockThreshold = 98
            };

        }

        [Test]
        public void TestBaseKioskDevice()
        {
            TestContext.WriteLine(">>Testing the Kiosk Device's base functionality..");

            KioskDevice device = new KioskDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            // Make sure the device has one simulated event
            Assert.AreEqual(fakeScheduler.EventList.Count, 1);

            // make sure the device has one direct callback method
            Assert.AreEqual(fakeDeviceClient.directMethods.Count, 1);
        }

        [Test]
        public void TestKioskPurchaseTicket()
        {
            TestContext.WriteLine("\n>> Testing the purchase ticket simulated event..");

            KioskDevice device = new KioskDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            // execute a purchase ticket event and check the result, it should always be false
            Assert.IsFalse(fakeScheduler.EventList[0]());
            // that delegate should have sent one message to the cloud
            Assert.AreEqual(fakeDeviceClient.sendMessageLog.Count, 1);

            // check the message sent to make sure its correct
            // create a sample request for comparison
            PurchaseTicketRequest expectedRequest = new PurchaseTicketRequest()
            {
                DeviceId = deviceconfig.DeviceId,
                DeviceType = deviceconfig.DeviceType,
                MessageType = "Purchase",
                TransactionId = "fakeId",
                CreateTime = System.DateTime.UtcNow,
                Price = 1,
                MethodName = "ReceivePurchaseTicketResponse"
            };
            // get request message into an object so we can compare it
            PurchaseTicketRequest actualRequest = JsonConvert.DeserializeObject<PurchaseTicketRequest>(fakeDeviceClient.sendMessageLog[0]);
            // compare properties to make sure they're valid. 
            Assert.AreEqual(actualRequest.DeviceId, expectedRequest.DeviceId);
            Assert.AreEqual(actualRequest.DeviceType, expectedRequest.DeviceType);
            Assert.AreEqual(actualRequest.MessageType, expectedRequest.MessageType);
            // skipping the TransactionID and CreationTime
            Assert.IsTrue(actualRequest.Price > 2);
            Assert.IsTrue(actualRequest.Price < 100);
            Assert.AreEqual(actualRequest.MethodName, expectedRequest.MethodName);

            //
            /// test the CloudToEvent PurchaseResponse call we expect back
            //
            TestContext.WriteLine("\n>> Testing the ticket approval direct method..");

            // test the direct method itself
            PurchaseTicketPayload approvePurchaseMethodkRequest = new PurchaseTicketPayload()
            {
                IsApproved = true,
                DeviceId = expectedRequest.DeviceId,
                DeviceType = expectedRequest.DeviceType,
                MessageType = expectedRequest.MessageType,
            };
            string requestString = JsonConvert.SerializeObject(approvePurchaseMethodkRequest);
            // execute the method
            MethodRequest methodRequest = new MethodRequest(expectedRequest.MethodName, Encoding.UTF8.GetBytes(requestString));
            MethodResponse methodresult = fakeDeviceClient.directMethods[0](methodRequest, null).Result;
            // check results
            Assert.AreEqual(methodresult.Status, 200); // got back an ok
        }

        [Test]
        public void TestLowStock()
        {
            TestContext.WriteLine(">> Testing the Kiosk Device's Low Stock Ticket..");

            // create our test device
            KioskDevice device = new KioskDevice(deviceconfig, fakeDeviceClient, fakeScheduler);

            TestContext.WriteLine(">> Purchasing tickets, shouldn't throw event");

            PurchaseTicketPayload approvePurchaseMethodkRequest = new PurchaseTicketPayload()
            {
                IsApproved = true,
                DeviceId = deviceconfig.DeviceId,
                DeviceType = deviceconfig.DeviceType,
                MessageType = "Purchase",
            };
            string requestString = JsonConvert.SerializeObject(approvePurchaseMethodkRequest);
            MethodRequest methodRequest = new MethodRequest("ReceivePurchaseTicketResponse", Encoding.UTF8.GetBytes(requestString));

            // fire events to bring the count down to right at threshold
            for (long count = this.deviceconfig.InitialStockCount; count > this.deviceconfig.LowStockThreshold; count--)
            {
                MethodResponse myresult = fakeDeviceClient.directMethods[0](methodRequest, null).Result;
            }

            // now that we're at the threshold, lets clear all previous events
            fakeDeviceClient.sendMessageLog.Clear(); // clear out all messages to this point

            TestContext.WriteLine(">> Purchasing 1 more ticket. Should send low stock notification");

            // purchase one more ticket. 
            MethodResponse methodresult = fakeDeviceClient.directMethods[0](methodRequest, null).Result;
            // we expect 2 messages to have been sent
            Assert.AreEqual(fakeDeviceClient.sendMessageLog.Count, 2);
            // second message should make expected result
            LowStockRequest expectedRequest = new LowStockRequest()
            {
                DeviceId = deviceconfig.DeviceId,
                DeviceType = deviceconfig.DeviceType,
                MessageType = "LowStock",
                StockLevel = (deviceconfig.LowStockThreshold-1),
            };
            // get actual message into an object so we can compare it
            LowStockRequest actualRequest = JsonConvert.DeserializeObject<LowStockRequest>(fakeDeviceClient.sendMessageLog[1]);
            TestContext.WriteLine(fakeDeviceClient.sendMessageLog[1]);
            // compare properties to make sure they're valid. 
            Assert.AreEqual(actualRequest.DeviceId, expectedRequest.DeviceId);
            Assert.AreEqual(actualRequest.DeviceType, expectedRequest.DeviceType);
            Assert.AreEqual(actualRequest.MessageType, expectedRequest.MessageType);
            Assert.AreEqual(actualRequest.StockLevel, expectedRequest.StockLevel);


        }
    }
}