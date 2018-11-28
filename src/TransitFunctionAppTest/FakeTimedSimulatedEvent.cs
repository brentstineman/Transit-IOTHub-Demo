using System;
using System.Collections.Generic;
using System.Text;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Devices.Base.Interfaces;

namespace TransportationDemoTests
{
    public class FakeTimedSimulatedEvent : ISimulatedEvent
    {
        private eventDelegate eventCallback;
        private bool isRunning;

        public FakeTimedSimulatedEvent(ISimulatedEvent originalEvent)
        {
            this.eventCallback = originalEvent.EventDelegate;
        }

        public eventDelegate EventDelegate
        {
            get
            {
                return eventCallback;
            }
        }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }

        public void Start()
        {
            isRunning = true;
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}
