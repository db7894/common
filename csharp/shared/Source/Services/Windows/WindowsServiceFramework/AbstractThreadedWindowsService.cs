using System.Threading;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// This class takes the interface one step further and gives 
	/// a basic starting block with the two things you MUST do for
	/// a service, START and STOP.
	/// </summary>
	public abstract class AbstractThreadedWindowsService : AbstractWindowsService
	{
		// thread management members
		private Thread _workerThread;
		private bool _isPaused;
		private bool _isRunning;

		// synch object
		private readonly object _synchRoot = new object();


		/// <summary>
		/// Interval to wait when paused before next pause check
		/// </summary>
		public uint PauseSleepInterval { get; set; }


		/// <summary>
		/// Interval to wait between calls to DoUnitOfWork()
		/// if it returns true meaning more work is ready
		/// </summary>
		public uint MoreWorkSleepInterval { get; set; }


		/// <summary>
		/// Interval to wait between calls to DoUnitOfWork() if
		/// it returns false meaning no more work ready
		/// </summary>
		public uint NoWorkSleepInterval { get; set; }


		/// <summary>
		/// Interval to wait for the thread to stop gracefully before
		/// attempting a hard kill
		/// </summary>
		public uint StopWaitInterval { get; set; }


		/// <summary>
		/// The delegate for getting notifications on thread events.
		/// </summary>
		public delegate void ThreadedServiceEvent();

	
		/// <summary>
		/// Do initialization
		/// </summary>
		protected AbstractThreadedWindowsService()
		{
			// default pauses
			PauseSleepInterval = 5000;
			MoreWorkSleepInterval = 0;
			NoWorkSleepInterval = 1000;
			StopWaitInterval = 5000;
		}


		/// <summary>
		/// This method is called when the service gets a request to start
		/// </summary>
		/// <param name="args">Any command line arguments</param>
		public sealed override void OnStart(string[] args)
		{
			lock (_synchRoot)
			{
				// call helper 
				OnThreadStart(args);

				// start the thread
				_isRunning = true;
				_workerThread = new Thread(ServiceThreadBody) { IsBackground = true };
				_workerThread.Start();
			}
		}


		/// <summary>
		/// This method is called when the service gets a request to stop
		/// </summary>
		public sealed override void OnStop()
		{
			lock (_synchRoot)
			{
				// halt thread and attempt to wait for a join
				_isRunning = false;
				if (!_workerThread.Join((int)StopWaitInterval))
				{
					_workerThread.Abort();
				}

				// call helper
				OnThreadStop();
			}
		}


		/// <summary>
		/// This is the method called at each pulse of the 
		/// service to do some unit of work.  Should return true
		/// if there is more work to do immediately.
		/// </summary>
		/// <returns>True if unit of work was successful.</returns>
		public abstract bool DoUnitOfWork();


		/// <summary>
		/// Helper method to allow you to specify actions when thread starts
		/// </summary>
		/// <param name="args">Arguments for the thread start.</param>
		public virtual void OnThreadStart(string[] args)
		{
		}


		/// <summary>
		/// Helper method to allow you to specify actions when thread stops
		/// </summary>
		public virtual void OnThreadStop()
		{
		}


		/// <summary>
		/// This method is called when a service gets a request to pause, 
		/// but not stop completely.
		/// </summary>
		public sealed override void OnPause()
		{
			lock (_synchRoot)
			{
				_isPaused = true;
			}
		}


		/// <summary>
		/// This method is called when a service gets a request to resume 
		/// after a pause is issued
		/// </summary>
		public sealed override void OnContinue()
		{
			lock (_synchRoot)
			{
				_isPaused = false;
			}
		}


		/// <summary>
		/// The thread body for a threaded service call
		/// </summary>
		private void ServiceThreadBody()
		{
			// as long as we're running continue to look for work
			while (_isRunning)
			{
				// if we're not paused do a unit of work
				if (!_isPaused)
				{
					if (DoUnitOfWork())
					{
						// if more work, don't want to yield the thread
						// if wait is zero.
						if (MoreWorkSleepInterval > 0)
						{
							// if more work exists, may sleep a shorter interval
							Thread.Sleep((int)MoreWorkSleepInterval);
						}
					}
					else
					{
						// otherwise, if no more work at the moment, may
						// sleep longer interval.  In these cases still
						// yield thread (Sleep(0)) on an interval of zero
						Thread.Sleep((int)NoWorkSleepInterval);
					}
				}
				else
				{
					// if paused, sleep and check pause again.
					// In these cases still yield thread (Sleep(0)) 
					// on an interval of zero
					Thread.Sleep((int)PauseSleepInterval);
				}
			}
		}
	}
}