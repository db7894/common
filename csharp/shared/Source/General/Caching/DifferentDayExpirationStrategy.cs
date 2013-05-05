using System;

namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// Expiration strategy to determine if the cached item was cached on the same day. If it was, the 
    /// item is not expired. If it was not, the item is expired.
    /// </summary>
    /// <typeparam name="TCacheItem">The type of item in the cache.</typeparam>
    public class DifferentDayExpirationStrategy<TCacheItem> 
        : ICacheExpirationStrategy<TCacheItem> where TCacheItem : class
    {
        /// <summary>
        /// Evaluates the cached item to determine if it is expired or not.
        /// </summary>
        /// <param name="cachedItem">The item to evaluate for expiration.</param>
        /// <returns>True if expired, false otherwise.</returns>
        public bool IsExpired(CachedItem<TCacheItem> cachedItem)
        {
            DateTime now = DateTime.Now;
            DateTime ts = cachedItem.Timestamp;

            return now.Date != ts.Date;
        }
    }
}
