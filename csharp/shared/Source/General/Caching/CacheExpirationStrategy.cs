using System;

namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// A wrapper class to create a cache expiration strategy directly from a predicate.
    /// </summary>
    /// <typeparam name="TCacheItem">The type of item in the cache.</typeparam>
    public class CacheExpirationStrategy<TCacheItem> : ICacheExpirationStrategy<TCacheItem> 
		where TCacheItem : class
    {
        /// <summary>The test to determine if the cached item is expired or not</summary>
        private readonly Predicate<CachedItem<TCacheItem>> _expirationTest;

        
        /// <summary>
        /// Constructs a CacheExpirationStrategy with the specified expiration test predicate.
        /// </summary>
        /// <param name="expirationTest">The expiration test predicate to apply to the cached item.</param>
        public CacheExpirationStrategy(Predicate<CachedItem<TCacheItem>> expirationTest)
        {
            _expirationTest = expirationTest;
        }


        /// <summary>
        /// Evaluates the cached item to determine if it is expired or not.
        /// </summary>
        /// <param name="cachedItem">The item to evaluate for expiration.</param>
        /// <returns>True if expired, false otherwise.</returns>
        public bool IsExpired(CachedItem<TCacheItem> cachedItem)
        {
            return _expirationTest(cachedItem);
        }
    }
}
