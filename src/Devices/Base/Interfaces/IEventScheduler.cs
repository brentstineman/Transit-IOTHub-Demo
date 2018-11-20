using System;
using System.Collections.Generic;
using System.Text;

namespace Transportation.Demo.Devices.Base.Interfaces
{
    public interface IEventScheduler
    {
        void Add(ISimulatedEvent simulatedEvent);

        void StartAll();

        void StopAll();

    }
}
