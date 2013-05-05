using System;


namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Interface for a performance counter strategy
	/// </summary>
	internal interface ICounterStrategy : IDisposable
	{
		/// <summary>
		/// Whether or not the counter owned by this strategy must be flushed
		/// using the <see cref="FlushCounter"/> method.
		/// </summary>
		bool RequiresFlush { get; }

		/// <summary>
		/// Whether or not the counter requires the instrumentation block to
		/// store a start time.
		/// </summary>
		bool RequireStartTimer { get; }

		/// <summary>
		/// Flushes any cached data in the counter down to the PerformanceCounter objects
		/// owned by this strategy.
		/// </summary>
		void FlushCounter();

		/// <summary>
		/// Resets the current performance count
		/// </summary>
		void Reset();

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		void EndBlock(InstrumentationBlock block);

		/// <summary>
		/// Manually increment a performance counter
		/// </summary>
		/// <param name="count">The amount to increment by</param>
		void IncrementBy(long count);

		/// <summary>
		/// Manually set the counter strategy to a given value.  How this is performed is strategy dependent.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter(s) to.</param>
		void SetValue(long rawValue);

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The next calculated value</returns>
		double NextValue();
	}
}