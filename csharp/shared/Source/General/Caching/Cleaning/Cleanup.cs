using System;
using System.Linq;
using SharedAssemblies.General.Caching.Provider;
using System.Diagnostics;

namespace SharedAssemblies.General.Caching.Cleaning
{
    /// <summary>
    /// A wrapper class to help create a janitor stategy
    /// </summary>
    public static class Cleanup
    {
		/// <summary>
		/// Janitor strategy that does absolutely no cleaning.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy Nothing<TKey, TValue>(
			ICache<TKey, TValue> cache)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return Enumerable.Empty<TKey>();
			});
		}

		/// <summary>
		/// Janitor strategy that cleans up the N least hit elements in
		/// the cache (the least popular).
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="count">The number of elements to remove</param>
        /// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy LeastPopular<TKey, TValue>(
			ICache<TKey, TValue> cache, int count)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return stream.OrderBy(pair => pair.Value.Hits)
					.Select(pair => pair.Key).Take(count);
			});			
		}

		/// <summary>
		/// Janitor strategy that cleans up all but the N most hit elements
		/// in the cache (the most popular).
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="count">The number of elements to remove</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy AllButMostPopular<TKey, TValue>(
			ICache<TKey, TValue> cache, int count)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return stream.OrderByDescending(pair => pair.Value.Hits)
					.Select(pair => pair.Key).Skip(count);
			});
		}

		/// <summary>
		/// Janitor strategy that cleans up the N least recently used
		/// elements in the cache.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="count">The number of elements to remove</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy LeastRecentlyUsed<TKey, TValue>(
			ICache<TKey, TValue> cache, int count)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return stream.OrderBy(pair => pair.Value.LastTouched)
					.Select(pair => pair.Key).Take(count);
			});
		}

		/// <summary>
		/// Janitor strategy that cleans up all but the N most recently
		/// used elements in the cache.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="count">The number of elements to keep</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy AllButMostRecentlyUsed<TKey, TValue>(
			ICache<TKey, TValue> cache, int count)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return stream.OrderByDescending(pair => pair.Value.LastTouched)
					.Select(pair => pair.Key).Skip(count);
			});
		}

		/// <summary>
		/// Janitor strategy that keeps the cache at a specified size
		/// by removing the oldest elements.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="count">The number of elements to keep</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy BoundedAtFifo<TKey, TValue>(
			ICache<TKey, TValue> cache, int count)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return stream.OrderBy(pair => pair.Value.Created)
					.Select(pair => pair.Key).Take(stream.Count() - count);
			});
		}

		/// <summary>
		/// Janitor strategy that keeps the cache at a specified size
		/// by removing the newest elements.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="count">The number of elements to keep</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy BoundedAtLifo<TKey, TValue>(
			ICache<TKey, TValue> cache, int count)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return stream.OrderByDescending(pair => pair.Value.Created)
					.Select(pair => pair.Key).Take(stream.Count() - count);
			});
		}

		/// <summary>
		/// Janitor strategy that cleans all the the expired
		/// elements in one sweep.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy Expired<TKey, TValue>(
			ICache<TKey, TValue> cache)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				return stream.Where(pair => pair.Value.IsExpired)
					.Select(pair => pair.Key);
			});
		}

		/// <summary>
		/// Janitor strategy that cleans all elements that
		/// are past a specified age.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="span">The older than timespan</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy OlderThan<TKey, TValue>(
			ICache<TKey, TValue> cache, TimeSpan span)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				var current = Stopwatch.GetTimestamp() - span.Ticks;
				return stream.Where(pair => pair.Value.Created < current)
					.Select(pair => pair.Key);
			});
		}

		/// <summary>
		/// Janitor strategy that cleans all elements that
		/// are younger than a specified age.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="span">The younger than timespan</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy YoungerThan<TKey, TValue>(
			ICache<TKey, TValue> cache, TimeSpan span)
		{
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				var current = Stopwatch.GetTimestamp() - span.Ticks;
				return stream.Where(pair => pair.Value.Created > current)
					.Select(pair => pair.Key);
			});			
		}

		/// <summary>
		/// Janitor strategy that cleans all elements that
		/// haven't been touched in a while.
		/// </summary>
		/// <param name="cache">The cache to attach the strategy to</param>
		/// <param name="span">The last used by timespan</param>
		/// <returns>An initialized IJanitor</returns>
		public static ICleanupStrategy NotUsedIn<TKey, TValue>(
			ICache<TKey, TValue> cache, TimeSpan span)
		{			
			return new GenericCleanupStrategy<TKey, TValue>(cache, stream => {
				var current = Stopwatch.GetTimestamp() - span.Ticks;
				return stream.Where(pair => pair.Value.LastTouched < current)
					.Select(pair => pair.Key);
			});
		}
    }
}
