using System;

namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// Expiration strategy to determine if the current date/time minus the cached time is greater
    /// than the specified timespan.
    /// </summary>
    /// <typeparam name="TCacheItem">The type of item in the cache</typeparam>
    public class TimeSpanExpirationStrategy<TCacheItem> 
        : ICacheExpirationStrategy<TCacheItem> where TCacheItem : class
    {
        /// <summary>Time span to determine if an item is expired.</summary>
        private readonly TimeSpan _timeSpan;


        /// <summary>
        /// Create a TimeSpanExpirationStrategy with the specified timeSpan.
        /// </summary>
        /// <param name="timeSpan">The time span used to determine whether the item is expired.</param>
        public TimeSpanExpirationStrategy(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
        }

        /// <summary>
        /// Evaluates the cached item to determine if it is expired or not.
        /// </summary>
        /// <param name="cachedItem">The item to evaluate for expiration.</param>
        /// <returns>True if expired, false otherwise.</returns>
        public bool IsExpired(CachedItem<TCacheItem> cachedItem)
        {
			// if the number of ticks to cache is positive, check for duration from current item, otherwise a zero 
			// time means never expire.
        	return _timeSpan.Ticks > 0 
				? (DateTime.Now - cachedItem.Timestamp) > _timeSpan 
				: false;
        }
    }
}
