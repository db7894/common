using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web
{
	/// <summary>
	/// Base response for any Bashwork web service request that requires two-way communication with a client.
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "OperationResult")]
	public class OperationResult
	{
		/// <summary>The result of the operation</summary>
		[DataMember(Name = "Result", Order = 1, IsRequired = true)]
		public ResponseResult Result { get; set; }

		/// <summary>If there is an error, the error that occurred</summary>
		[DataMember(Name = "ErrorCode", Order = 1, IsRequired = false)]
		public int ErrorCode { get; set; }

		/// <summary>An optional information message</summary>
		[DataMember(Name = "Message", Order = 1, IsRequired = false)]
		public string Message { get; set; }
	}
}