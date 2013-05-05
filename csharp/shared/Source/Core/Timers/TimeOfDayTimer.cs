using System;
using System.Timers;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.Timers
{
	/// <summary>
	/// A timer that will fire at time of day each day.
	/// </summary>
	public class TimeOfDayTimer : IDisposable
	{
		private readonly object _timerLock = new object();
		private readonly Timer _timeOfDayTimer;
		private TimeSpan _timeOfDay;

		/// <summary>
		/// Gets the time of day the timer is set to trigger.
		/// </summary>
		public TimeSpan TimeOfDay
		{
			get { return _timeOfDay; }
			set { SetNextInterval(value, IsEnabled); }
		}

		/// <summary>
		/// Gets or sets whether the time of day timer is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get { return _timeOfDayTimer.Enabled; }
			set { SetNextInterval(TimeOfDay, value); }
		}

		/// <summary>
		/// Event that is triggered at time of day.
		/// </summary>
		public event ElapsedEventHandler Elapsed;

		/// <summary>
		/// Constructs a timer that will fire once it is time of day.  This timer is not enabled and does not
		/// have a default event handler, you must still attach to the Elapsed event and set Enabled to true.
		/// The default time of day is midnight if you do not specify otherwise.
		/// </summary>
		public TimeOfDayTimer() : this(TimeSpan.FromTicks(0))
		{
		}

		/// <summary>
		/// Constructs a timer that will fire once it is time of day.  This timer is not enabled and does not
		/// have a default event handler, you must still attach to the Elapsed event and set Enabled to true.
		/// </summary>
		/// <param name="timeOfDay">The time of day the timer should trigger at.</param>
		public TimeOfDayTimer(TimeSpan timeOfDay)
		{
			if (timeOfDay.Ticks < 0)
			{
				throw new ArgumentOutOfRangeException("timeOfDay");
			}

			_timeOfDayTimer = new Timer();
			_timeOfDayTimer.Elapsed += OnTimeOfDayTimerElapsed;
			_timeOfDayTimer.AutoReset = true;

			// set the time of day
			TimeOfDay = timeOfDay;
		}

		/// <summary>
		/// Disposes the MidnightTimer instance by stopping the timer.
		/// </summary>
		public void Dispose()
		{
			_timeOfDayTimer.Enabled = false;
		}

		/// <summary>
		/// If timer is being enabled, sets the timer interval to be the number of milliseconds from now to time of day,
		/// otherwise it disables the auto reset timer.
		/// </summary>
		/// <param name="timeOfDay">The time of day to trigger the event.</param>
		/// <param name="isEnabled">True or false whether timer should be enabled.</param>
		private void SetNextInterval(TimeSpan timeOfDay, bool isEnabled)
		{
			lock (_timerLock)
			{
				_timeOfDay = timeOfDay;

				// if we are turning timer on, calculate time difference from now to instant before time of day
				if (isEnabled)
				{
					_timeOfDayTimer.Interval = DateTime.Now.TimeUntil(_timeOfDay).TotalMilliseconds;
				}

				_timeOfDayTimer.Enabled = isEnabled;
			}
		}

		/// <summary>
		/// When the auto reset timer interval has elapsed, we should go ahead and reset all auto-counters and then set the timer
		/// again for time of day of next day.
		/// </summary>
		/// <param name="sender">Timer that generated the event.</param>
		/// <param name="e">Timer elapsed event arguments.</param>
		private void OnTimeOfDayTimerElapsed(object sender, ElapsedEventArgs e)
		{
			lock (_timerLock)
			{
				var handler = Elapsed;

				if (handler != null)
				{
					handler(this, e);
				}

				SetNextInterval(TimeOfDay, IsEnabled);
			}
		}
	}
}
