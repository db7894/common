using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// Represents the current health status of the application.
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "GetApplicationHealthResponse")]
	public class GetApplicationHealthResponse
	{
		/// <summary>
		/// A flag indicating if the specified application is running
		/// </summary>
		[DataMember(Name = "IsFunctioning", Order = 1, IsRequired = true)]
		public bool IsFunctioning { get; set; }

		/// <summary>
		/// A collection of dependencies and their current status
		/// </summary>
		[DataMember(Name = "Dependencies", Order = 1, IsRequired = true)]
		public List<HealthTestResult> Dependencies { get; set; }
	}
}

