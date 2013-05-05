using System;
using System.Diagnostics;
using System.Threading;

namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// This class implements a new performance counter that actually tracks a rolling average
	/// of time spans.  This is needed because the standard average time counter in windows
	/// is instantaneous, and if the number of hits in a second drop to zero, the timer
	/// average zeros.  Instead, we want to keep hold of the last several timestamps and average those.
	/// </summary>
	internal class NonLockingRollingAverageStrategy : ICounterStrategy
	{
		#region Private Members

		/// <summary>The counter for the strategy</summary>
		private readonly PerformanceCounter _counter;
		
		/// <summary>The base performance counter</summary>
		private readonly PerformanceCounter _base;
		
		/// <summary>
		/// Maximum number of samples to include in the rolling average.
		/// </summary>
		private const int MaxSamples = 50;

		/// <summary>
		/// Record of sample times for the rolling average.  Contains up to MaxSamples 
		/// sample times in stopwatch ticks.
		/// </summary>
		private readonly long[] _sampleTimes;

		/// <summary>
		/// The number of samples recorded. This value must be atomically updated and read.
		/// This value always increases, this value is modded by MaxSamples for inserting
		/// new samples into the _sampleTimes list.
		/// </summary>
		private long _sampleCount;

		#endregion

		/// <summary>
		/// Initializes the strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		public NonLockingRollingAverageStrategy(PerformanceCounter counter)
		{
			_counter = counter;
			_base = new PerformanceCounter(_counter.CategoryName,
										   _counter.CounterName + CounterStrategyFactory.BaseCounterSuffix,
										   _counter.InstanceName,
										   _counter.ReadOnly);
			_sampleTimes = new long[MaxSamples];
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
			_counter.RawValue = GetRollingAverageTicks();

			// base is a percentage so we have to shift by 100 to end up with units in seconds.
			_base.RawValue = Stopwatch.Frequency * 100; 
		}

		/// <summary>
		/// Resets the current performance count
		/// </summary>
		public void Reset()
		{	
			// just set the sample count back to zero.
			Interlocked.Exchange(ref _sampleCount, 0);
		}

		/// <summary>
		/// Exit a block of code to be instrumented.
		/// </summary>
		/// <param name="block">Instrumentation block that wraps the start, end block calls</param>
		public void EndBlock(InstrumentationBlock block)
		{
			long diff = Stopwatch.GetTimestamp() - block.StartTime;
			IncrementBy(diff);
		}

		/// <summary>
		/// Manually increment a performance counter
		/// </summary>
		/// <param name="count">The amount to increment by</param>
		public void IncrementBy(long count)
		{
			// Increment the sample count.
			long nextSample = Interlocked.Increment(ref _sampleCount);
	
			// Assign the sample value to the next sample slot.
			Interlocked.Exchange(ref _sampleTimes[nextSample % MaxSamples], count);
		}


		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.  In the case of RollingAverageStrategy,
		/// this will clear the queue of all rolling averages and set it to one sample with the raw value given.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			// Reset to a single sample with the supplied value.
			Interlocked.Exchange(ref _sampleCount, 1);
			Interlocked.Exchange(ref _sampleTimes[0], rawValue);
		}

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The calculated next value</returns>
		public double NextValue()
		{
			// divide the rolling average ticks by the stop watch frequency to
			// get the average as seconds.
			return (double)GetRollingAverageTicks() / Stopwatch.Frequency;
		}

		/// <summary>
		/// Disposes of performance counter handles
		/// </summary>
		public void Dispose()
		{
			_counter.Dispose();
			_base.Dispose();
		}

		#region Private Methods

		/// <summary>
		/// Examines the current sample set and returns the rolling average values
		/// in stopwatch ticks.
		/// </summary>
		/// <returns>Rolling average as ticks.</returns>
		private long GetRollingAverageTicks()
		{
			long sampleCount = Interlocked.Read(ref _sampleCount);
			long result = 0;
			if (sampleCount > 0)
			{
				// may have less than MaxSamples.
				long sampleSize = Math.Min(sampleCount, MaxSamples);

				// don't use a foreach, avoid the iterator as it's possible this method
				// could be invoked many times.
				for(int i = 0; i < sampleSize; ++i)
				{
					result += _sampleTimes[i];
				}

				// divide by the number of samples, and the convert to seconds
				// by dividing by the frequency.
				result /= sampleSize;
			}
			return result;
		}

		#endregion

	}
}
