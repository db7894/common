using System.Diagnostics;


namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Strategy for counting the number of times an action has occurred.  Uses the underlying
	/// Windows NumberOfItems counter.
	/// </summary>
	internal class NumberOfItemsStrategy : ICounterStrategy
	{
		/// <summary>
		/// Lock to prevent concurrent access to the counter data.
		/// </summary>
		private readonly object _lock = new object();

		/// <summary>
		/// The counter instance in the strategy
		/// </summary>
		private readonly PerformanceCounter _counter;

		/// <summary>
		/// Initializes the number of items strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		public NumberOfItemsStrategy(PerformanceCounter counter)
		{
			_counter = counter;
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
				_counter.RawValue = 0;
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
				_counter.Increment();
			}
		}

		/// <summary>
		/// Manually increment a performance counter
		/// </summary>
		/// <param name="count">The amount to increment by</param>
		public void IncrementBy(long count)
		{
			lock(_lock)
			{
				_counter.IncrementBy(count);
			}
		}

		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.  In the case of NumberOfItems
		/// counter the raw value of the counter will be directly set to this value.
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
		/// Return the value of the current counter
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