using System;
using System.ServiceProcess;
using log4net;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// A generic Windows Service that can handle any assembly that
	/// implements IWindowsService (including AbstractWindowsService) 
	/// </summary>
	public sealed partial class WindowsServiceHarness : ServiceBase
	{
		/// <summary>
		/// log4net logger instance.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(WindowsServiceHarness));


		/// <summary>
		/// Get the class implementing the windows service
		/// </summary>
		public IWindowsService ServiceImplementation { get; private set; }


		/// <summary>
		/// Constructor a generic windows service from the given class
		/// </summary>
		/// <param name="serviceImplementation">Service implementation.</param>
		public WindowsServiceHarness(IWindowsService serviceImplementation)
		{
			// make sure service passed in is valid
			if (serviceImplementation == null)
			{
				throw new ArgumentNullException("serviceImplementation",
					"IWindowsService cannot be null in call to GenericWindowsService");
			}

			// set instance and backward instance
			ServiceImplementation = serviceImplementation;

			// configure our service
			ConfigureServiceFromAttributes(serviceImplementation);
		}


		/// <summary>
		/// Pass disposal off to service disposal method
		/// </summary>
		/// <param name="disposing">True if disposing</param>
		protected override void Dispose(bool disposing)
		{
			_log.InfoFormat("Disposing service [{0}].", disposing);

			if (disposing)
			{
				ServiceImplementation.Dispose();

				if(_components != null)
				{
					_components.Dispose();
				}
			}
			base.Dispose(disposing);
		}


		/// <summary>
		/// Override service control on continue
		/// </summary>
		protected override void OnContinue()
		{
			_log.Info("Continuing service.");

			// perform class specific behavior 
			ServiceImplementation.OnContinue();
		}


		/// <summary>
		/// Called when service is paused
		/// </summary>
		protected override void OnPause()
		{
			_log.Info("Pausing service.");

			// perform class specific behavior 
			ServiceImplementation.OnPause();
		}


		/// <summary>
		/// Called when a custom command is requested
		/// </summary>
		/// <param name="command">Id of custom command</param>
		protected override void OnCustomCommand(int command)
		{
			_log.InfoFormat("Issuing custom command [{0}].", command);

			// perform class specific behavior 
			ServiceImplementation.OnCustomCommand(command);
		}


		/// <summary>
		/// Called when the Operating System is shutting down
		/// </summary>
		protected override void OnShutdown()
		{
			_log.Info("Shutting down service.");
	
			// perform class specific behavior
			ServiceImplementation.OnShutdown();
		}


		/// <summary>
		/// Called when service is requested to start
		/// </summary>
		/// <param name="args">The startup arguments array.</param>
		protected override void OnStart(string[] args)
		{
			_log.Info("Starting service.");

			ServiceImplementation.OnStart(args);
		}


		/// <summary>
		/// Called when service is requested to stop
		/// </summary>
		protected override void OnStop()
		{
			_log.Info("Stopping service.");

			ServiceImplementation.OnStop();
		}


		/// <summary>
		/// Set configuration data
		/// </summary>
		/// <param name="serviceImplementation">The service with configuration settings.</param>
		private void ConfigureServiceFromAttributes(IWindowsService serviceImplementation)
		{
			var attribute = serviceImplementation.GetType().GetAttribute<WindowsServiceAttribute>();

			if(attribute != null)
			{
				EventLog.Source = string.IsNullOrEmpty(attribute.EventLogSource)
									? "WindowsServiceHarness"
									: attribute.EventLogSource;

				CanStop = attribute.CanStop;
				CanPauseAndContinue = attribute.CanPauseAndContinue;
				CanShutdown = attribute.CanShutdown;

				// we don't handle: laptop power change event
				CanHandlePowerEvent = false;

				// we don't handle: Term Services session event
				CanHandleSessionChangeEvent = false;

				// always auto-event-log 
				AutoLog = true;

				_log.InfoFormat("Service Configuration - EventLog: {0}, CanStop: {1}, " +
				          "CanPause: {2}, CanShutDown: {3}.",
				          EventLog.Source, CanStop, CanPauseAndContinue, CanShutdown);
			}
			else
			{
				throw new InvalidOperationException(
					string.Format("IWindowsService implementer {0} must have a WindowsServiceAttribute.",
					              serviceImplementation.GetType().FullName));
			}
		}
	}
}