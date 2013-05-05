using System.Diagnostics;
using System.Threading;

namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Strategy for counting the number of times an action has occurred.  Uses the underlying
	/// Windows NumberOfItems counter.
	/// </summary>
	internal class NonLockingNumberOfItemsStrategy : ICounterStrategy
	{
		#region Private Members

		/// <summary>
		/// The counter instance in the strategy
		/// </summary>
		private readonly PerformanceCounter _counter;

		/// <summary>
		/// A local count variable independent of the counter for complete thread
		/// safety.   The counter itself is only safe for increment and decrement, not
		/// for setting the raw value or getting the raw value.
		/// </summary>
		private long _count;

		#endregion

		/// <summary>
		/// Initializes the number of items strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		public NonLockingNumberOfItemsStrategy(PerformanceCounter counter)
		{
			_counter = counter;
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
			_counter.RawValue = Interlocked.Read(ref _count);
		}

		/// <summary>
		/// Resets the current performance count
		/// </summary>
		public void Reset()
		{
			Interlocked.Exchange(ref _count, 0);
		}

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		public void EndBlock(InstrumentationBlock block)
		{
			Interlocked.Increment(ref _count);
		}

		/// <summary>
		/// Manually increment a performance counter
		/// </summary>
		/// <param name="count">The amount to increment by</param>
		public void IncrementBy(long count)
		{
			Interlocked.Add(ref _count, count);
		}

		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.  In the case of NumberOfItems
		/// counter the raw value of the counter will be directly set to this value.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			Interlocked.Exchange(ref _count, rawValue);
		}

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The next sampled value</returns>
		public double NextValue()
		{
			return Interlocked.Read(ref _count);
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
