using System.Runtime.Serialization;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web
{
	/// <summary>
	/// Base response for any Bashwork web service request.
	/// </summary>
	[DataContract(Namespace = "http://data.bashwork.com/8/2010/", Name = "BaseServiceResponse")]
	public class BaseServiceResponse : OperationResult
	{
		/// <summary>
		/// A unique identifier to associate with this transaction.
		/// Not required for operations that have no side-effects.
		/// </summary>
		[DataMember(Name = "TransactionId", Order = 1, IsRequired = false)]
		public string TransactionId { get; set; }
	}
}