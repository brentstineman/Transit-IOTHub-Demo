﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Transportation.Demo.Base.Interfaces
{
    // Interface for sending & receiving messages to/from devices
    public interface IDeviceClient
    {
        Task SendMessageAsync(string msg);
        Task SendMessageBatchAsync(IEnumerable<string> msgs);
        Task<Message> ReceiveMessageAsync();
        Task RegisterDirectMethodAsync(MethodCallback methodHandler);
    }
}
