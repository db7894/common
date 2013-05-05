using System.Runtime.Serialization;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// Represents the result of a management request to the windows service
	/// </summary>
	[DataContract]
	public class ManagementActionResponse
	{
		/// <summary>
		/// A unique transaction identifier for the request that 
		/// ties the request and response together in case the 
		/// caller is consuming asynchronously.
		/// </summary>
		[DataMember(Name = "TransactionId", IsRequired = true)]
		public string TransactionId { get; set; }


		/// <summary>
		/// The results of the command.  Can be a boolean
		/// </summary>
		[DataMember(Name = "Result", IsRequired = true)]
		public string Result { get; set; }


		/// <summary>
		/// A flag indicating if the management request was successful
		/// </summary>
		[DataMember(Name = "IsSuccessful", IsRequired = true)]
		public bool IsSuccessful { get; set; }
	}
}
