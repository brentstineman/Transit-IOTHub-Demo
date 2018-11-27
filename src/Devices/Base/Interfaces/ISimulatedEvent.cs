using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Devices.Base.Interfaces
{
    public interface ISimulatedEvent
    {
        eventDelegate getEventDelegate();

        void Start();
        void Stop();
    }

    public delegate bool eventDelegate();

    public interface ISimulatedEventWithSetter : ISimulatedEvent
    {
        void SetCallback(eventDelegate callback);
    }
}
