using System;
using System.Collections.Generic;
using SharedAssemblies.General.Caching.Cleaning;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// A thread safe class to allow caching of items with custom expiration strategies. Currently, the 
	/// entire cache must share the same strategy.
	/// </summary>
	/// <typeparam name="TKey">The type to use as a key</typeparam>
	/// <typeparam name="TValue">The type to use as a value</typeparam>
	public interface ICache<TKey, TValue> : IDisposable, IEnumerable<KeyValuePair<TKey, CachedValue<TValue>>>
	{
		/// <summary>
		/// Gets a value from the referenced key in the cache
		/// </summary>
		/// <param name="key">The key to set the value at</param>
		/// <param name="value">The value to set at the specified key</param>
		TValue this[TKey key] { get; }

		/// <summary>
		/// Returns the current count of elements in the cache
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Removes all expired items from the cache
		/// </summary>
		/// <returns>The result of the operation</returns>
		bool Clean();

		/// <summary>
		/// Removes all items from the cache
		/// </summary>
		/// <returns>The result of the operation</returns>
		bool Clear();

		/// <summary>
		/// Gets a value from the referenced key in the cache
		/// </summary>
		/// <param name="key">The key to get the value for</param>
		/// <returns>The requested value in the cache</returns>
		TValue Get(TKey key);

		/// <summary>
		/// Adds a value to the cache at the specified key
		/// </summary>
		/// <param name="key">The key to set the value at</param>
		/// <param name="value">The value to set at the specified key</param>
		void Add(TKey key, TValue value);

		/// <summary>
		/// Adds a factory to the cache at the specified key
		/// </summary>
		/// <param name="key">The key to set the value at</param>
		/// <param name="factory">A factory that can generate a new value for the given key</param>
		void Add(TKey key, Func<TKey, TValue> factory);

		/// <summary>
		/// Adds a value to the cache at the specified key
		/// </summary>
		/// <param name="key">The key to set the value at</param>
		/// <param name="value">The value to set at the specified key</param>
		/// <param name="factory">A factory that can generate a new value for the given key</param>
		void Add(TKey key, TValue value, Func<TKey, TValue> factory);

		/// <summary>
		/// Remove the value at the specified key
		/// </summary>
		/// <param name="key">The key to remove the value at</param>
		bool Remove(TKey key);

		/// <summary>
		/// Gets the current statistics for the cache
		/// </summary>
		CacheStatistics Statistics { get; }

		//// <summary>
		//// Gets or sets the strategy used to expire elements in the cache
		///// </summary>
		//IExpirationStrategy<TValue> ExpirationStrategy { get; set; }
	}
}
