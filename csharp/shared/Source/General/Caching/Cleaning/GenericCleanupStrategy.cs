using System;
using System.Linq;
using System.Collections.Generic;
using SharedAssemblies.General.Caching.Provider;
using System.Threading;
using System.Threading.Tasks;

namespace SharedAssemblies.General.Caching.Cleaning
{
	/// <summary>
	/// Interface to a janitor which is used to periodically
	/// clean a cache.
	/// </summary>
	public sealed class GenericCleanupStrategy<TKey, TValue> : ICleanupStrategy
	{
		private readonly CleanupCallback _strategy;
		private readonly ICache<TKey, TValue> _provider; // TODO convert to provider

		/// <summary>
		/// The options controlling how this janitor works
		/// </summary>
		public CleanupOptions Options { get; set; }

		/// <summary>
		/// The callback sent to the janitor strategy to cleanup the cache
		/// </summary>
		/// <param name="cache">The cache collection</param>
		/// <returns>The keys to cleanup</returns>
		public delegate IEnumerable<TKey> CleanupCallback(
			IEnumerable<KeyValuePair<TKey, CachedValue<TValue>>> cache);

		/// <summary>
		/// Initialize a new instance of the GenericJanitor class
		/// </summary>
		/// <param name="provider">The cache provider to clean</param>
		/// <param name="strategy">The callback to clean with</param>		
		/// <param name="options">The options to control how the janitor works</param>
		public GenericCleanupStrategy(ICache<TKey, TValue> provider,
			CleanupCallback strategy, CleanupOptions options = null)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (strategy == null)
			{
				throw new ArgumentNullException("strategy");
			}

			_provider = provider;
			_strategy = strategy;
			Options = options ?? CleanupOptions.Default;
		}

		/// <summary>
		/// Performs the cleanup for the supplied janitor
		/// </summary>
		/// <returns>The result of the operation</returns>
		public bool PerformCleanup()
		{
			// using linq allows this to be lazy all the way through
			// without ever having to fully construct the resulting collections
			//
			// TODO pass the entire enumerable all the way through if we can
			// perform multiple commands at once
			return _strategy(_provider).All(_provider.Remove);
		}
	}
}
