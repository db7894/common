using System;
using SharedAssemblies.General.Caching;
using SharedAssemblies.General.Caching.Serialization;
using SharedAssemblies.General.Caching.Cleaning;

namespace Cache.Integration.Configuration
{
	/// <summary>
	/// A helper factory to initialize a cache from the supplied
	/// configuration information.
	/// </summary>
	public static class CacheConfigurationFactory
	{
		/// <summary>
		/// Initialize a new cache from the supplied configuration
		/// </summary>
		/// <returns>The initialized cache</returns>
		public static ICache<TKey, TValue> Initialize<TKey, TValue>()
			where TValue : new()
		{
			var settings = HoldThisSection.Settings;
			var expiration = ParseExpiration<TValue>(settings.Cache.Expires);
			var serializer = ParseSerializer(settings.Cache.Serializer);
			var cache = ParseCache<TKey, TValue>(settings.Cache.Type, expiration);
			var cleanup = ParseCleaner<TKey, TValue>(settings.Cache.Cleanup, cache);
			return cache;
		}

		/// <summary>
		/// Given a string value, return the required cache implementation
		/// </summary>
		/// <param name="cache">The cache to use</param>
		/// <returns>The initialized cache strategy</returns>
		public static ICache<TKey, TValue> ParseCache<TKey, TValue>(string cache,
			IExpirationStrategy<TValue> expiration)
			where TValue : new()
		{
			var pieces = cache.Split(new[] { '<', '>' }); 

			switch (pieces[0].ToLower())
			{
				case "checkandset":			return new CheckAndSetCache<TKey, TValue>(expiration);
				case "genericrefresh":		// cannot be configured for now
				case "genericondemand":		// cannot be configured for now
				case "specificrefresh":		// cannot be configured for now
				case "specificondemand":	// cannot be configured for now
				default: throw new ArgumentException("Invalid type of cache requested");
			}
		}

		/// <summary>
		/// Given a string value, return the required serializer
		/// </summary>
		/// <param name="serializer">The serializer to use</param>
		/// <returns>The initialized serializer</returns>
		public static ICacheSerializer ParseSerializer(string serializer)
		{
			switch (serializer.ToLower())
			{
				case "json":	return new JsonCacheSerializer();
				case "xml":		return new XmlCacheSerializer();
				case "binary":	return new BinaryCacheSerializer();
				default: throw new ArgumentException("Invalid type of serializer requested");
			}
		}

		/// <summary>
		/// Given a string value, return the required expiration strategy
		/// </summary>
		/// <param name="expiration">The expiration strategy to use</param>
		/// <returns>The initialized expiration strategy</returns>
		public static IExpirationStrategy<TValue> ParseExpiration<TValue>(string expiration)
		{
			var pieces = expiration.Split(new[] { '(', ')' }); 

			switch (pieces[0].ToLower())
			{
				case "always":		return Expires.Always<TValue>();
				case "never":		return Expires.Never<TValue>();
				case "notusedin":	return Expires.NotUsedIn<TValue>(TimeSpan.Parse(pieces[1]));
				case "hits":		return Expires.Hits<TValue>(int.Parse(pieces[1]));
				case "nextday":		return Expires.NextDay<TValue>();
				case "timespan":	return Expires.TimeSpan<TValue>(TimeSpan.Parse(pieces[1]));
				case "at":			return Expires.At<TValue>(DateTime.Parse(pieces[1]));
				case "when":		// cannot be configured for now
				case "intropspect":	// cannot be configured for now
				default: throw new ArgumentException("Invalid type of expiration strategy requested");
			}
		}

		/// <summary>
		/// Given a string value, return the required cleanup strategy
		/// </summary>
		/// <param name="cleanup">The expiration strategy to use</param>
		/// <param name="cache">The cache to clean</param>
		/// <returns>The initialized cleanup strategy</returns>
		public static ICleanupStrategy ParseCleaner<TKey, TValue>(string cleanup,
			ICache<TKey, TValue> cache)
		{
			var pieces = cleanup.Split(new[] { '(', ')' }); 

			switch (pieces[0].ToLower())
			{
				case "nothing":		return Cleanup.Nothing(cache);
				case "lru":			return Cleanup.LeastRecentlyUsed(cache, int.Parse(pieces[1]));
				case "mru":			return Cleanup.AllButMostRecentlyUsed(cache, int.Parse(pieces[1]));
				case "boundedfifo":	return Cleanup.BoundedAtFifo(cache, int.Parse(pieces[1]));
				case "boundedlifo":	return Cleanup.BoundedAtLifo(cache, int.Parse(pieces[1]));
				case "expired":		return Cleanup.Expired(cache);
				case "olderthan":	return Cleanup.OlderThan(cache, TimeSpan.Parse(pieces[1]));
				case "youngerthan":	return Cleanup.YoungerThan(cache, TimeSpan.Parse(pieces[1]));
				case "notusedin":	return Cleanup.NotUsedIn(cache, TimeSpan.Parse(pieces[1]));
				default: throw new ArgumentException("Invalid type of cleanup strategy requested");
			}
		}
	}
}


