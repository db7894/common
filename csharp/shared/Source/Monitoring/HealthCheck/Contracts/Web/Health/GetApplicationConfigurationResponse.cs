using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// Represents the current configuration of the application
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "GetApplicationConfigurationResponse")]
	public class GetApplicationConfigurationResponse
	{
		/// <summary>
		/// A collection of configuration information about the specified application
		/// </summary>
		[DataMember(Name = "Configurations", Order = 1, IsRequired = true)]
		public List<ApplicationConfiguration> Configurations { get; set; }
	}
}

