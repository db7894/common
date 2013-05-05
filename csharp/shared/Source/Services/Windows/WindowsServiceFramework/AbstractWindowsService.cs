using System.ServiceProcess;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// This class takes the interface one step further and gives 
	/// a basic starting block with the two things you MUST do for
	/// a service, START and STOP.
	/// </summary>
	public abstract class AbstractWindowsService : IWindowsService
	{
		/// <summary>
		/// Get/set handle to service base
		/// </summary>
		public ServiceBase ServiceHarness { get; set; }


		/// <summary>
		/// This method is called to intialize the service (but not start).
		/// </summary>
		public virtual void OnInitialize()
		{
			// default behavior is nothing
		}


		/// <summary>
		/// This method is called when the service gets a request to start
		/// </summary>
		/// <param name="args">Any command line arguments</param>
		public abstract void OnStart(string[] args);


		/// <summary>
		/// This method is called when the service gets a request to stop
		/// </summary>
		public abstract void OnStop();


		/// <summary>
		/// This method is called when a service gets a request to pause, 
		/// but not stop completely.
		/// </summary>
		public virtual void OnPause()
		{
			// default behavior is nothing
		}


		/// <summary>
		/// This method is called when a service gets a request to resume 
		/// after a pause is issued
		/// </summary>
		public virtual void OnContinue()
		{
			// default behavior is nothing
		}


		/// <summary>
		/// This method is called when the machine the service is running on
		/// is being shutdown
		/// </summary>
		public virtual void OnShutdown()
		{
			// default behavior is to shunt to OnStop();
			OnStop();
		}


		/// <summary>
		/// This method is called when a custom command is issued to the service
		/// </summary>
		/// <param name="command">command id number</param>
		public virtual void OnCustomCommand(int command)
		{
			// default behavior is nothing
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, 
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			// default behavior is nothing
		}
	}
}