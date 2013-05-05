using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// A thread safe class to allow caching of items with custom expiration strategies. Currently, the 
	/// entire cache must share the same strategy.
	/// </summary>
	/// <typeparam name="TKey">The type to use as a key.</typeparam>
	/// <typeparam name="TValue">The type to use as a value. Must be a class.</typeparam>
	public sealed class Cache<TKey, TValue> : IDisposable 
		where TKey : IEquatable<TKey> 
		where TValue : class
	{
		/// <summary>The strategy to use to expire items from the cache.</summary>
		private readonly ICacheExpirationStrategy<TValue> _cacheExpirationTest;

		/// <summary>The dictionary to store the cache.</summary>
		private readonly ConcurrentDictionary<TKey, CachedItem<TValue>> _dictionary;  

		/// <summary>The timer to keep track of automatic cleanups.</summary>
		private readonly System.Timers.Timer _cleanUpTimer;

		/// <summary>Constant to represent cleanup span of 1 hour.</summary>
		private static readonly TimeSpan _hourlyCleanup = TimeSpan.FromHours(1.0);

		/// <summary>Constant to represent cleanup timespan of never.</summary>
		private static readonly TimeSpan _neverCleanup = new TimeSpan(0);

		/// <summary>Number of cache hits.</summary>
		private long _hits = 0;

		/// <summary>Number of cache misses.</summary>
		private long _misses = 0;

		/// <summary>Number of cleanup sweeps.</summary>
		private long _cleanups = 0;

		/// <summary>The amount of time the last cleanup took. </summary>
		private volatile int _lastCleanupDurationInMs;

		/// <summary>True if the cache has been disposed.</summary>
		private bool _isDisposed;


		/// <summary>
		/// Gets the number of items in the cache, expired or valid.
		/// </summary>
		public int Count
		{
			get { return _dictionary.Count; }
		}


		/// <summary>
		/// Gets the number of cache hits.
		/// </summary>
		public long Hits
		{
			get { return _hits; }
		}


		/// <summary>
		/// Gets the number of cache misses.
		/// </summary>
		public long Misses
		{
			get { return _misses; }
		}


		/// <summary>
		/// Gets the number of cleanup sweeps;
		/// </summary>
		public long Cleanups
		{
			get { return _cleanups; }
		}


		/// <summary>
		/// Gets the duration of the last cleanup sweep.
		/// </summary>
		public TimeSpan LastCleanupDuration
		{
			get { return TimeSpan.FromMilliseconds(_lastCleanupDurationInMs); }
		}


		/// <summary>
		/// Gets whether the cache has already been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return _isDisposed; }
		}


		/// <summary>
		/// Create a new Cache with no expiration strategy and defaults to 1 hour clean up intervals.
		/// </summary>
		public Cache()
			: this(new CacheExpirationStrategy<TValue>(x => false), _neverCleanup)
		{
		}


		/// <summary>
		/// Creates a new Cache with the specified expiration predicate and defaults 
		/// to 1 hour clean up intervals
		/// </summary>
		/// <param name="expiredTest">The expiration test</param>
		public Cache(Predicate<CachedItem<TValue>> expiredTest)
			: this(new CacheExpirationStrategy<TValue>(expiredTest), _hourlyCleanup)
		{
		}


		/// <summary>
		/// Creates a new Cache with the specified expiration predicate and 
		/// with a specified cleanup interval.
		/// </summary>
		/// <param name="expiredTest">The expiration test</param>
		/// <param name="cleanupInterval">The duration of time between cleans</param>
		/// <param name="comparer">The comparer to compare keys</param>
		public Cache(Predicate<CachedItem<TValue>> expiredTest,
					 TimeSpan cleanupInterval,
					 IEqualityComparer<TKey> comparer = null)
			: this(new CacheExpirationStrategy<TValue>(expiredTest), cleanupInterval, comparer)
		{
		}


		/// <summary>
		/// Creates a new Cache with the specified expiration strategy and a cleanup
		/// time of hour.
		/// </summary>
		/// <param name="expirationStrategy">The expiration strategy</param>
		public Cache(ICacheExpirationStrategy<TValue> expirationStrategy)
			: this(expirationStrategy, _hourlyCleanup)
		{
		}


		/// <summary>
		/// Creates a new Cache with the specified expiration TimeSpan and hourly cleanup.
		/// </summary>
		/// <param name="cacheInterval">The interval to expire items</param>
		public Cache(TimeSpan cacheInterval)
			: this(new TimeSpanExpirationStrategy<TValue>(cacheInterval), _hourlyCleanup)
		{
		}


		/// <summary>
		/// Creates a new Cache with the specified expiration TimeSpan and cleanup TimeSpan.
		/// </summary>
		/// <param name="cacheInterval">The interval to expire items</param>
		/// <param name="cleanupInterval">The duration of time between cleans</param>
		public Cache(TimeSpan cacheInterval, TimeSpan cleanupInterval)
			: this(new TimeSpanExpirationStrategy<TValue>(cacheInterval), cleanupInterval)
		{
		}


		/// <summary>
		/// Creates a new Cache with the specified expiration strategy and cleanup interval.  This version
		/// lets you specify a comparer.  If you pass null, the default comparer for TKey is used.
		/// </summary>
		/// <param name="expirationStrategy">The expiration strategy</param>
		/// <param name="cleanupInterval">The duration of time between cleans</param>
		/// <param name="comparer">The comparer to compare keys</param>
		public Cache(ICacheExpirationStrategy<TValue> expirationStrategy,
					 TimeSpan cleanupInterval,
					 IEqualityComparer<TKey> comparer = null)
		{
			_dictionary = comparer == null ? new ConcurrentDictionary<TKey, CachedItem<TValue>>() :
											 new ConcurrentDictionary<TKey, CachedItem<TValue>>(comparer);

			_cacheExpirationTest = expirationStrategy ?? new CacheExpirationStrategy<TValue>(x => false);

			if (cleanupInterval.TotalMilliseconds > 0)
			{
				_cleanUpTimer = new System.Timers.Timer(cleanupInterval.TotalMilliseconds);
				_cleanUpTimer.Elapsed += OnTimedEvent;
				_cleanUpTimer.Enabled = true;
			}
		}


		/// <summary>
		/// Add an item to the cache with <see cref="DateTime.Now" /> as the timestamp.
		/// </summary>
		/// <param name="key">The key to reference the cached value.</param>
		/// <param name="value">The cached value.</param>
		public void Add(TKey key, TValue value)
		{
			Add(key, value, DateTime.Now);
		}


		/// <summary>
		/// Adds a list of values to the cache using the corresponding keys 
		/// with <see cref="DateTime.Now" /> as the timestamp.
		/// </summary>
		/// <param name="values">The <see cref="KeyValuePair{TKey, TValue}" /> list to add to the cache.</param>
		public void Add(IEnumerable<KeyValuePair<TKey, TValue>> values)
		{
			values.ForEach(pair => Add(pair.Key, pair.Value, DateTime.Now));
		}


		/// <summary>
		/// Retrieve the specified value from cache.
		/// </summary>
		/// <param name="key">The key to search for the value.</param>
		/// <returns>The cached value, or null if not found.</returns>
		public TValue GetValue(TKey key)
		{
			CachedItem<TValue> value = Retrieve(key);

			// if a hit increment hit count and return value
			if (value != null)
			{
				IncrementHits();
				return value.Value;
			}

			IncrementMisses();
			return null;
		}


		/// <summary>
		/// Retrieves the specified value from cache if the item is not expired.
		/// </summary>
		/// <param name="key">The key to search for the value.</param>
		/// <returns>The cached value, or null if not found or expired.</returns>
		public TValue GetValidValue(TKey key)
		{
			return GetValidValue(key, false);
		}


		/// <summary>
		/// Retrieves the specified value from cache if the item is not expired.
		/// </summary>
		/// <param name="key">The key to search for the value.</param>
		/// <param name="shouldReleaseIfInvalid">True if should release reference (set to null) if invalid for GC.</param>
		/// <returns>The cached value, or null if not found or expired.</returns>
		public TValue GetValidValue(TKey key, bool shouldReleaseIfInvalid)
		{
			CachedItem<TValue> value = Retrieve(key);

			// if it's a hit AND if it's not expired, return the value
			if (value != null && !value.IsExpired)
			{
				IncrementHits();
				return value.Value;
			}

			// if should release if value is invalid, expire with release.
			if (shouldReleaseIfInvalid && value != null)
			{
				value.Expire(true);
			}

			// otherwise it's a miss.
			IncrementMisses();
			return null;
		}


		/// <summary>
		/// Retrieve the specified cached item from cache.
		/// </summary>
		/// <param name="key">The key to search.</param>
		/// <returns>The cached item, or null if not found.</returns>
		public CachedItem<TValue> Get(TKey key)
		{
			var result = Retrieve(key);

			// if a hit increment hit count and return value
			if (result != null)
			{
				IncrementHits();
				return result;
			}

			// otherwise, increment miss count and return null
			IncrementMisses();
			return null;
		}


		/// <summary>
		/// Forces a manual expiration of an item
		/// </summary>
		/// <param name="key">The key of the item to expire</param>
		public void Expire(TKey key)
		{
			Expire(key, false);
		}


		/// <summary>
		/// Forces a manual expiration of an item
		/// </summary>
		/// <param name="key">The key of the item to expire</param>
		/// <param name="shouldReleaseReference">True if should release reference (set to null) so eligible for GC collection.</param>
		public void Expire(TKey key, bool shouldReleaseReference)
		{
			var item = Retrieve(key);

			// manually expire the item
			if (item != null)
			{ 
				item.Expire(shouldReleaseReference);
			}
		}


		/// <summary>
		/// Removes all expired items from the cache.
		/// </summary>
		public void Cleanup()
		{
			var timer = Stopwatch.StartNew();

			List<TKey> expirationList = _dictionary.Where(i => i.Value.IsExpired)
											.Select(i => i.Key)
											.ToList();

			CleanCache(expirationList);

			// save duration of cleanup.
			unchecked
			{
				_lastCleanupDurationInMs = (int)timer.ElapsedMilliseconds;
				Interlocked.Increment(ref _cleanups);
			}
		}


		/// <summary>
		/// Determines whether the specified item in the cache is valid or not. If the item exists
		/// and is not expired, it is considered valid.
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>True if the item exists and is not expired, false otherwise.</returns>
		public bool IsValid(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			bool isValid = false;
			CachedItem<TValue> item;
			isValid = _dictionary.TryGetValue(key, out item);
			if ( isValid )
			{
				isValid = !item.IsExpired;
			}
			return isValid;
		}


		/// <summary>
		/// Removes all items from the Cache.
		/// </summary>
		public void Clear()
		{
			_hits = 0;
			_misses = 0;
			_cleanups = 0;

			_dictionary.Clear();
		}


		/// <summary>
		/// Disposes all disposable resources being held
		/// </summary>
		public void Dispose()
		{
			if (!_isDisposed)
			{
				_isDisposed = true;

				// cleanup timer is never instantiated if automatic timespan is <= 0ms
				if (_cleanUpTimer != null)
				{
					_cleanUpTimer.Dispose();
				}

				// clear lists
				_dictionary.Clear();
			}
		}


		/// <summary>
		/// Add an item to the cache
		/// </summary>
		/// <param name="key">Key of the item to add</param>
		/// <param name="cachedItem">The item to add</param>
		private void Add(TKey key, CachedItem<TValue> cachedItem)
		{
			_dictionary.AddOrUpdate(key, cachedItem, ( k, v ) => cachedItem);
		}


		/// <summary>
		/// Retrieve the specified cached item from cache.
		/// </summary>
		/// <param name="key">The key to search.</param>
		/// <returns>The cached item, or null if not found.</returns>
		private CachedItem<TValue> Retrieve(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			CachedItem<TValue> value = null;
			_dictionary.TryGetValue(key, out value);
			return value;
		}


		/// <summary>
		/// starts cleanup cache
		/// </summary>
		/// <param name="obj">not used, but required for timer api</param>
		/// <param name="e">Arguments passed from the timer event</param>
		private void OnTimedEvent(object obj, ElapsedEventArgs e)
		{
			Cleanup();
		}


		/// <summary>
		/// helper method to clean up cache when passed expired keys
		/// </summary>
		/// <param name="expiredKeys">List of expired keys</param>
		private void CleanCache(List<TKey> expiredKeys)
		{
			CachedItem<TValue> item;
			expiredKeys.ForEach(key => _dictionary.TryRemove(key, out item));
		}


		/// <summary>
		/// Add the item to the cache with the specified timestamp.
		/// </summary>
		/// <param name="key">The key to reference the cached value.</param>
		/// <param name="value">The cached value.</param>
		/// <param name="timestamp">The timestamp of the cached item.</param>
		private void Add(TKey key, TValue value, DateTime timestamp)
		{
			Add(key, new CachedItem<TValue>(value, _cacheExpirationTest, timestamp));
		}

		/// <summary>
		/// Helper method to increment the number of misses.
		/// </summary>
		private void IncrementMisses()
		{
			Interlocked.Increment(ref _misses);
		}

		/// <summary>
		/// Helper method to increment the number of hits.
		/// </summary>
		private void IncrementHits()
		{
			Interlocked.Increment(ref _hits);
		}
	}
}
