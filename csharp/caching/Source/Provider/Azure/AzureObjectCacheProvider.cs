using System;
using System.Collections.Generic;
using System.Linq;
using SharedAssemblies.General.Caching;
using SharedAssemblies.General.Caching.Provider;
using Microsoft.ApplicationServer.Caching;

namespace Cache.Provider.Azure
{
	/// <summary>
	/// Implementation of the IObjectCacheProvider that backs up to an azure cache
	/// </summary>
		/// <typeparam name="TValue">The cache value type</typeparam>
	public class AzureObjectCacheProvider<TValue> : IObjectCacheProvider<TValue>
	{
		private readonly DataCache _cache;
		private readonly DataCacheFactory _factory;
		private readonly string _key = Guid.NewGuid().ToString();

		/// <summary>
		/// Initializes a new instance of the AzureCacheProvider class
		/// </summary>
		/// <param name="servers">the servers to connect to</param>
		/// <param name="cache">The name of the cache to connect to</param>
		public AzureObjectCacheProvider(IEnumerable<Tuple<string, int>> servers, string cache = null)
		{
			_factory = AzureCacheFactory.InitializeFactory(servers);
			_cache = string.IsNullOrEmpty(cache)
				? _factory.GetDefaultCache()
				: _factory.GetCache(cache);
		}

		/// <summary>
		/// Get or set the underlying cache value
		/// </summary>
		/// <returns>The underlying held value</returns>
		public TValue Value
		{
			get
			{
				return (TValue)_cache.Get(_key);
			}
			set
			{
				var version = _cache.Put(_key, value);
			}
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
