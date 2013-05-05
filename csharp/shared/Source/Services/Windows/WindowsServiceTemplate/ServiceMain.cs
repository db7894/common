using System;
using System.Linq;
using System.Configuration;
using log4net;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using SharedAssemblies.Services.Windows.WindowsServiceFramework;


namespace $safeprojectname$
{
	/// <summary>
	/// <para>
	/// The launcher for the windows service, determines from command line arguments if
	/// the program should launch as a windows service or as a console application.
	/// </para>
	/// <para>
	/// To launch as a console application for debugging, add the /console command line
	/// argument to the debugging settings.
	/// </para>
	/// </summary>
	public static class ServiceMain
	{
		/// <summary>
		/// A logging instance for the log4net logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(ServiceMain));


		/// <summary>
		/// Static constructor to get log4net configured properly.
		/// </summary>
		static ServiceMain()
		{
			// force configuration of log4net here so no other classes call first
			log4net.Config.XmlConfigurator.Configure();	
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <param name="arguments">Any command line arguments used to start the program.</param>
		public static void Main(string[] arguments)
		{
			try
			{
				if (arguments.Contains("-install"))
				{
					// you can pass additional installers here...
					_log.Info("Installing Windows Service.");
					WindowsServiceInstaller.RuntimeInstall<ServiceImplementation>(
						new AutoCounterInstaller());
				}
				else if (arguments.Contains("-uninstall"))
				{
					// you can pass additional uninstallers here...
					_log.Info("Uninstalling Windows Service.");
					WindowsServiceInstaller.RuntimeUnInstall<ServiceImplementation>(
						new AutoCounterInstaller());
				}
				else
				{
					_log.Info("Starting ServiceMain.Main()");

					// if you rename the IWindowsService implementation or IManagementActionHandler
					// implementation, make sure to change it here too.
					var implementation = new ServiceImplementation();
					var actionHandler = AreManagementActionsEnabled() 
						? new ManagementActionHandler(implementation) : null;

					// pass the arguments, the IWindowsService implementation, and the action handler
					// implementation.
					ServiceLauncher.Run(arguments, implementation, actionHandler);
				}
			}
			catch (Exception ex)
			{
				_log.Error("Exception in ServiceMain.Main().", ex);
				throw;
			}
			finally
			{
				_log.Info("Exiting ServiceMain.Main().");
			}
		}

		
		/// <summary>
		/// Check to see if management actions are enabled or not.
		/// </summary>
		/// <returns>True if enabled in the app.config.</returns>
		private static bool AreManagementActionsEnabled()
		{
			bool result;

			// try to parse the app settings to see if the management actions are enabled
			if(!bool.TryParse(ConfigurationManager.AppSettings.Get("ManagementActionsAllowed"), out result))
			{
				// default to false if not found or not parsable
				result = false;
			}

			return result;
		}
	}
}
