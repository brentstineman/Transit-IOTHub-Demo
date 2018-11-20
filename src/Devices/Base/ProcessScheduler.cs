using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Transportation.Demo.Devices.Base
{
    public class ProcessScheduler
    {
        private Timer _timer;
        private int _interval;
        private int _jitter;
        private Random _rand;
        private Action _action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">The average time to wait between executions in seconds</param>
        /// <param name="jitter">The maximum amount of variance both positive and negative to adjust the interval by in seconds</param>
        public ProcessScheduler(Action action, int interval, int jitter)
        {
            
            _interval = interval;
            _jitter = jitter;
            _rand = new Random(DateTime.Now.Millisecond);
            _action = action;
            _timer = new Timer(Tick, null, CalculateNextTick(), TimeSpan.FromMilliseconds(-1));
        }

        private void Tick(object state)
        {
            _action();
            _timer.Change(CalculateNextTick(), TimeSpan.FromMilliseconds(-1));
        }

        private TimeSpan CalculateNextTick()
        {
            var tickJitter = _rand.Next(_jitter * -1000, _jitter * 1000);
            var tickInterval = _interval * 1000 + tickJitter;
            return TimeSpan.FromMilliseconds(tickInterval);
        }
    }
}
