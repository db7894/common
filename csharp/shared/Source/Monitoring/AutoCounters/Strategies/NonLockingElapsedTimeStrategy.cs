using System.Diagnostics;
using System.Threading;

namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Strategy for determining the amount of time that has passed since the last increment.  
	/// Uses the underlying Windows ElapsedTime performance counter.
	/// </summary>
	internal class NonLockingElapsedTimeStrategy : ICounterStrategy
	{
		#region Private Members

		/// <summary>The counter for this strategy</summary>
		private readonly PerformanceCounter _counter;

		/// <summary>
		/// Whether or not the start time should be flushed to the
		/// elapsed time timer.
		/// </summary>
		private bool _requiresFlush;

		/// <summary>
		/// Baseline for the elapsed time.  This is the stopwatch timestamp of
		/// when the counter started.
		/// </summary>
		private long _startTime;

		#endregion
	
		/// <summary>
		/// Initializes the number of items strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		/// <param name="isReadOnly">True if the strategy should be implemented read-only.</param>
		public NonLockingElapsedTimeStrategy(PerformanceCounter counter, bool isReadOnly)
		{
			_counter = counter;
			_startTime = Stopwatch.GetTimestamp();

			if(!isReadOnly)
			{
				_counter.RawValue = _startTime;
				_counter.NextValue();				
			}
		}

		/// <summary>
		/// Whether or not the counter owned by this strategy must be flushed
		/// using the <see cref="FlushCounter"/> method.
		/// </summary>
		public bool RequiresFlush { get { return true; } }	// counter always must be flushed.

		/// <summary>
		/// Whether or not the counter requires the instrumentation block to
		/// store a start time.
		/// </summary>
		public bool RequireStartTimer { get { return false; } }

		/// <summary>
		/// Flushes any cached data in the counter down to the PerformanceCounter objects
		/// owned by this strategy.
		/// </summary>
		public void FlushCounter()
		{
			if(_requiresFlush)
			{
				// for an ElapsedTime counter, we set the raw value to be the 
				// current number of ticks as our starting value.
				_counter.RawValue = Interlocked.Read(ref _startTime);
				_counter.NextValue();
				_requiresFlush = false;  // we could still miss some Reset/SetValue calls, but
										// this should be close enough to thread safe.
			}
		}

		/// <summary>
		/// Resets the current performance count
		/// </summary>
		public void Reset()
		{
			// for an ElapsedTime counter, we set the raw value to be the 
			// current number of ticks as our starting value.
			Interlocked.Exchange(ref _startTime, Stopwatch.GetTimestamp());
			_requiresFlush = true;
		}

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		public void EndBlock(InstrumentationBlock block)
		{
			Reset(); // just moves the base of the timer back to the current time.
		}

		/// <summary>
		/// Manually increment the ElapsedTime performance counter by the 
		/// number of ticks that have elapsed since last sample.
		/// </summary>
		/// <param name="count">The number of ticks to increment by</param>
		public void IncrementBy(long count)
		{
			// yes, you can do this if you wish, but you must increment by
			// the appropriate number of elapsed ticks.
			Interlocked.Add(ref _startTime, count);
			_requiresFlush = true;	
		}


		/// <summary>
		/// Sets the raw value of the performance counter to the timestamp given.  This timestamp
		/// should be considered the same timestamp as typically returned by Stopwatch.GetTimestamp()
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			// for an ElapsedTime counter, we set the raw value to be the 
			// current number of ticks as our starting value.
			Interlocked.Exchange(ref _startTime, rawValue);
			_requiresFlush = true;
		}


		/// <summary>
		/// Return the value of the current counter which is the number of 
		/// seconds since the last update to the counter was made.
		/// </summary>
		/// <returns>The next sampled value</returns>
		public double NextValue()
		{
			// the elapsed time is the current time minus the start time (which gives the value in
			// ticks).   Dividing by the stopwatch frequency gives the time in seconds.
			return (double)(Stopwatch.GetTimestamp() - Interlocked.Read(ref _startTime)) / Stopwatch.Frequency;
		}

		/// <summary>
		/// Disposes of performance counter handles
		/// </summary>
		public void Dispose()
		{
			_counter.Dispose();	
		}
	}
}
