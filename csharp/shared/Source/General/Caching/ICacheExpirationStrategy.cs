namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// Interface for cache item expiration strategy.
    /// </summary>
    /// <typeparam name="TCacheItem">The type of item in the cache.</typeparam>
    public interface ICacheExpirationStrategy<TCacheItem> where TCacheItem : class
    {
        /// <summary>
        /// Evaluates the cached item to determine if it is expired or not.
        /// </summary>
        /// <param name="cachedItem">The item to evaluate for expiration.</param>
        /// <returns>True if expired, false otherwise.</returns>
        bool IsExpired(CachedItem<TCacheItem> cachedItem);
    }
}
