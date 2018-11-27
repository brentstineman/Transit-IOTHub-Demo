using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Devices.Base.Interfaces;

namespace TransportationDemoTests
{
    public class FakeEventScheduler : IEventScheduler
    {
        public List<eventDelegate> EventList = new List<eventDelegate>();

        public void Add(ISimulatedEvent simulatedEvent)
        {
            // map inbound event to FakeEvent
            EventList.Add(simulatedEvent.getEventDelegate());
        }

        public void StartAll()
        {
            // do nothing, this is a test
        }

        public void StopAll()
        {
            // do nothing, this is a test
        }
    }
}
