using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// Represents the current versions of the application
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "GetApplicationVersionsResponse")]
	public class GetApplicationVersionsResponse
	{
		/// <summary>
		/// A collection of version information about the
		/// specified application
		/// </summary>
		[DataMember(Name = "Versions", Order = 1, IsRequired = true)]
		public IDictionary<string, string> Versions { get; set; }
	}
}

