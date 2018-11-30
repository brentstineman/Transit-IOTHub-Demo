using System;
using System.Collections.Generic;
using Transportation.Demo.Devices.Base;
using Transportation.Demo.Devices.Base.Interfaces;

namespace TransportationDemoTests
{
    /// <summary>
    /// This class is used by the unit tests. It disables the "timer" functionality so 
    /// we can test the various timer delegates directly. Thus allowing unit tests to not 
    /// be dependent on that functionality.
    /// </summary>
    public class FakeEventScheduler : EventScheduler
    {
        /// <summary>
        /// This overridding Add method replaces the "real" event with a fake one.
        /// The fake event doesn't have the timer so we can manually execute the 
        /// events to allow for testing of them without having to expose the timer
        /// methods publically and violating OO principles.
        /// </summary>
        /// <param name="simulatedEvent"></param>
        public override void Add(ISimulatedEvent simulatedEvent)
        {
            // map inbound event to FakeEvent and add to the collection
            EventList.Add(new FakeTimedSimulatedEvent(simulatedEvent));
        }
    }
}
