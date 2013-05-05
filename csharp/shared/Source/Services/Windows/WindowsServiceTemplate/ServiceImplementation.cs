using System.ServiceProcess;
using SharedAssemblies.Monitoring.AutoCounters;
using SharedAssemblies.Services.Windows.WindowsServiceFramework;


namespace $safeprojectname$
{
	/// <summary> 
	/// <para>
	/// This is your custom implementation of the windows service.  Choose one of the implementation
	/// methods below and fill in the WindowsService attribute tags as appropriate.
	/// </para>
	/// <para>
	/// Note: you have three choices in how to implement this class:
	/// <list>
	///		<item>
	///			Inherit from AbstractThreadedWindowsService - performs unit of work processing.
	///		</item>
	///		<item>
	///			Inherit from AbstarctWindowsService - provides basic implementations of start, stop, etc.
	///		</item>
	///		<item>
	///			Implement IWindowsService - you provide all interface members yourself.
	///		</item>
	/// </list>
	/// </para>
	/// </summary>
	[WindowsService("$safeprojectname$",
		DisplayName = "$safeprojectname$ service display name",
		Description = "$safeprojectname$ service description.",
		CanPauseAndContinue = true,
		CanStop = true,
		UserName = null,
		Password = null,
		EventLogSource = "$safeprojectname$",
		StartMode = ServiceStartMode.Manual)]
	public class ServiceImplementation : AbstractWindowsService
	{
		// create the auto-counter cache which will start the heartbeat, etc.
		private static readonly ICounterCache _counters = AutoCounterCacheFactory.GetCache();


		/// <summary>
		/// This method is called when the service gets a request to start
		/// </summary>
		/// <param name="args">Any command line arguments</param>
		public override void OnStart(string[] args)
		{
		}


		/// <summary>
		/// This method is called when the service gets a request to stop
		/// </summary>
		public override void OnStop()
		{
		}


		/// <summary>
		/// This method is called when a service gets a request to pause, 
		/// but not stop completely.
		/// </summary>
		public override void OnPause()
		{
		}


		/// <summary>
		/// This method is called when a service gets a request to resume 
		/// after a pause is issued
		/// </summary>
		public override void OnContinue()
		{
		}


		/// <summary>
		/// This method is called when a custom command is issued to the service
		/// </summary>
		/// <param name="command">The command identifier to execute.</param>
		public override void OnCustomCommand(int command)
		{
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or 
		/// resetting unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
		}
	}
}
