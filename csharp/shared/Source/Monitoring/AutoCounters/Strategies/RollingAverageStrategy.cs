using System.Collections.Generic;
using System.Diagnostics;


namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// This class implements a new performance counter that actually tracks a rolling average
	/// of time spans.  This is needed because the standard average time counter in windows
	/// is instantaneous, and if the number of hits in a second drop to zero, the timer
	/// average zeros.  Instead, we want to keep hold of the last several timestamps and average those.
	/// </summary>
	internal class RollingAverageStrategy : ICounterStrategy
	{
		/// <summary>
		/// Lock to prevent concurrent modifications to the counter.
		/// </summary>
		private readonly object _lock;

		/// <summary>The counter for the strategy</summary>
		private readonly PerformanceCounter _counter;
		
		/// <summary>The base performance counter</summary>
		private readonly PerformanceCounter _base;
		
		/// <summary>The samples for the average</summary>
		private readonly Queue<long> _samples;
		
		/// <summary>The total number of milliseconds</summary>
		private long _totalMilliseconds;

		/// <summary>
		/// Divisor to convert a tick to milliseconds.
		/// </summary>
		private static readonly long _ticksToMillis = Stopwatch.Frequency / 1000;

		/// <summary>The number of samples to hold</summary>
		private const int _maxSampleSize = 50;

		/// <summary>The constant to undo the percentage inherent in the counter</summary>
		private const int _undoPercentage = 100;

		/// <summary>The number of milliseconds per second</summary>
		private const int _millisecondsPerSecond = 1000;

		/// <summary>
		/// Initializes the strategy
		/// </summary>
		/// <param name="counter">The initial initialized counter</param>
		public RollingAverageStrategy(PerformanceCounter counter)
		{
			_counter = counter;
			_lock = new object();
			_base = new PerformanceCounter(_counter.CategoryName,
										   _counter.CounterName + CounterStrategyFactory.BaseCounterSuffix,
										   _counter.InstanceName,
										   _counter.ReadOnly);

			_samples = new Queue<long>(_maxSampleSize);
			_totalMilliseconds = 0;
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
				_samples.Clear();
				_counter.RawValue = 0;
				_base.RawValue = 0;
				_totalMilliseconds = 0;	
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
				long current = Stopwatch.GetTimestamp();
				IncrementBy((current - block.StartTime) / _ticksToMillis);	
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
				while (_samples.Count >= _maxSampleSize)
				{
					_totalMilliseconds -= _samples.Dequeue();
				}
				_totalMilliseconds += count;
				_samples.Enqueue(count);

				_counter.RawValue = _totalMilliseconds;
				_base.RawValue = _samples.Count * _undoPercentage * _millisecondsPerSecond;
			}
		}


		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.  In the case of RollingAverageStrategy,
		/// this will clear the queue of all rolling averages and set it to one sample with the raw value given.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			lock(_lock)
			{
				// dump the current samples, enqueue the raw value as the only sample
				_samples.Clear();
				_samples.Enqueue(rawValue);

				// given the way this raw fraction counter works, we need to account for milliseconds and percentage
				_counter.RawValue = rawValue;
				_base.RawValue = 1 * _undoPercentage * _millisecondsPerSecond;	
			}
		}

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>The calculated next value</returns>
		public double NextValue()
		{
			lock(_lock)
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
				_base.Dispose();
				_samples.Clear();
			}
		}
	}
}