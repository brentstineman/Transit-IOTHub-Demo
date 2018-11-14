using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Devices.Base.Interfaces;

namespace TransportationDemoTests
{
    // A simulated events for testing that starts one event, and logs the calls.
    class TestSimulatedEvent : ISimulatedEventWithSetter
    {
        eventDelegate callback_;
        public void SetCallback(eventDelegate callback)
        {
            this.callback_ = callback;
        }

        public bool started = false;
        public List<bool> callbackLog = new List<bool>();
        public void Start()
        {
            started = true;
            callbackLog.Add(callback_());
        }

        public void Stop()
        {
            started = false;
        }
    }
}
