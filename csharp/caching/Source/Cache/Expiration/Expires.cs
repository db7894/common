using System;
using System.Diagnostics;

namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// A wrapper class to create a cache expiration strategy directly
	/// from a predicate.
    /// </summary>
	public static class Expires
    {
		/// <summary>
		/// Expiration strategy that forces the cached value to always expire.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
        /// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> Always<TItem>()
		{
			return new GenericExpirationStrategy<TItem>(item =>
				true);
		}

		/// <summary>
		/// Expiration strategy that forces the cached value to never expire.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
        /// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> Never<TItem>()
		{
			return new GenericExpirationStrategy<TItem>(item =>
				false);
		}

		/// <summary>
		/// Expiration strategy that forces the cached value to expire
		/// when it is not used for a given amount of time.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
		/// <param name="span">The lenght of time between item usage</param>
		/// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> NotUsedIn<TItem>(TimeSpan span)
		{
			return new GenericExpirationStrategy<TItem>(item =>
				(Stopwatch.GetTimestamp() - item.LastTouched) >= span.Ticks);
		}

		/// <summary>
		/// Expiration strategy that forces the cached value to expire
		/// when it is queried N times.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
		/// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> Hits<TItem>(int count)
		{
			return new GenericExpirationStrategy<TItem>(item =>
				item.Hits == count);
		}

		/// <summary>
		/// Expiration strategy that forces the cached value to expire upon
		/// some external condition.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
		/// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> When<TItem>(Func<bool> condition)
		{
			return new GenericExpirationStrategy<TItem>(_ =>
				condition());
		}

		/// <summary>
		/// Expiration strategy to determine if the cached item was cached on the same day.
		/// If it was, the item is not expired. If it was not, the item is expired.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
		/// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> NextDay<TItem>()
		{
			return new GenericExpirationStrategy<TItem>(item =>
				(Stopwatch.GetTimestamp() - item.Created) >= System.TimeSpan.TicksPerDay);
		}

		/// <summary>
		/// Expiration strategy to determine if the current time minus the cached
		/// time is greater than the specified timespan.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
		/// <param name="span">The lenght of time to keep the item valid</param>
        /// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> TimeSpan<TItem>(TimeSpan span)
		{
			return new GenericExpirationStrategy<TItem>(item => 
				(Stopwatch.GetTimestamp() - item.Created) >= span.Ticks);
		}

		/// <summary>
		/// Expiration strategy to determine if the specified time has been
		/// reached.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
		/// <param name="time">The time that the cache item should expire at</param>
		/// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> At<TItem>(DateTime time)
		{
			return new GenericExpirationStrategy<TItem>(_ =>
				DateTime.Now.TimeOfDay >= time.TimeOfDay);
		}

		/// <summary>
		/// Expiration strategy that allows the object itself to tell the cache
		/// when it should be expired.
		/// </summary>
		/// <typeparam name="TItem">The type of item in the cache.</typeparam>
        /// <returns>An initialized IExperationStrategy</returns>
		public static IExpirationStrategy<TItem> Introspect<TItem>()
			where TItem : IExpirationStrategy<TItem>
		{
			return new GenericExpirationStrategy<TItem>(item =>
				item.Value.IsExpired(item));
		}
    }
}
