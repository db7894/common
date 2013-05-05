using System;
using System.Threading;
using System.Diagnostics;

namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// Represents an item in a cache. Each item contains
    /// a timestamp of when the cached item was added to the cache. If the cache is not
    /// created with a custom expiration predicate test, the item will never expire.
    /// </summary>
    /// <typeparam name="TValue">Type of item cache will store</typeparam>
    public sealed class CachedValue<TValue>
    {
		private long _hits = 0L;
		private bool _isExpired;
		private TValue _value;
		private readonly IExpirationStrategy<TValue> _strategy;

		/// <summary>
		/// A helper type that can be used to return a default not found value
		/// </summary>
		public static readonly CachedValue<TValue> Expired =
			new CachedValue<TValue>(default(TValue), Expires.Always<TValue>(), true);
		
		/// <summary>
		/// A count of how many times this item as been hit
		/// </summary>
		public long Hits { get { return _hits; } }

        /// <summary>
        /// The timestamp when the item was cached.
        /// </summary>
        public long Created { get; private set; }

		/// <summary>
		/// The last time this value was touched
		/// </summary>
		public long LastTouched { get; private set; }

        /// <summary>
        /// The cached value.
        /// </summary>
        public TValue Value
		{
			get
			{

				Interlocked.Increment(ref _hits);
				LastTouched = Stopwatch.GetTimestamp();
				return _value;
			}
		}

		/// <summary>
		/// Returns true if the item is expired, false otherwise.
		/// </summary>
		public bool IsExpired
		{
			get
			{
				if (!_isExpired)
				{
					_isExpired = _strategy.IsExpired(this);
				}
				return _isExpired;
			}
		}

        /// <summary>
        /// Creates an item for caching with the specified expiration predicate test and timestamp.
        /// </summary>
        /// <param name="value">The value to cache.</param>
        /// <param name="strategy">The expiration test predicate</param>
		/// <param name="expired">A flag to initialize a value to expired</param>
        public CachedValue(TValue value, IExpirationStrategy<TValue> strategy = null,
			bool expired = false)
        {

			LastTouched = Created = Stopwatch.GetTimestamp();
			_value = value;
			_strategy = strategy ?? Expires.Never<TValue>();
			_isExpired = expired;
        }
	}
}
