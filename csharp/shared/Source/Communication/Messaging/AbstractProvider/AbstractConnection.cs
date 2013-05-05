using System;
using System.Timers;
using log4net;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// The basic connection/disconnection framework
	/// </summary>
	public abstract class AbstractConnection : IConnection
	{
		private static readonly TimeSpan _reconnectTimeSpan = TimeSpan.FromSeconds(10);
		private static readonly ILog _log = LogManager.GetLogger(typeof(AbstractConnection));
		private readonly object _connectionMutex = new object();
		private readonly object _timerSync = new object();
		private readonly Timer _timer;


		/// <summary>
		/// True if the sender is connected to the messaging provider.
		/// </summary>
		public bool IsConnected { get; private set; }


		/// <summary>
		/// True if the connection has been disposed already.  Calling any methods on this class
		/// after it has been disposed is an error.
		/// </summary>
		public bool IsDisposed { get; private set; }


		/// <summary>
		/// Gets the connection behavior to exhibit if the connection fails.
		/// </summary>
		public ConnectionFailureBehavior ConnectionFailureBehavior { get; private set; }


		/// <summary>
		/// Constructs an instance of the abstract connection class with just the
		/// provider connection details.
		/// </summary>
		/// <param name="connectionFailureBehavior">Dictates behavior if connection fails.</param>
		protected AbstractConnection(ConnectionFailureBehavior connectionFailureBehavior)
		{
			ConnectionFailureBehavior = connectionFailureBehavior;

			if (ConnectionFailureBehavior == ConnectionFailureBehavior.ReconnectOnFailure)
			{
				_timer = new Timer(_reconnectTimeSpan.TotalMilliseconds) { AutoReset = false, Enabled = false };
				_timer.Elapsed += ReconnectTimerElapsed;
			}
		}

		/// <summary>
		/// Attempt to connect to the messaging provider.
		/// </summary>
		public void Connect()
		{
			lock (_connectionMutex)
			{
				if (!IsConnected && !IsDisposed)
				{
					try
					{
						OnConnect();

						// only set is connected to true if no exceptions
						IsConnected = true;
					}
					catch (Exception ex)
					{
						_log.Error("Exception in AbstractConnection.Connect().", ex);

						if (ConnectionFailureBehavior == ConnectionFailureBehavior.ThrowOnFailure)
						{
							throw;
						}

						lock (_timerSync)
						{
							if (_timer != null)
							{
								_timer.Enabled = true;
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Attempt to disconnect from the messaging provider.
		/// </summary>
		public void Disconnect()
		{
			lock (_connectionMutex)
			{
				if (IsConnected && !IsDisposed)
				{
					// even if fails, disconnect it.
					IsConnected = false;

					lock (_timerSync)
					{
						if (_timer != null)
						{
							_timer.Enabled = false;
						}
					}

					try
					{
						OnDisconnect();
					}

					catch (Exception ex)
					{
						_log.Error("Exception in AbstractConnection.Disconnect().", ex);

						if (ConnectionFailureBehavior == ConnectionFailureBehavior.ThrowOnFailure)
						{
							throw;
						}
					}
				}
			}
		}


		/// <summary>
		/// Attempts to disconnect from the feed if currently connected and then reconnect.
		/// </summary>
		public void Reconnect()
		{
			try
			{
				Disconnect();
			}
			catch (Exception)
			{
				// don't care about disconnect exceptions here, just reconnect
			}

			// here, we are concerned about exceptions so let bubble up.
			Connect();
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			lock (_connectionMutex)
			{
				if (!IsDisposed)
				{
					// disconnect from the provider, then cleanup any resources in the subclasses, and then
					// mark disposed so users won't try to use us further.
					Disconnect();

					try
					{
						OnDispose();
					}

					catch (Exception ex)
					{
						_log.Error("Exception in AbstractConnection.Dispose().", ex);						
					}

					IsDisposed = true;
				}
			}
		}


		/// <summary>
		/// The actual method to connect to a messaging provider.  This method is expected to throw
		/// a provider-specific exception if a connection cannot be achieved.
		/// </summary>
		protected abstract void OnConnect();


		/// <summary>
		/// The actual method to disconnect from the messaging provider.  This method is expected to throw
		/// a provider-specific exception if disconnection cannot be achieved.
		/// </summary>
		protected abstract void OnDisconnect();


		/// <summary>
		/// Dispose of all resources held by the provider.  Should be coded in such a way to
		/// be safe from repeated calls.
		/// </summary>
		protected abstract void OnDispose();

	
		/// <summary>
		/// Event handler for timer when timer expires.
		/// </summary>
		/// <param name="sender">Not currently used.</param>
		/// <param name="e">Not currently used.</param>
		private void ReconnectTimerElapsed(object sender, ElapsedEventArgs e)
		{
			_log.Warn("Attempting auto-reconnect.");
			Connect();
		}
	}
}
