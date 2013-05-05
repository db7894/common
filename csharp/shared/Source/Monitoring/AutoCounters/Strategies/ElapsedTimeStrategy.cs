using System.Diagnostics;


namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Strategy for determining the amount of time that has passed since the last increment.  
	/// Uses the underlying Windows ElapsedTime performance counter.
	/// </summary>
	internal class ElapsedTimeStrategy : ICounterStrategy
	{
		/// <summary>
		/// Lock to prevent concurrent access to the counter data.
		/// </summary>
		private readonly object _lock = new object();

		/// <summary>The counter for this strategy</summary>
		private readonly PerformanceCounter _counter;

		/// <summary>
		/// Initializes the number of items strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		/// <param name="isReadOnly">True if the strategy should be implemented read-only.</param>
		public ElapsedTimeStrategy(PerformanceCounter counter, bool isReadOnly)
		{
			_counter = counter;

			if(!isReadOnly)
			{
				_counter.RawValue = Stopwatch.GetTimestamp();
				_counter.NextValue();				
			}
		}


		/// <summary>
		/// Whether or not the counter owned by this strategy must be flushed
		/// using the <see cref="FlushCounter"/> method.
		/// </summary>
		public bool RequiresFlush { get { return false; } }	// counter is always up to date.

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
			// do nothing.
		}

		/// <summary>
		/// Resets the current performance count
		/// </summary>
		public void Reset()
		{
			lock(_lock)
			{
				// for an ElapsedTime counter, we set the raw value to be the 
				// current number of ticks as our starting value.
				_counter.RawValue = Stopwatch.GetTimestamp();
				_counter.NextValue();
			}
		}

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		public void EndBlock(InstrumentationBlock block)
		{
			lock(_lock)
			{
				// for an ElapsedTime counter, we again set the raw value to be the 
				// current number of ticks at each "increment" to mark the last time
				// we left the block.
				_counter.RawValue = Stopwatch.GetTimestamp();
				_counter.NextValue();
			}
		}


		/// <summary>
		/// Manually increment the ElapsedTime performance counter by the 
		/// number of ticks that have elapsed since last sample.
		/// </summary>
		/// <param name="count">The number of ticks to increment by</param>
		public void IncrementBy(long count)
		{
			lock(_lock)
			{
				// yes, you can do this if you wish, but you must increment by
				// the appropriate number of elapsed ticks.
				_counter.IncrementBy(count);
			}
		}


		/// <summary>
		/// Sets the raw value of the performance counter to the timestamp given.  This timestamp
		/// should be considered the same timestamp as typically returned by Stopwatch.GetTimestamp()
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			lock(_lock)
			{
				_counter.RawValue = rawValue;
			}
		}


		/// <summary>
		/// Return the value of the current counter which is the number of 
		/// seconds since the last update to the counter was made.
		/// </summary>
		/// <returns>The next sampled value</returns>
		public double NextValue()
		{
			lock (_lock)
			{
				return _counter.NextValue();	
			}
		}

		/// <summary>
		/// Disposes of performance counter handles
		/// </summary>
		public void Dispose()
		{
			lock(_lock)
			{
				_counter.Dispose();	
			}
		}
	}
}