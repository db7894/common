using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// Represents the current configuration of the application
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "ApplicationConfiguration")]
	public class ApplicationConfiguration
	{
		/// <summary>
		/// A collection of configuration information about the
		/// specified application
		/// </summary>
		[DataMember(Name = "Configuration", Order = 1, IsRequired = true)]
		public IDictionary<string, string> Configuration { get; set; }

		/// <summary>
		/// The name of the specified configuration section
		/// </summary>
		[DataMember(Name = "SectionName", Order = 1, IsRequired = true)]
		public string SectionName;
	}
}

