using System.Diagnostics;
using System.Threading;

namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// The strategy for calculating average time using the underlying windows performance counter
	/// method where you have one counter that increments the number of milliseconds and another that
	/// increments the number of times the action occurred.
	/// <para>
	/// This strategy does not use locking so the underlying performance counter value may not
	/// be up to date until the value has been flushed by a background process.
	/// </para>
	/// </summary>
	internal class NonLockingAverageTimeStrategy : ICounterStrategy
	{
		#region Private Members

		/// <summary>The main counter for average time</summary>
		private readonly PerformanceCounter _counter;

		/// <summary>The base counter</summary>
		private readonly PerformanceCounter _base;

		/// <summary>
		/// Sum of all sample times recorded.  This value must be atomically updated and read.
		/// </summary>
		private long _sampleTimes;

		/// <summary>
		/// The number of samples recorded. This value must be atomically updated and read.
		/// </summary>
		private long _sampleCount;

		#endregion

		/// <summary>
		/// Initializes the average timer strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		public NonLockingAverageTimeStrategy(PerformanceCounter counter)
		{
			_counter = counter;
			_base = new PerformanceCounter(_counter.CategoryName,
										   _counter.CounterName + CounterStrategyFactory.BaseCounterSuffix,
										   _counter.InstanceName, 
										   _counter.ReadOnly);      
		}

		/// <summary>
		/// Resets the current performance count
		/// </summary>
		public void Reset()
		{
			Interlocked.Exchange(ref _sampleTimes, 0);
			Interlocked.Exchange(ref _sampleCount, 0);
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
		public bool RequireStartTimer { get { return true; } }


		/// <summary>
		/// Flushes any cached data in the counter down to the PerformanceCounter objects
		/// owned by this strategy.
		/// </summary>
		public void FlushCounter()
		{
			_counter.RawValue = Interlocked.Read(ref _sampleTimes);
			_base.RawValue = Interlocked.Read(ref _sampleCount);
		}

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		public void EndBlock(InstrumentationBlock block)
		{
			long currentTime = Stopwatch.GetTimestamp();
			IncrementBy(currentTime - block.StartTime);
		}

		/// <summary>
		/// Manually increment a performance counter
		/// </summary>
		/// <param name="count">The amount to increment by</param>
		public void IncrementBy(long count)
		{
			// Add the total time of the sample to the sum of sample times.
			Interlocked.Add(ref _sampleTimes, count);

			// increment the sample count by one.
			Interlocked.Increment(ref _sampleCount);
		}

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The next calculated value</returns>
		public double NextValue()
		{
			long sampleTimeSum = Interlocked.Read(ref _sampleTimes);
			long sampleCount = Interlocked.Read(ref _sampleCount);

			double result = 0.0;
			if(sampleCount > 0)
			{
				// the average time is just the sum of sample times divided
				// by the count of samples recorded.  The value is then divided
				// by the stop watch frequency to give the average time in seconds.
				result = ((double)sampleTimeSum / sampleCount) / Stopwatch.Frequency;
			}
			return result;
		}

		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.  For AverageTime strategy this
		/// will be a the raw value given and a base value of one, yielding the raw value in the combined calculation.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			// for the average to be the supplied value.
			Interlocked.Exchange(ref _sampleTimes, rawValue);
			Interlocked.Exchange(ref _sampleCount, 1);
		}

		/// <summary>
		/// Disposes of performance counter handles
		/// </summary>
		public void Dispose()
		{
			_counter.Dispose();
			_base.Dispose();	
		}
	}
}
