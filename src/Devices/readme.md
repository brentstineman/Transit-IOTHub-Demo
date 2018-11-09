# Transit Demo - Devices
This readme explains the general organization and usage of the simulated devices created for this demo solution. We'll discuss the overall organization as well as the implementation and how to use the devices. 

This folder contains a solution file for Visual Studio that includes the following projects:

**Transportation.Demo.Shared**: contains shared methods used by various aspects of the entire solution. Including the models on which all events are based. 

**Transportation.Demo.Devices.Base**: A base class for simulated devices. Specific simulated devices will inherit from this class and provide their specific implementation details. 

**Transportation.Demo.Devices.GateReader**: The implementation of a simulated ticket reader and platform gate access device. Includes the device implementation as a class, and a console application to act as a host for the simulated device. 

**Transportation.Demo.Devices.Kiosk**: The implementation of a simulated kiosk for purchasing tickets. Includes the device implementation as a class, and a console application to act as a host for the simulated device. 

## Simulated Device Implementation
The implementation of a specific simulated device is done by inheriting from the [Transportation.Demo.Devices.Base.BaseDevice class](../Devices/Base/BaseDevice.cs). This base class implements some basic items such as an implementation of a [device client](../Devices/Base/TransportationDeviceClient.cs) for talking to the Azure IOT Event Hub. This base device also includes a collection of [SimulatedEvents](../Devices/Base/SimulatedEvent.cs) and methods of starting/stopping them. 

The implementation of our two simulated devices (the Kiosk and the GateReader) are done via specific classes that inherit from BaseDevice and are then hosted by a console application. 

The core of the implementation is a constructor that instantiates the various **SimulatedEvents** and add them to the internal collection. The construct also registers any device Direct Methods that will be called by Azure IOT Hub.

A SimulatedEvent is a class that contains a timer that will fire on a given interval +/- an amount of "jitter" to provide variation in the timer. Jitter is a non-negative double that represents the number of nanoseconds (1000 per second), and interval must be greater than or equal to the amount of jitter. An event will not fire until it has been started. The base device includes a method to start all events that have been added to its event collection. 

In both cases (SimulatedEvents and Direct Methods), the device implementation class defines the methods and that they do, and then registers them for the specific operation. 

By seperating the simulated device and its host into seperate classes, we believe this provide clear seperation of duties while also allowing the host to have multiple devices. This also helps encapsulate the device functionality into its own class, completely isolated from the host environment.

## The Simulated Kiosk Device
As previously mentioned, the Kiosk device is a simulation of a kiosk that would be used to purchase tickets. 

<< provide details on events and direct methods >>

## The Simulated GateReader Device
The GateReader device is the counterpart of the Kiosk. In the "real world" it would be responsible for scanning/reading the ticket, and opening the gate to allow passenger on and off of the transportation platform. 

<< provide details on events and direct methods >>

## Running the Simulated Devices
The GateReader and Kiosk devices are hosted in their own console applications. The console application is responsible for retrieving any configuration details needed by the devices, instantiating the devices, and then "starting" the device's simulated events. 

The console applications will retrieve configuration settings via a [ConfigurationHandler](../Devices/Base/ConfigurationHandler.cs) which will look for a appsettings.json file in the same location as their executable. A sample version of the file, called Sample.AppSettings.json, has been provided as a reference. 

To run the simulated device, copy this file and rename it appsettings.json, then set the properties of the file so that its "Copy to Output directory" value is "Copy always" or "Copy if newer". 

With the file in place, you can provide the outlined values as follows:

**IoTConnectionString**: Create a device twin in an Azure IOT Hub, then select that device and copy the primary or secondary connection string for it for us in this setting.