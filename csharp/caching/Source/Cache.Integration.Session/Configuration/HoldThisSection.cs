using System.Configuration;

namespace Cache.Integration.Configuration
{
	/// <summary>
	/// Settings section for the cache configuration
	/// </summary>
	public class HoldThisSection : ConfigurationSection
	{
		private static HoldThisSection _settings
			= ConfigurationManager.GetSection("HoldThis") as HoldThisSection;

		/// <summary>
		/// Used to parse and retrieve the settings for this sink
		/// </summary>
		public static HoldThisSection Settings { get { return _settings; } }
		
		/// <summary>
		/// The configuration for the cache
		/// </summary>
		[ConfigurationProperty("cache", IsRequired = true)]
		public CacheElement Cache
		{
			get { return (CacheElement)this["cache"]; }
		}

		/// <summary>
		/// The configuration for the cache provider
		/// </summary>
		[ConfigurationProperty("provider", IsRequired = false)]
		public ProviderElement Provider
		{
			get { return (ProviderElement)this["provider"]; }
		}
	}
}