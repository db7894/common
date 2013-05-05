using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.Redis;
using SharedAssemblies.General.Caching.Provider;

namespace Cache.Provider.Redis
{
	/// <summary>
	/// Implementation of the IObjectCacheProvider that backs up to a redis database
	/// </summary>
	/// <typeparam name="TValue">The cache value type</typeparam>
	public class RedisObjectCacheProvider<TValue> : IObjectCacheProvider<TValue>
	{
		private readonly PooledRedisClientManager _manager;
		private readonly string _key = Guid.NewGuid().ToString();

		/// <summary>
		/// Initializes a new instance of the RedisObjectCacheProvider class
		/// </summary>
		/// <param name="servers">the servers to connect to</param>
		public RedisObjectCacheProvider(IEnumerable<string> servers)
		{
			if ((servers == null) || (servers.Count() == 0))
			{
				throw new ArgumentException("servers");
			}
			_manager = new PooledRedisClientManager(servers.ToArray());
		}

		/// <summary>
		/// Get or set the underlying cache value
		/// </summary>
		/// <returns>The underlying held value</returns>
		public TValue Value
		{
			get
			{
				return _manager.ExecAs<TValue>(client => client.GetValue(_key));
			}
			set
			{
				var result = _manager.Exec(client => client.Set(_key, value));
			}
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
