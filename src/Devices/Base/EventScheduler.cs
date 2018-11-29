using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Devices.Base.Interfaces;

namespace Transportation.Demo.Devices.Base
{
    public class EventScheduler : IEventScheduler
    {
        private List<ISimulatedEvent> _EventList = new List<ISimulatedEvent>();

        public EventScheduler()
        {

        }

        public List<ISimulatedEvent> EventList
        {
            get
            {
                return _EventList;
            }
        }

        public void Add(ISimulatedEvent simulatedEvent)
        {
            _EventList.Add(simulatedEvent);
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
