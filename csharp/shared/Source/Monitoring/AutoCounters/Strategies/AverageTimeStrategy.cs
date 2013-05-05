using System.Diagnostics;


namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// The strategy for calculating average time using the underlying windows performance counter
	/// method where you have one counter that increments the number of milliseconds and another that
	/// increments the number of times the action occurred.
	/// </summary>
	internal class AverageTimeStrategy : ICounterStrategy
	{
		/// <summary>
		/// Lock to prevent concurrent access to the counter data.
		/// </summary>
		private readonly object _lock = new object();

		/// <summary>The main counter for average time</summary>
		private readonly PerformanceCounter _counter;

		/// <summary>The base counter</summary>
		private readonly PerformanceCounter _base;

		/// <summary>
		/// Initializes the average timer strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		public AverageTimeStrategy(PerformanceCounter counter)
		{
			_counter = counter;
			_base = new PerformanceCounter(_counter.CategoryName,
										   _counter.CounterName + CounterStrategyFactory.BaseCounterSuffix,
										   _counter.InstanceName, 
										   _counter.ReadOnly);      
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
		public bool RequireStartTimer { get { return true; } }


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
				_base.RawValue = 0;
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
				IncrementBy(Stopwatch.GetTimestamp() - block.StartTime);
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
				_base.Increment();
			}
		}

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The next calculated value</returns>
		public double NextValue()
		{
			lock(_lock)
			{
				return _counter.NextValue();	
			}
		}

		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.  For AverageTime strategy this
		/// will be a the raw value given and a base value of one, yielding the raw value in the combined calculation.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			lock(_lock)
			{
				_counter.RawValue = rawValue;
				_base.RawValue = 1;
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
				_base.Dispose();	
			}
		}
	}
}