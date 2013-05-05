namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// An empty counter strategy that does nothing except return -1 for current value.
	/// </summary>
	internal class StubCounterStrategy : ICounterStrategy
	{
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
		/// Performs application-defined tasks associated with freeing, releasing, or 
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// null strategy does nothing here
		}

		/// <summary>
		/// Resets the current performance count
		/// </summary>
		public void Reset()
		{
			// null strategy does nothing here
		}

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		public void EndBlock(InstrumentationBlock block)
		{
			// null strategy does nothing here
		}


		/// <summary>
		/// Manually increment a performance counter
		/// </summary>
		/// <param name="count">The amount to increment by</param>
		public void IncrementBy(long count)
		{
			// null strategy does nothing here
		}


		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
		}


		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The next calculated value</returns>
		public double NextValue()
		{
			// we don't want to return zero, because this could be a valid value and
			// it would be hard to determine whether it was the null strategy because
			// the counter doesn't exist or whether it was a good counter but zero value.
			return -1;
		}
	}
}
