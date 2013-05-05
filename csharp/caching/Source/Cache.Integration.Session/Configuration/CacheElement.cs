using System.Configuration;

namespace Cache.Integration.Configuration
{
	/// <summary>
	/// Settings section for the message sink configuration
	/// </summary>
	public class CacheElement : ConfigurationElement
	{
		/// <summary>
		/// A unique name for this cache
		/// </summary>
		[ConfigurationProperty("name", DefaultValue = "default", IsRequired = false)]
		[StringValidator(MinLength = 5, MaxLength = 256)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		/// <summary>
		/// The type of cache to create
		/// </summary>
		[ConfigurationProperty("type", DefaultValue = "checkandset", IsRequired = false)]
		[StringValidator(MinLength = 5, MaxLength = 256)]
		public string Type
		{
			get { return (string)this["type"]; }
			set { this["type"] = value; }
		}

		/// <summary>
		/// The method of cleanup for the cache
		/// </summary>
		[ConfigurationProperty("cleanup", DefaultValue = "expired", IsRequired = false)]
		[StringValidator(MinLength = 5, MaxLength = 256)]
		public string Cleanup
		{
			get { return (string)this["cleanup"]; }
			set { this["cleanup"] = value; }
		}

		/// <summary>
		/// The method of expiring values in the cache
		/// </summary>
		[ConfigurationProperty("expires", DefaultValue = "timespan(3600)", IsRequired = false)]
		[StringValidator(MinLength = 5, MaxLength = 256)]
		public string Expires
		{
			get { return (string)this["expires"]; }
			set { this["expires"] = value; }
		}

		/// <summary>
		/// The serizlier to use with the provider
		/// </summary>
		[ConfigurationProperty("serializer", DefaultValue = "json", IsRequired = false)]
		[StringValidator(MinLength = 2, MaxLength = 256)]
		public string Serializer
		{
			get { return (string)this["serializer"]; }
			set { this["serializer"] = value; }
		}
	}
}