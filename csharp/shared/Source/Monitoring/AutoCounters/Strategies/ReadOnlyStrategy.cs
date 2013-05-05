namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Strategy that wraps around other strategies to guarantee that the underlying
	/// counters are never changed or modified by the application.
	/// </summary>
	internal class ReadOnlyStrategy : ICounterStrategy
	{
		/// <summary>
		/// Strategy being wrapped.
		/// </summary>
		private readonly ICounterStrategy _strategy;

		/// <summary>
		/// Constructor for the read only counter strategy. 
		/// </summary>
		/// <param name="strategy">Counter to wrap, must not be null</param>
		public ReadOnlyStrategy(ICounterStrategy strategy)
		{
			_strategy = strategy;
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
			// do nothing.
		}

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		public void EndBlock(InstrumentationBlock block)
		{
			// do nothing.
		}

		/// <summary>
		/// Manually increment a performance counter
		/// </summary>
		/// <param name="count">The amount to increment by</param>
		public void IncrementBy(long count)
		{
			// do nothing.
		}


		/// <summary>
		/// Manually set the counter strategy to a given value.  How this is performed is strategy dependent.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter(s) to.</param>
		public void SetValue(long rawValue)
		{
			// do nothing.
		}

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The next calculated value</returns>
		public double NextValue()
		{
			return _strategy.NextValue();
		}

		/// <summary>
		/// Disposes of performance counter handles
		/// </summary>
		public void Dispose()
		{
			// go ahead and dispose, this modifies the object but it's at the
			// end of it's life anyway.
			_strategy.Dispose();
		}
	}
}
