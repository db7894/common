using System;
using System.Threading;
using SharedAssemblies.General.Caching.Provider;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// The background cache refreshes the value of the cache on a background thread
	/// when the specified condition has been met.
	/// </summary>
	/// <typeparam name="TValue">The type of the underlying cached value</typeparam>
	public class BackgroundObjectCache<TValue> : IObjectCache<TValue>
	{
		private volatile CachedValue<TValue> _value;
		private readonly Func<TValue> _factory;
		private readonly TimeSpan _rate;
		private readonly Timer _timer;

		/// <summary>
		/// Gets the current statistics for the cache
		/// </summary>
		public CacheStatistics Statistics { get; private set; }

		/// <summary>
		/// Initializes a new instance of the BackgroundObjectCache class
		/// </summary>
		/// <param name="factory">The factory used to create a new value</param>
		/// <param name="rate">The rate at which the cache should be refreshed</param>
		public BackgroundObjectCache(Func<TValue> factory, TimeSpan rate)
		{
			Statistics = new CacheStatistics();
			_rate = rate;
			_factory = factory;
			_timer = new Timer((o) => Refresh(),
				null, (int)_rate.TotalMilliseconds, Timeout.Infinite);
			Refresh();
		}

		/// <summary>
		/// Force a refresh of the cached value
		/// </summary>
		/// <returns>The result of the operation</returns>
		public bool Refresh()
		{
			Statistics.Updates.Increment();
			// assignments of reference types are guaranteed to be atomic
			_value = new CachedValue<TValue>(_factory());
			_timer.Change((int)_rate.TotalMilliseconds, Timeout.Infinite);

			return true;
		}

		/// <summary>
		/// Retrieve the underlying cached value
		/// </summary>
		/// <returns>The underlying held value</returns>
		public TValue Value
		{
			// reads of reference types are guaranteed to be atomic
			get
			{
				Statistics.Hits.Increment();
				Statistics.Requests.Increment();
				return _value.Value;
			}
		}

		/// <summary>
		/// Dispose of all managed resources
		/// </summary>
		public void Dispose()
		{
			_timer.Dispose();
		}
	}
}
