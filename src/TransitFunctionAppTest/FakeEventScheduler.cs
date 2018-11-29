using System;
using System.Collections.Generic;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Devices.Base.Interfaces;

namespace TransportationDemoTests
{
    public class FakeEventScheduler : IEventScheduler
    {
        public List<FakeTimedSimulatedEvent> EventList = new List<FakeTimedSimulatedEvent>();

        public void Add(ISimulatedEvent simulatedEvent)
        {
            // map inbound event to FakeEvent and add to the collection
            EventList.Add(new FakeTimedSimulatedEvent(simulatedEvent));
        }

        public void Start(int index)
        {
            EventList[index].Start();
        }

        public void StartAll()
        {
            foreach (var myevent in EventList)
            {
                myevent.Start();
            }
        }

        public void Stop(int index)
        {
            EventList[index].Stop();
        }

        public void StopAll()
        {
            foreach (var myevent in EventList)
            {
                myevent.Stop();
            }
        }
    }
}
