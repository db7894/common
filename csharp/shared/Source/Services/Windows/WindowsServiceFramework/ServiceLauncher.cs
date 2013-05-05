using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel;
using log4net;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// The actual main entry point to the program
	/// </summary>
	public static class ServiceLauncher
	{
		/// <summary>
		/// Command line parameter to run without an admin service.
		/// </summary>
		private const string _adminServiceOffCommandLineOption = "/NOADMIN";


		/// <summary>
		/// Grab a log4net instance of a logger.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(ServiceLauncher));


		/// <summary>
		/// Main entry point.  If arg 1 is CONSOLE then we run as a 
		/// console app, otherwise run as a windows service.
		/// </summary>
		/// <param name="args">Any args to pass on to service wrapper.</param>
		/// <param name="service">The service implementation to wrap and run.</param>
		public static void Run(string[] args, IWindowsService service)
		{
			// run without a service action handler.
			Run(args, service, null);
		}


		/// <summary>
		/// Main entry point.  If arg 1 is CONSOLE then we run as a 
		/// console app, otherwise run as a windows service.
		/// </summary>
		/// <param name="args">Any args to pass on to service wrapper.</param>
		/// <param name="service">The service implementation to wrap and run.</param>
		/// <param name="handler">The handler for action events.</param>
		public static void Run(string[] args, IWindowsService service, IManagementActionHandler handler)
		{
			// print service info
			_log.Info("Creating Service.");
			_log.Info("Service arguments: " + args.Summarize(10));
			_log.Info("IWindowsService Implementer: " + service.GetType().FullName);
			_log.Info("Interactive Mode: " + Environment.UserInteractive);

			// start the management service
			using (var managementService = CreateManagementServiceHost(args, handler))
			{
				// open the management service.
				OpenManagementService(managementService);

				// determine which type of main to used based on whether we're in interactive
				// mode or not.  If interactive, launch as console, otherwise as service.
				if (Environment.UserInteractive)
				{
					// run as a console app (debug mode)
					_log.Info("Running service as console application.");
					ConsoleMain.Run(args, service);
				}
				else
				{
					// run as a true windows service
					_log.Info("Running service as windows service.");
					WindowsServiceMain.Run(service);
				}
			}
		}


		/// <summary>
		/// Create the service host given the args and the handler for management action
		/// </summary>
		/// <param name="args">Args for starting the management service.</param>
		/// <param name="handler">Handler for management service events.</param>
		/// <returns>The service host for the management service.</returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "Do not want inability to create management service to stop windows service.")]
		private static ServiceHost CreateManagementServiceHost(string[] args,
			IManagementActionHandler handler)
		{
			ServiceHost host = null;

			if (!args.Contains(_adminServiceOffCommandLineOption, StringComparer.InvariantCultureIgnoreCase)
				&& handler != null)
			{
				_log.Info("IManagementActionHandler Implementer: " + handler.GetType().FullName);

				try
				{
					_log.Info("Creating management service host.");
					host = new ServiceHost(new ManagementActionService(handler));
				}

				catch (Exception ex)
				{
					_log.Error("Error creating management service host, management service not started.",
						ex);
				}
			}

			return host;
		}


		/// <summary>
		/// Open the management service to accept management commands from WCF.
		/// If an exception is thrown, we do NOT want to halt the service itself.
		/// </summary>
		/// <param name="managementService">The management service to start.</param>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "Do not want inability to open management service to stop windows service.")]
		private static void OpenManagementService(ServiceHost managementService)
		{
			if (managementService != null)
			{
				try
				{
					_log.Info("Opening management service host.");
					managementService.Open(TimeSpan.FromSeconds(5));
				}

				catch (Exception ex)
				{
					_log.Error("Error opening managment service host, management service not started.", ex);
				}
			}
		}
	}
}