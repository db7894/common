using System.ComponentModel;
using SharedAssemblies.Services.Windows.WindowsServiceFramework;


namespace $safeprojectname$
{
	/// <summary>
	/// This class defines an installer which will install your windows service
	/// when run using the installutil .NET Framework utility program.
	/// </summary>
	[RunInstaller(true)]
	public class $safeprojectname$WindowsServiceInstaller : WindowsServiceInstaller
	{
		/// <summary>
		/// Installer that invokes the WindowsServiceInstaller with the type
		/// of the IWindowsService implementation class.  
		/// </summary>
		public $safeprojectname$WindowsServiceInstaller() : base(typeof(ServiceImplementation))
		{			
			// If you change the name of ServiceImplementation make sure you
			// change it in the base constructor call above.
		}
	}
}
