using System;
using System.Timers;

namespace Transportation.Demo.Devices.Base
{
    public class SimulatedEvent
    {
        private Timer eventTimer;
        private double interval;
        private double jitter; 
        // return true is ready to continue, otherwise return false
        public delegate bool eventDelegate();
        private long eventCount = 0;

        private eventDelegate timerfunction;

        // constructor
        public SimulatedEvent(double interval, double jitter, eventDelegate timerfunction)
        {
            // jitter must be a positive value
            if (jitter < 0)
                throw new ArgumentException("Jitter value must be >= 0");
            else
                this.jitter = jitter;

            // interval must be at greater than or equal to the jitter value
            if (interval < jitter)
                throw new ArgumentException("interval must be >= jitter value");
            else
                this.interval = interval;

            this.timerfunction = timerfunction;

            eventTimer = new Timer();
            eventTimer.AutoReset = false; // don't automatically rerun the timer
            eventTimer.Elapsed += OnTimedEvent;

        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            eventCount++; // increment count
            Console.WriteLine("The Elapsed event #{0} was raised at {1:HH:mm:ss.fff}", this.eventCount, e.SignalTime);

            // execute our event
            if (this.timerfunction()) // if the function returned "true", restart the timer. 
            {                          
                this.Start(); // restart the timer for our next run 
            }
        }

        // Starts the event's internal timer
        public void Start()
        {
            eventTimer.Interval = this.CalculateNextInterval(); // set new interval
            eventTimer.Start(); 
        }

        // Stops the event's internal timer
        public void Stop()
        {
            eventTimer.Stop();
        }

        private double CalculateJitter()
        { 
            Random random = new Random();
            // calculate number that is +/- jitter
            return (this.jitter - (random.NextDouble() * (this.jitter  * 2)));
        }

        private double CalculateNextInterval()
        { 
            return this.interval - this.CalculateJitter(); 
        }
        
    }
}