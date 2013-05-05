using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SharedAssemblies.General.Caching;
using SharedAssemblies.General.Caching.Provider;
using Microsoft.ApplicationServer.Caching;

namespace Cache.Provider.Azure
{
	/// <summary>
	/// Implementation of the ICacheProvider that backs up to an azure cache
	/// </summary>
	/// <typeparam name="TKey">The cache key type</typeparam>
	/// <typeparam name="TValue">The cache value type</typeparam>
	public class AzureCacheProvider<TKey, TValue> : ICacheProvider<TKey, TValue>
	{
		private readonly DataCache _cache;
		private readonly DataCacheFactory _factory;
		private readonly ConcurrentDictionary<TKey, bool> _keys
			= new ConcurrentDictionary<TKey, bool>();

		/// <summary>
		/// Initializes a new instance of the AzureCacheProvider class
		/// </summary>
		/// <param name="servers">the servers to connect to</param>
		/// <param name="cache">The name of the cache to connect to</param>
		public AzureCacheProvider(IEnumerable<Tuple<string, int>> servers, string cache = null)
		{
			_factory = AzureCacheFactory.InitializeFactory(servers);
			_cache = string.IsNullOrEmpty(cache)
				? _factory.GetDefaultCache()
				: _factory.GetCache(cache);
		}

		/// <summary>
		/// Remove the specified element from the cache.
		/// </summary>
		/// <param name="key">The element to remove from the cache</param>
		public bool Remove(TKey key)
		{
			var result = false;
			_keys.TryRemove(key, out result);
			return _cache.Remove(key.ToString());
		}

		/// <summary>
		/// Remove the specified elements from the cache.
		/// </summary>
		/// <param name="keys">The elements to remove from the cache</param>
		public bool Remove(IEnumerable<TKey> keys)
		{
			return keys.All(Remove);
		}

		/// <summary>
		/// Retrieve the value at the specified key
		/// </summary>
		/// <param name="key">The key to retrieve the value for</param>
		/// <returns>The cached value at the supplied key</returns>
		public CachedValue<TValue> Get(TKey key)
		{
			return (CachedValue<TValue>)_cache.Get(key.ToString());
		}

		/// <summary>
		/// Retrieve the values at the specified keys
		/// </summary>
		/// <param name="keys">The keys to retrieve the value for</param>
		/// <returns>The cached values at the supplied keys</returns>
		public IEnumerable<CachedValue<TValue>> Get(IEnumerable<TKey> keys)
		{
			return keys.Select(Get).ToList();
		}

		/// <summary>
		/// Set the current value for the specified key
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		/// <returns>true if the set was successful, false otherwise</returns>
		public bool Add(TKey key, CachedValue<TValue> value)
		{
			var result = _keys.AddOrUpdate(key, true, (k, e) => true);
			var version = _cache.Put(key.ToString(), value);
			return (version != null);
		}

		/// <summary>
		/// Set the current elements in the cache
		/// </summary>
		/// <param name="elements">The values to to the cache</param>
		/// <returns>true if the sets were successful, false otherwise</returns>
		public bool Add(IEnumerable<KeyValuePair<TKey, CachedValue<TValue>>> elements)
		{
			return elements.All(element => Add(element.Key, element.Value));
		}

		/// <summary>
		/// Returns an iterator around all the values in the cache
		/// </summary>
		/// <returns>The iterator around all the cache values</returns>
		public IEnumerator<KeyValuePair<TKey, CachedValue<TValue>>> GetEnumerator()
		{
			foreach (var key in _keys)
			{
				yield return new KeyValuePair<TKey, CachedValue<TValue>>(
					(TKey)Convert.ChangeType(key, typeof(TKey)),
					(CachedValue<TValue>)_cache.Get(key.ToString()));
			}			
		}

		/// <summary>
		/// Returns an iterator around all the values in the cache
		/// </summary>
		/// <returns>The iterator around all the cache values</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Dispose of all managed resources
		/// </summary>
		public void Dispose()
		{
			_factory.Dispose();
		}
	}
}
