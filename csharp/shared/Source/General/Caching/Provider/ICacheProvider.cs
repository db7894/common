using System;
using System.Collections.Generic;

namespace SharedAssemblies.General.Caching.Provider
{
	/// <summary>
	/// Interface to some implementation of a cache provider.
	/// </summary>
	/// <typeparam name="TKey">The cache key type</typeparam>
	/// <typeparam name="TValue">The cache value type</typeparam>
	public interface ICacheProvider<TKey, TValue>
		: IEnumerable<KeyValuePair<TKey, CachedValue<TValue>>>, IDisposable
	{
		/// <summary>
		/// Remove the specified element from the cache.
		/// </summary>
		/// <param name="key">The element to remove from the cache</param>
		bool Remove(TKey key);

		/// <summary>
		/// Remove the specified elements from the cache.
		/// </summary>
		/// <param name="keys">The elements to remove from the cache</param>
		bool Remove(IEnumerable<TKey> keys);

		/// <summary>
		/// Retrieve the value at the specified key
		/// </summary>
		/// <param name="key">The key to retrieve the value for</param>
		/// <returns>The cached value at the supplied key</returns>
		CachedValue<TValue> Get(TKey key);

		/// <summary>
		/// Retrieve the values at the specified keys
		/// </summary>
		/// <param name="keys">The keys to retrieve the value for</param>
		/// <returns>The cached values at the supplied keys</returns>
		IEnumerable<CachedValue<TValue>> Get(IEnumerable<TKey> keys);

		/// <summary>
		/// Set the current value for the specified key
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		/// <returns>true if the set was successful, false otherwise</returns>
		bool Add(TKey key, CachedValue<TValue> value);

		/// <summary>
		/// Set the current elements in the cache
		/// </summary>
		/// <param name="elements">The values to to the cache</param>
		/// <returns>true if the sets were successful, false otherwise</returns>
		bool Add(IEnumerable<KeyValuePair<TKey, CachedValue<TValue>>> elements);
	}
}
