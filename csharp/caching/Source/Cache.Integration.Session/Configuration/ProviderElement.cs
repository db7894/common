using System.Configuration;

namespace Cache.Integration.Configuration
{
	/// <summary>
	/// Settings section for the message sink configuration
	/// </summary>
	public class ProviderElement : ConfigurationElement
	{
		/// <summary>
		/// The type of provider to use
		/// </summary>
		[ConfigurationProperty("type", DefaultValue = "memory", IsRequired = true)]
		[StringValidator(MinLength = 5, MaxLength = 256)]
		public string Type
		{
			get { return (string)this["type"]; }
			set { this["type"] = value; }
		}

		/// <summary>
		/// The connection to the provider
		/// </summary>
		[ConfigurationProperty("connection", DefaultValue = "default", IsRequired = true)]
		[StringValidator(MinLength = 5, MaxLength = 256)]
		public string Connection
		{
			get { return (string)this["connection"]; }
			set { this["connection"] = value; }
		}
	}
}