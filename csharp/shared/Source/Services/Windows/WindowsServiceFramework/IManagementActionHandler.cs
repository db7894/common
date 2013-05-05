namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// An interface which allows an implementing class to respond to management
	/// action requests.
	/// </summary>
	public interface IManagementActionHandler
	{
		/// <summary>
		/// Performs a management action on the windows service.
		/// </summary>
		/// <param name="request">The request to perform.</param>
		/// <returns>The resulting response message.</returns>
		ManagementActionResponse OnManagementAction(ManagementActionRequest request);
	}
}
