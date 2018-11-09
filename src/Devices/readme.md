# Transit Demo - Devices
This readme explains the general organization and usage of the simulated devices created for this demo solution. We'll discuss the overall organization as well as the implementation and how to use the devices. 

This folder contains a solution file for Visual Studio that includes the following projects:

**Transportation.Demo.Shared**: contains shared methods used by various aspects of the entire solution. Including the models on which all events are based. 

**Transportation.Demo.Devices.Base**: A base class for simulated devices. Specific simulated devices will inherit from this class and provide their specific implementation details. 

**Transportation.Demo.Devices.GateReader**: The implementation of a simulated ticket reader and platform gate access device. Includes the device implementation as a class, and a console application to act as a host for the simulated device. 

**Transportation.Demo.Devices.Kiosk**: The implementation of a simulated kiosk for purchasing tickets. Includes the device implementation as a class, and a console application to act as a host for the simulated device. 

## Simulated Device Implementation
The implementation of a specific simulated device is done by inheriting from the [Transportation.Demo.Devices.Base.BaseDevice class](../blob/BaseDevice/src/Devices/Base/BaseDevice.cs). 
