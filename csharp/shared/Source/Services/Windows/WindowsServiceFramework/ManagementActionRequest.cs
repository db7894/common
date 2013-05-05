using System.Runtime.Serialization;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// Represents a management request to the windows service
	/// </summary>
	[DataContract]
	public class ManagementActionRequest
	{
		/// <summary>
		/// A unique transaction identifier for this request.
		/// </summary>
		[DataMember(Name = "TransactionId", IsRequired = true)]
		public string TransactionId { get; set; }


		/// <summary>
		/// The request to send, can be a command number, XML, or
		/// any other payload dictated by the service itself.
		/// </summary>
		[DataMember(Name = "Request", IsRequired = true)]
		public string Request { get; set; }
	}
}
