using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;
using SharedAssemblies.General.Caching;
using SharedAssemblies.General.Caching.Provider;

namespace Cache.Provider.Redis
{
	/// <summary>
	/// Implementation of the ICacheProvider that backs up to a redis database
	/// </summary>
	/// <typeparam name="TKey">The cache key type</typeparam>
	/// <typeparam name="TValue">The cache value type</typeparam>
	public class RedisCacheProvider<TKey, TValue> : ICacheProvider<TKey, TValue>
	{
		private readonly PooledRedisClientManager _manager;

		/// <summary>
		/// Initializes a new instance of the AzureCacheProvider class
		/// </summary>
		/// <param name="servers">the servers to connect to</param>
		public RedisCacheProvider(IEnumerable<string> servers)
		{
			if ((servers == null) || (servers.Count() == 0))
			{
				throw new ArgumentException("servers");
			}
			_manager = new PooledRedisClientManager(servers.ToArray());
		}

		/// <summary>
		/// Remove the specified element from the cache.
		/// </summary>
		/// <param name="key">The element to remove from the cache</param>
		public bool Remove(TKey key)
		{
			return _manager.Exec(client => client.Remove(key.ToString()));
		}

		/// <summary>
		/// Remove the specified elements from the cache.
		/// </summary>
		/// <param name="keys">The elements to remove from the cache</param>
		public bool Remove(IEnumerable<TKey> keys)
		{
			_manager.Exec(client => client.RemoveAll(keys.Cast<string>()));
			return true; // TODO
		}

		/// <summary>
		/// Retrieve the value at the specified key
		/// </summary>
		/// <param name="key">The key to retrieve the value for</param>
		/// <returns>The cached value at the supplied key</returns>
		public CachedValue<TValue> Get(TKey key)
		{
			return _manager.ExecAs<CachedValue<TValue>>(client =>
				client.GetValue(key.ToString()));
		}

		/// <summary>
		/// Retrieve the values at the specified keys
		/// </summary>
		/// <param name="keys">The keys to retrieve the value for</param>
		/// <returns>The cached values at the supplied keys</returns>
		public IEnumerable<CachedValue<TValue>> Get(IEnumerable<TKey> keys)
		{
			return _manager.ExecAs<CachedValue<TValue>>(client =>
				client.GetValues(keys.Cast<string>().ToList()));
		}

		/// <summary>
		/// Set the current value for the specified key
		/// </summary>
		/// <param name="key">The key to add the value at</param>
		/// <param name="value">The value to add at key</param>
		/// <returns>true if the set was successful, false otherwise</returns>
		public bool Add(TKey key, CachedValue<TValue> value)
		{
			return _manager.Exec(client => client.Set(key.ToString(), value));
		}

		/// <summary>
		/// Set the current elements in the cache
		/// </summary>
		/// <param name="elements">The values to to the cache</param>
		/// <returns>true if the sets were successful, false otherwise</returns>
		public bool Add(IEnumerable<KeyValuePair<TKey, CachedValue<TValue>>> elements)
		{
			var dictionary = elements.ToDictionary(el => el.Key.ToString(), el => el.Value);
			_manager.Exec(client => client.SetAll(dictionary));
			return true; // TODO
		}

		/// <summary>
		/// Returns an iterator around all the values in the cache
		/// </summary>
		/// <returns>The iterator around all the cache values</returns>
		public IEnumerator<KeyValuePair<TKey, CachedValue<TValue>>> GetEnumerator()
		{
			using (var client = _manager.GetClient())
			{
				foreach (var key in client.GetAllKeys())
				{
					yield return new KeyValuePair<TKey, CachedValue<TValue>>(
						(TKey)Convert.ChangeType(key, typeof(TKey)),
						client.Get<CachedValue<TValue>>(key));
				}
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
			_manager.Dispose();
		}
	}
}
