using System;
using System.Timers;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.Timers
{
	/// <summary>
	/// A timer that will fire at midnight each night.
	/// </summary>
	public class MidnightTimer : IDisposable
	{
		private readonly object _timerLock = new object();
		private readonly Timer _midnightTimer;

		/// <summary>
		/// Gets or sets whether the midnight timer is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get { return _midnightTimer.Enabled; }
			set { SetMidnightInterval(value); }
		}

		/// <summary>
		/// Event that is triggered at midnight.
		/// </summary>
		public event ElapsedEventHandler Elapsed;

		/// <summary>
		/// Constructs a timer that will fire once it is midnight.  This timer is not enabled and does not
		/// have a default event handler, you must still attach to the Elapsed event and set Enabled to true.
		/// </summary>
		public MidnightTimer()
		{
			_midnightTimer = new Timer();
			_midnightTimer.Elapsed += OnMidnightTimerElapsed;
			_midnightTimer.AutoReset = true;
		}

		/// <summary>
		/// Disposes the MidnightTimer instance by stopping the timer.
		/// </summary>
		public void Dispose()
		{
			_midnightTimer.Enabled = false;
		}

		/// <summary>
		/// If timer is being enabled, sets the timer interval to be the number of milliseconds from now to midnight,
		/// otherwise it disables the auto reset timer.
		/// </summary>
		/// <param name="value">True or false whether timer should be enabled.</param>
		private void SetMidnightInterval(bool value)
		{
			lock (_timerLock)
			{
				// if we are turning timer on, calculate time difference from now to instant before midnight
				if (value)
				{
					_midnightTimer.Interval = DateTime.Now.TimeUntilMidnight().TotalMilliseconds;
				}

				_midnightTimer.Enabled = value;
			}
		}

		/// <summary>
		/// When the auto reset timer interval has elapsed, we should go ahead and reset all auto-counters and then set the timer
		/// again for midnight of next day.
		/// </summary>
		/// <param name="sender">Timer that generated the event.</param>
		/// <param name="e">Timer elapsed event arguments.</param>
		private void OnMidnightTimerElapsed(object sender, ElapsedEventArgs e)
		{
			lock (_timerLock)
			{
				var handler = Elapsed;

				if (handler != null)
				{
					handler(this, e);
				}

				SetMidnightInterval(_midnightTimer.Enabled);
			}
		}
	}
}
