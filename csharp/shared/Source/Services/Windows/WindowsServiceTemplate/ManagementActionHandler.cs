using SharedAssemblies.Services.Windows.WindowsServiceFramework;


namespace $safeprojectname$
{
	/// <summary>
	/// The management action handler WCF service adapter.
	/// </summary>
	public class ManagementActionHandler : IManagementActionHandler
	{
		/// <summary>
		/// Gets the IWindowsService implementation to delegate requests to.
		/// </summary>
		public IWindowsService Service { get; private set; }


		/// <summary>
		/// Construct the management action handler with a reference to the service
		/// it will delegate action requests to.
		/// </summary>
		/// <param name="service">The IWindowsService implementation to delegate requests to.</param>
		public ManagementActionHandler(IWindowsService service)
		{
			Service = service;
		}


		/// <summary>
		/// Handles a management action request for this windows service.
		/// </summary>
		/// <param name="request">The requested action to perform.</param>
		/// <returns>The response from the requested action.</returns>
		public ManagementActionResponse OnManagementAction(ManagementActionRequest request)
		{
			// until implemented, just returns a dummy response.
			return new ManagementActionResponse
			{
				IsSuccessful = false,
				Result = "No actions implemented.",
				TransactionId = request.TransactionId
			};
		}
	}
}
