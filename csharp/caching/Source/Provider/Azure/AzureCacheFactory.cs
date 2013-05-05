using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.ApplicationServer.Caching;

namespace Cache.Provider.Azure
{
	/// <summary>
	/// A helper factory to build an azure cache instance
	/// </summary>
	internal static class AzureCacheFactory
	{
		/// <summary>
		/// Initializes a new instance of the cache factory
		/// </summary>
		/// <param name="servers">the servers to connect to</param>
		/// <returns>An initialized factory</returns>
		public static DataCacheFactory InitializeFactory(IEnumerable<Tuple<string, int>> servers)
		{
			var configuration = new DataCacheFactoryConfiguration
			{
				Servers = servers.Select(server => new DataCacheServerEndpoint(
					server.Item1, server.Item2)).ToList()
			};

			return new DataCacheFactory(configuration);
		}
	}
}
