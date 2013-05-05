using System;

namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// An item to be held in a <see cref="Cache{TKey,TValue}" />. Each item contains
    /// a timestamp of when the cached item was added to the cache. If the cache is not
    /// created with a custom expiration predicate test, the item will never expire.
    /// </summary>
    /// <typeparam name="TValue">Type of item cache will store</typeparam>
    public sealed class CachedItem<TValue> where TValue : class
    {
        /// <summary>Expiration strategy for item</summary>
        private readonly ICacheExpirationStrategy<TValue> _expirationTest;

        /// <summary>True if item has already expired</summary>
        private bool _isExpired;

        /// <summary>
        /// Returns true if the item is expired, false otherwise.
        /// </summary>
        public bool IsExpired
        {
            get { return CheckExpiration(); }
        }

        /// <summary>
        /// The timestamp when the item was cached.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// The cached value.
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Creates an item for caching with the specified expiration predicate test and timestamp.
        /// </summary>
        /// <param name="value">The value to cache.</param>
        /// <param name="expirationTest">The expiration test predicate. The predicate should
        /// return true if the item is expired, false if the item is valid.</param>
        /// <param name="timestamp">The timestamp of the cached item.</param>
        public CachedItem(TValue value, ICacheExpirationStrategy<TValue> expirationTest, DateTime timestamp)
        {
            Value = value;
            _expirationTest = expirationTest ?? new CacheExpirationStrategy<TValue>(x => false);
            _isExpired = false;
            Timestamp = timestamp;
        }
        
        /// <summary>
        /// Helper method to check if the current item is expired
        /// </summary>
		/// <returns>True if current item has already expired or is now expired</returns>
        private bool CheckExpiration()
        {
            // if not already expired (manually or automatically), check expiration
            if (!_isExpired)
            {
                _isExpired = _expirationTest.IsExpired(this);
            }

            return _isExpired;
        }
	
		/// <summary>
		/// Expires the current item
		/// </summary>
		/// <param name="shouldReleaseReference">True if also should release the reference (set to null) so eligible for GC.</param>
		public void Expire(bool shouldReleaseReference)
		{
			_isExpired = true;

			if (shouldReleaseReference)
			{
				Value = null;
			}
		}
	}
}
