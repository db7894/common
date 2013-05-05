using System.ServiceModel;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// Interface for a wcf management service
	/// </summary>
	[ServiceContract(
		Name = "ManagementService",
		Namespace = "http://service.bashwork.com/2010/05/")]
	public interface IManagementActionService
	{
		/// <summary>
		/// Method to receive an arbitrary management command
		/// </summary>
		/// <param name="request">The management request</param>
		/// <returns>The result of the management operation</returns>
		[OperationContract(Name = "PerformRequest", IsOneWay = false)]
		ManagementActionResponse PerformManagementAction(ManagementActionRequest request);
	}
}
