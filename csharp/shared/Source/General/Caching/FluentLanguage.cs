using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedAssemblies.General.Caching.Cleaning;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// A collection of extensions to make wiring the cache
	/// a bit prettier.
	/// </summary>
	public static class FluentLanguage
	{
		///// <summary>
		///// Register an expiration strategy with the specified cache
		///// </summary>
		///// <typeparam name="TKey">The type to use as a key</typeparam>
		///// <typeparam name="TValue">The type to use as a value</typeparam>
		///// <param name="cache">The cache to register a cleanup strategy for</param>
		///// <param name="factory">A factory used to create a strategy</param>
		///// <returns>The fluent language continuation</returns>
		//public static ICache<TKey, TValue> ExpireWith<TKey, TValue>(
		//    this ICache<TKey, TValue> cache,
		//    Func<ICache<TKey, TValue>, IExpirationStrategy<TValue>> factory)
		//{
		//    var strategy = factory(cache);
		//    cache.ExpirationStrategy = strategy;
		//    return cache;
		//}

		/// <summary>
		/// Registers a cleanup strategy for the specified cache
		/// </summary>
		/// <typeparam name="TKey">The type to use as a key</typeparam>
		/// <typeparam name="TValue">The type to use as a value</typeparam>
		/// <param name="cache">The cache to register a cleanup strategy for</param>
		/// <param name="factory">A factory used to create a strategy</param>
		/// <param name="options">Options used to control the cache cleanup</param>
		/// <returns>The fluent language continuation</returns>
		public static ICache<TKey, TValue> CleanWith<TKey, TValue>(
			this ICache<TKey, TValue> cache,
			Func<ICache<TKey, TValue>, ICleanupStrategy> factory, CleanupOptions options = null)
		{
			var strategy = factory(cache);
			strategy.Options = options ?? CleanupOptions.Default;
			CacheJanitor.Instance.Register(cache, strategy);
			return cache;
		}
	}
}    

