using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Devices.Base.Interfaces
{
    public interface ISimulatedEvent
    {
        eventDelegate EventDelegate
        {
            get;
        }

        void Start();
        void Stop();

        bool IsRunning
        {
            get;
        }
    }

    public delegate bool eventDelegate();

}
