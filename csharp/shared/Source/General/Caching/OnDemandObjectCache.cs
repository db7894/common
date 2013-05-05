using System;
using SharedAssemblies.General.Caching.Provider;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// The on demand cache refreshes the value of the cache on the next cache.Value
	/// call after the underlying value has expired.
	/// </summary>
	/// <typeparam name="TValue">The type of the underlying cached value</typeparam>
	public class OnDemandObjectCache<TValue> : IObjectCache<TValue>
	{
		private CachedValue<TValue> _value;
		private readonly Func<TValue> _factory;
		private readonly IExpirationStrategy<TValue> _strategy;

		/// <summary>
		/// Gets the current statistics for the cache
		/// </summary>
		public CacheStatistics Statistics { get; private set; }

		/// <summary>
		/// Initializes a new instance of the OnDemandObjectCache class
		/// </summary>
		/// <param name="factory">The factory used to create a new value</param>
		/// <param name="strategy">The strategy to test for expiration</param>
		public OnDemandObjectCache(Func<TValue> factory, IExpirationStrategy<TValue> strategy)
		{
			Statistics = new CacheStatistics();
			_factory = factory;
			_strategy = strategy;
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
			_value = new CachedValue<TValue>(_factory(), _strategy);

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
				Statistics.Requests.Increment();
				if (_value.IsExpired)
				{
					Statistics.Misses.Increment();
					Refresh();
				}
				else { Statistics.Hits.Increment(); }
				return _value.Value;
			}
		}

		/// <summary>
		/// Dispose of all managed resources
		/// </summary>
		public void Dispose()
		{ }			
	}
}
