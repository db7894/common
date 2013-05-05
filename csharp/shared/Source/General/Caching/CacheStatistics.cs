using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// A collection of statistics about how the cache is performing
	/// </summary>
	public sealed class CacheStatistics
	{
		/// <summary>
		/// The creation time of the cache
		/// </summary>
		public readonly long CreatedTime = Stopwatch.GetTimestamp();

		/// <summary>
		/// Gets or sets how many cache requests have occurred.
		/// </summary>
		public readonly SafeLong Requests = new SafeLong();

		/// <summary>
		/// Gets or sets how many cache value updates occurred.
		/// </summary>
		public readonly SafeLong Updates = new SafeLong();

		/// <summary>
		/// Gets or sets how many cache hits have occurred.
		/// </summary>
		public readonly SafeLong Hits = new SafeLong();

		/// <summary>
		/// Gets or sets how many cache misses have occurred.
		/// </summary>
		public readonly SafeLong Misses = new SafeLong();

		/// <summary>
		/// Gets or sets how many cache element evictions have occurred.
		/// </summary>
		public readonly SafeLong Evictions = new SafeLong();

		/// <summary>
		/// Gets or sets how many cache cleanings have occurred.
		/// </summary>
		public readonly SafeLong Cleanings = new SafeLong();

		/// <summary>
		/// Returns the current rate of hitting an element in the cache
		/// </summary>
		/// <returns>The result of the operation</returns>
		public double HitRate()
		{
			return (Requests.Value == 0) ? 1.0 : Hits.Value / (double)Requests.Value;
		}

		/// <summary>
		/// Returns the current rate of missing an element in the cache
		/// </summary>
		/// <returns>The result of the operation</returns>
		public double MissRate()
		{
			return (Requests.Value == 0) ? 1.0 : Misses.Value / (double)Requests.Value;
		}

		/// <summary>
		/// Indicates how long the cache has been in existance
		/// </summary>
		public TimeSpan ExistedTime()
		{
			return new TimeSpan(Stopwatch.GetTimestamp() - CreatedTime);
		}

		/// <summary>
		/// A wrapper around a long value that allows us to safely increment it
		/// </summary>
		public class SafeLong
		{
			private long _value = 0;

			/// <summary>
			/// Returns the underlying value of the safe long
			/// </summary>
			public long Value { get { return _value; } }

			/// <summary>
			/// Increment the currently held value by one
			/// </summary>
			/// <returns>The previous value</returns>
			public long Increment()
			{
				return Interlocked.Increment(ref _value);
			}
		}
	}
}
