using System;

namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// A wrapper class to create a cache expiration strategy directly from a predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of item in the cache.</typeparam>
	public sealed class GenericExpirationStrategy<TItem> : IExpirationStrategy<TItem> 
    {
        /// <summary>The test to determine if the cached item is expired or not</summary>
        private readonly Predicate<CachedValue<TItem>> _strategy;
        
        /// <summary>
        /// Constructs a CacheExpirationStrategy with the specified expiration test predicate.
        /// </summary>
        /// <param name="strategy">The expiration test predicate to apply to the cached item.</param>
        public GenericExpirationStrategy(Predicate<CachedValue<TItem>> strategy)
        {
			if (strategy == null)
			{
				throw new ArgumentNullException("strategy");
			}
            _strategy = strategy;
        }

        /// <summary>
        /// Evaluates the cached item to determine if it is expired or not.
        /// </summary>
        /// <param name="item">The item to evaluate for expiration.</param>
        /// <returns>True if expired, false otherwise.</returns>
        public bool IsExpired(CachedValue<TItem> item)
        {
			return _strategy(item);
        }
    }	
}
