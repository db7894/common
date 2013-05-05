using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// TODO
	/// </summary>
	/// <typeparam name="TKey">The type to use as a key</typeparam>
	/// <typeparam name="TValue">The type to use as a value</typeparam>
	public class SpecificBackgroundRefreshCache<TKey, TValue> : ICache<TKey, TValue>
	{
		private readonly Timer _timer;
		private readonly Func<TKey, TValue> _missing;
		private readonly IExpirationStrategy<TValue> _strategy;
		private readonly ConcurrentDictionary<TKey, ValueWrapper> _dictionary
			= new ConcurrentDictionary<TKey, ValueWrapper>();

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
		/// <param name="rate">The rate of the background refresh</param>
		/// <param name="strategy">The strategy for determining if the value is expired</param>		
		/// <param name="missing">The default callback to execute if a value is missing</param>
		public SpecificBackgroundRefreshCache(TimeSpan rate,
			IExpirationStrategy<TValue> strategy = null, Func<TKey, TValue> missing = null)
		{
			Statistics = new CacheStatistics();
			_missing = missing ?? ((key) => { throw new KeyNotFoundException(); });
			_strategy = strategy ?? Expires.Never<TValue>();
			_timer = new Timer((o) => BackgroundCleanup(), null, rate, rate);
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
				return RegenerateMissingValue(k);
			}); statistic.Increment();

			return resulting.Item.Value;
		}

		/// <summary>
		/// Callback to regenerate all the expired values in the cache
		/// </summary>
		private void BackgroundCleanup()
		{
			foreach (var entry in _dictionary)
			{
				if (entry.Value.Item.IsExpired)
				{
					Statistics.Updates.Increment();
					var result = _dictionary.AddOrUpdate(entry.Key, RegenerateMissingValue,
						(newKey, oldValue) => RegenerateValue(newKey, oldValue));
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private ValueWrapper RegenerateMissingValue(TKey key)
		{
			return new ValueWrapper
			{
				Item = new CachedValue<TValue>(_missing(key), _strategy),
				Factory = _missing,
			};
		}

		/// <summary>
		/// Regenerate the cached value at the specified key
		/// </summary>
		/// <param name="key">The key to retrieve the value for</param>
		/// <param name="oldValue">The previous value to construct the new one with</param>
		/// <returns>The value at the supplied key wrapped as a CachedItem</returns>
		private ValueWrapper RegenerateValue(TKey key, ValueWrapper oldValue)
		{
			return new ValueWrapper
			{
				Item = new CachedValue<TValue>(oldValue.Factory(key), _strategy),
				Factory = oldValue.Factory,
			};
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
			ValueWrapper dumpster;

			Statistics.Cleanings.Increment();
			return _dictionary.All(field => (field.Value.Item.IsExpired)
				? _dictionary.TryRemove(field.Key, out dumpster) : true);
		}

		/// <summary>
		/// Disposes of all the held resources of the cache
		/// </summary>
		public void Dispose()
		{
			_timer.Dispose();
			Clear();
		}
		
		/// <summary>
		/// Implementation of IEnumerable that allows one to add a value
		/// directly without the backing implementation.
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		public void Add(TKey key, TValue value)
		{
			Add(key, value, null);
		}

		/// <summary>
		/// Implementation of IEnumerable that allows one to add a value
		/// directly without the backing implementation.
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="factory">The callback used to supply a new value</param>
		public void Add(TKey key, Func<TKey, TValue> factory = null)
		{
			Statistics.Updates.Increment();
			_dictionary.TryAdd(key, new ValueWrapper
			{
				Item = new CachedValue<TValue>(default(TValue), _strategy, true),
				Factory = factory ?? _missing,
			});
		}

		/// <summary>
		/// Implementation of IEnumerable that allows one to add a value
		/// directly without the backing implementation.
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		/// <param name="factory">The callback used to supply a new value</param>
		public void Add(TKey key, TValue value=default(TValue),
			Func<TKey, TValue> factory = null)
		{
			Statistics.Updates.Increment();
			_dictionary.TryAdd(key, new ValueWrapper
			{
				Item = new CachedValue<TValue>(value, _strategy),
				Factory = factory ?? _missing,
			});
		}

		/// <summary>
		/// Returns an enumerator around the cache
		/// </summary>
		/// <returns>The cache enumerator</returns>
		public IEnumerator<KeyValuePair<TKey, CachedValue<TValue>>> GetEnumerator()
		{
			Statistics.Cleanings.Increment(); // TODO only used for cleaning now
			return _dictionary.Select(value => new KeyValuePair<TKey, CachedValue<TValue>>(
				value.Key, value.Value.Item)).GetEnumerator();
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
			ValueWrapper output;

			Statistics.Evictions.Increment();
			return _dictionary.TryRemove(key, out output);
		}

		/// <summary>
		/// An internal type used to wrap a cached item and its factory
		/// </summary>
		internal class ValueWrapper
		{
			public CachedValue<TValue> Item { get; set; }
			public Func<TKey, TValue> Factory { get; set; }
		}
	}
	
	/// <summary>
	/// Companion object to the SpecificBackgroundRefreshCache instance type
	/// </summary>
	public static class SpecificBackgroundRefreshCache
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey">The type to use as a key</typeparam>
		/// <typeparam name="TValue">The type to use as a value</typeparam>
		/// <param name="rate">The rate of the background refresh</param>
		/// <param name="strategy">The strategy for determining if the value is expired</param>		
		/// <param name="missing">The default callback to execute if a value is missing</param>
		/// <returns>An initialized cache</returns>
		public static SpecificBackgroundRefreshCache<TKey, TValue> Create<TKey, TValue>(
			TimeSpan rate, IExpirationStrategy<TValue> strategy, Func<TKey, TValue> missing = null)
		{
			return new SpecificBackgroundRefreshCache<TKey, TValue>(rate, strategy, missing);
		}
	}
}
