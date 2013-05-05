using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SharedAssemblies.General.Caching.Provider
{
	/// <summary>
	/// Implementation of a cache provider that stores cached
	/// values in memory.
	/// </summary>
	/// <typeparam name="TKey">The type to use as a key</typeparam>
	/// <typeparam name="TValue">The type to use as a value</typeparam>
	public class MemoryCacheProvider<TKey, TValue> : ICacheProvider<TKey, TValue>
	{
		private readonly ConcurrentDictionary<TKey, CachedValue<TValue>> _cache =
			new ConcurrentDictionary<TKey, CachedValue<TValue>>();

		/// <summary>
		/// Remove the specified element from the cache.
		/// </summary>
		/// <param name="key">The element to remove from the cache</param>
		public bool Remove(TKey key)
		{
			CachedValue<TValue> result;
			return _cache.TryRemove(key, out result);
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
			CachedValue<TValue> result;

			return _cache.TryGetValue(key, out result)
				? result
				: CachedValue<TValue>.Expired;
		}

		/// <summary>
		/// Retrieve the values at the specified keys
		/// </summary>
		/// <param name="keys">The keys to retrieve the value for</param>
		/// <returns>The cached values at the supplied keys</returns>
		public IEnumerable<CachedValue<TValue>> Get(IEnumerable<TKey> keys)
		{
			return keys.Select(Get);
		}

		/// <summary>
		/// Set the current value for the specified key
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		/// <returns>true if the set was successful, false otherwise</returns>
		public bool Add(TKey key, CachedValue<TValue> value)
		{
			var result = _cache.AddOrUpdate(key, value, (k, v) => value);

			return true;
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
		/// Cleanup all the managed resources
		/// </summary>
		public void Dispose()
		{}

		/// <summary>
		/// Returns an enumerator around the provider
		/// </summary>
		/// <returns>The provider enumerator</returns>
		public IEnumerator<KeyValuePair<TKey, CachedValue<TValue>>> GetEnumerator()
		{
			return _cache.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator around the provider
		/// </summary>
		/// <returns>The provider enumerator</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
