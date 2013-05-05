using System.ServiceProcess;
 

namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// This class handles the running of the windows service under
	/// Windows Service manager.
	/// </summary>
	internal static class WindowsServiceMain
	{
		/// <summary>
		/// Run a service from the service manager.
		/// </summary>
		/// <param name="service">The service to run as a service.</param>
		internal static void Run(IWindowsService service)
		{
			using (var serviceHarness = new WindowsServiceHarness(service))
			{
				// start the service
				ServiceBase.Run(serviceHarness);
			}
		}
	}
}