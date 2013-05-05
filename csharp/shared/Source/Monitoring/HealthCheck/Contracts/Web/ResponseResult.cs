using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web
{
	/// <summary>
	/// Enumeration that identifies the overall status of an operation.
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "ResponseResult")]
	public enum ResponseResult
	{
		/// <summary>Indicates an operation succeeded</summary>
		[EnumMember(Value = "Success")]
		Success,

		/// <summary>Indicates an operation could succeed with warnings</summary>
		[EnumMember(Value = "Warning")]
		Warning,

		/// <summary>Indicates an operation failed</summary>
		[EnumMember(Value = "Failure")]
		Failure,
	}
}