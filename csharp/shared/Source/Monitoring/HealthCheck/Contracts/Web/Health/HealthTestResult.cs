using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// Represents the current health status of the application.
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "HealthTestResult")]
	public class HealthTestResult
	{
		/// <summary>
		/// The name of the requested dependency
		/// </summary>
		[DataMember(Name = "Name", Order = 1, IsRequired = true)]
		public string Name { get; set; }

		/// <summary>
		/// A flag indicating if the specified application is running
		/// </summary>
		[DataMember(Name = "IsFunctioning", Order = 1, IsRequired = true)]
		public bool IsFunctioning { get; set; }

		/// <summary>
		/// A message explaining the current state of the dependency
		/// </summary>
		[DataMember(Name = "ResponseMessages", Order = 1, IsRequired = true)]
		public List<string> ResponseMessages { get; set; }
	}
}

