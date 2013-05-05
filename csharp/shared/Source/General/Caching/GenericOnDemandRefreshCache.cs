using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// TODO
	/// </summary>
	/// <typeparam name="TKey">The type to use as a key</typeparam>
	/// <typeparam name="TValue">The type to use as a value</typeparam>
	public class GenericOnDemandRefreshCache<TKey, TValue> : ICache<TKey, TValue>
	{
		private readonly Func<TKey, TValue> _factory;
		private readonly IExpirationStrategy<TValue> _strategy;
		private readonly ConcurrentDictionary<TKey, CachedValue<TValue>> _dictionary
			= new ConcurrentDictionary<TKey,CachedValue<TValue>>();

		/// <summary>
		/// Returns the current count of values in the cache
		/// </summary>
		public int Count { get { return _dictionary.Count; } }

		/// <summary>
		/// Gets the current statistics for the cache
		/// </summary>
		public CacheStatistics Statistics { get; private set; }

		/// <summary>
		/// Retrieve the cached value at the specified key
		/// </summary>
		/// <param name="strategy">The strategy for determining if the value is expired</param>
		/// <param name="factory">The callback used to rebuild the backing value</param>
		public GenericOnDemandRefreshCache(IExpirationStrategy<TValue> strategy = null,
			Func<TKey, TValue> factory = null)
		{
			Statistics = new CacheStatistics();
			_factory = factory ?? ((key) => default(TValue));
			_strategy = strategy ?? Expires.Never<TValue>();
		}

		/// <summary>
		/// Retrieve the cached value at the specified key
		/// </summary>
		/// <param name="key">The key to retrieve the value for</param>
		/// <returns>The value at the supplied key</returns>
		public TValue this[TKey key] { get { return Get(key); } }

		/// <summary>
		/// Retrieve the cached value at the specified key
		/// </summary>
		/// <param name="key">The key to retrieve the value for</param>
		/// <returns>The value at the supplied key</returns>
		public TValue Get(TKey key)
		{
			Statistics.Requests.Increment();

			var statistic = Statistics.Hits;
			var resulting = _dictionary.GetOrAdd(key, (k) => {
				statistic = Statistics.Misses;
				return RegenerateValue(k);
			}); statistic.Increment();

			if (resulting.IsExpired)
			{
				Statistics.Updates.Increment();
				resulting = _dictionary.AddOrUpdate(key, RegenerateValue,
					(newkey, old) => RegenerateValue(newkey));
			}

			return resulting.Value;
		}

		/// <summary>
		/// Flushes every cached value from the cache
		/// </summary>
		/// <returns>The result of the operation</returns>
		public bool Clear()
		{
			_dictionary.Clear();
			Statistics.Cleanings.Increment();

			return true;
		}

		/// <summary>
		/// Cleans out any expired items in the cache
		/// </summary>
		/// <returns>The result of the operation</returns>
		public bool Clean()
		{
			CachedValue<TValue> dumpster;

			Statistics.Cleanings.Increment();
			return _dictionary.All(field => (field.Value.IsExpired)
				? _dictionary.TryRemove(field.Key, out dumpster) : true);
		}

		/// <summary>
		/// Disposes of all the held resources of the cache
		/// </summary>
		public void Dispose()
		{
			Clear();
		}

		/// <summary>
		/// Regenerate the cached value at the specified key
		/// </summary>
		/// <param name="key">The key to retrieve the value for</param>
		/// <returns>The value at the supplied key wrapped as a CachedItem</returns>
		private CachedValue<TValue> RegenerateValue(TKey key)
		{
			return new CachedValue<TValue>(_factory(key), _strategy);
		}

		/// <summary>
		/// Implementation of IEnumerable that allows one to add a value
		/// directly without the backing implementation.
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		public void Add(TKey key, TValue value)
		{
			Statistics.Updates.Increment();
			_dictionary.TryAdd(key, new CachedValue<TValue>(value, _strategy));
		}

		/// <summary>
		/// Implementation of IEnumerable that allows one to add a value
		/// directly without the backing implementation.
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="factory">The factory used to generate a new value</param>
		public void Add(TKey key, Func<TKey, TValue> factory)
		{
			Add(key, factory(key));
		}

		/// <summary>
		/// Implementation of IEnumerable that allows one to add a value
		/// directly without the backing implementation.
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		/// <param name="factory">The factory used to generate a new value</param>
		public void Add(TKey key, TValue value, Func<TKey, TValue> factory)
		{
			Add(key, value);
		}

		/// <summary>
		/// Returns an enumerator around the cache
		/// </summary>
		/// <returns>The cache enumerator</returns>
		public IEnumerator<KeyValuePair<TKey, CachedValue<TValue>>> GetEnumerator()
		{
			Statistics.Cleanings.Increment(); // TODO only used for cleaning now
			return _dictionary.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator around the cache
		/// </summary>
		/// <returns>The cache enumerator</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		/// <summary>
		/// Remove the value at the specified key
		/// </summary>
		/// <param name="key">The key to remove the value at</param>
		public bool Remove(TKey key)
		{
			CachedValue<TValue> output;

			Statistics.Evictions.Increment();
			return _dictionary.TryRemove(key, out output);
		}
	}

	/// <summary>
	/// Companion object to the SinglePullCache instance type
	/// </summary>
	public static class GenericOnDemandRefreshCache
	{
		/// <summary>
		/// A helper factory to simplify the creation of a single pull cache
		/// </summary>
		/// <typeparam name="TKey">The type to use as a key</typeparam>
		/// <typeparam name="TValue">The type to use as a value</typeparam>
		/// <param name="strategy">The expiration stategy to use</param>
		/// <param name="factory">The factory to create new values</param>		
		/// <returns>An initialized cache</returns>
		public static ICache<TKey, TValue> Create<TKey, TValue>(
			IExpirationStrategy<TValue> strategy, Func<TKey, TValue> factory)
		{
			return new GenericOnDemandRefreshCache<TKey, TValue>(strategy, factory);
		}
	}
}
