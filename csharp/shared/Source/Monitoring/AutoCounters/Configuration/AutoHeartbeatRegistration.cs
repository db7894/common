using System;
using System.Timers;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// A registration for an automatically updating counter
    /// </summary>
    internal class AutoHeartbeatRegistration : IDisposable
    {
        /// <summary>
        /// The counter to auto-increment at each interval
        /// </summary>
        public ICounterRegistration Counter { get; private set; }


        /// <summary>
        /// The timer to manage the heartbeats
        /// </summary>
        public Timer Timer { get; private set; }


        /// <summary>
        /// Create the registration for the heartbeat with the counter to auto-increment
        /// and the interval to increment at.
        /// </summary>
        /// <param name="counter">Counter to auto-increment.</param>
        /// <param name="interval">Interval to increment at.</param>
        public AutoHeartbeatRegistration(ICounterRegistration counter, long interval)
        {
			if (counter == null)
			{
				throw new ArgumentNullException("counter");
			}

			if (interval <= 0)
			{
				throw new ArgumentOutOfRangeException("interval", "The interval must be a positive number.");
			}

            Counter = counter;

            Timer = new Timer(interval);
            Timer.Elapsed += OnTimerElapsed;
        }


        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            Timer.Enabled = true;
        }


        /// <summary>
        /// Halt the timer
        /// </summary>
        public void Stop()
        {
            Timer.Enabled = false;
        }


        /// <summary>
        /// Halt the timer and remove the event handler
        /// </summary>
        public void Dispose()
        {
            Timer.Enabled = false;
            Timer.Elapsed -= OnTimerElapsed;
        }

        
        /// <summary>
        /// Helper method to handle a timer elapsed event.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The arguments passed from the event sender</param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // never want a heartbeat to throw and should never be multi-instance...
            Counter.GetCounter().EndBlock();
        }
    }
}
