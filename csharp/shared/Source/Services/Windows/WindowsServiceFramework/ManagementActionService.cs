
using System.ServiceModel;

namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// Implementation of this <see cref="IManagementActionService"/> to manage a windows
	/// service.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Single)]
	public class ManagementActionService : IManagementActionService
	{
		/// <summary>
		/// Gets the handle to the windows service.
		/// </summary>
		public IManagementActionHandler Actionable { get; private set; }


		/// <summary>
		/// Construcst a management WCF service for an actionable item.
		/// </summary>
		/// <param name="actionable">The class to manage with management events.</param>
		public ManagementActionService(IManagementActionHandler actionable)
		{
			Actionable = actionable;
		}


		/// <summary>
		/// Method to receive an arbitrary management command
		/// </summary>
		/// <param name="request">The management request</param>
		/// <returns>The result of the management operation</returns>
		public ManagementActionResponse PerformManagementAction(ManagementActionRequest request)
		{
			return (Actionable != null) ? Actionable.OnManagementAction(request) : null;
		}
	}
}
