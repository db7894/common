using System;
using System.Diagnostics;
using System.Threading;

namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// Implementation of the <see cref="IMessageSender"/> to publish statistics
	/// about the sender.
	/// </summary>
	internal class StatisticsSender : IMessageSender
	{
		private const char _separator = (char)0x01;
		private const long _timerInterval = 3000;
		private const long _startTimer = 0;
		private const long _stopTimer = -1;

		private readonly IMessageSender _wrappedSender;
		private readonly IMessageReceiver _monitoredReceiver;
		private readonly string _machineName;
		private readonly int _processId;
		private int _lastStatisticsSequenceNumber = 0;
		private Timer _statsTimer;

		/// <summary>
		/// Gets the properties for this sender.
		/// </summary>
		public SenderProperties SenderProperties
		{
			get { return _wrappedSender.SenderProperties; }
		}

		/// <summary>
		/// True if the sender is connected to the messaging provider.
		/// </summary>
		public bool IsConnected
		{
			get { return _wrappedSender.IsConnected; }
		}

		/// <summary>
		/// True if this sender has been disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return _wrappedSender.IsDisposed;  }
		}

		/// <summary>
		/// The provider properties this receiver was created with.
		/// </summary>
		public ProviderProperties Provider
		{
			get { return _wrappedSender.Provider; }
		}

		/// <summary>
		/// The number of asynchronous errors received from the provider.
		/// </summary>
		public long ErrorCount
		{
			get { return _wrappedSender.ErrorCount; }
		}

		/// <summary>
		/// The last error message received from the provider.
		/// </summary>
		public string LastError
		{
			get { return _wrappedSender.LastError; }
		}

		/// <summary>
		/// Gets the host currently connected to.
		/// </summary>
		public string ConnectedHost
		{
			get { return _wrappedSender.ConnectedHost; }
		}

		/// <summary>
		/// Gets the number of messages currently waiting to be published.  
		/// </summary>
		public long AwaitingProcessingCount
		{
			get { return _wrappedSender.AwaitingProcessingCount; }
		}

		/// <summary>
		/// Gets the number of messages that have been published.
		/// </summary>
		public long ProcessedCount
		{ 
			get { return _wrappedSender.ProcessedCount;  } 
		}

		/// <summary>
		/// Wraps a sender with a stats sender to publish statistics at regular intervals.
		/// </summary>
		/// <param name="wrappedSender">The sender to utilize to publish statistics.</param>
		/// <param name="monitoredReceiver">The receiver to monitor statistics on.</param>
		public StatisticsSender(IMessageSender wrappedSender, IMessageReceiver monitoredReceiver)
		{
			_monitoredReceiver = monitoredReceiver;
			_wrappedSender = wrappedSender;
			_machineName = Environment.MachineName;
			_processId = Process.GetCurrentProcess().Id;
			_statsTimer = new Timer(OnTimerElapsed, null, _startTimer, _timerInterval);
		}


		/// <summary>
		/// Attempt to connect to the messaging provider.
		/// </summary>
		public void Connect()
		{
			_wrappedSender.Connect();
		}


		/// <summary>
		/// Attempt to disconnect from the messaging provider.
		/// </summary>
		public void Disconnect()
		{
			_wrappedSender.Disconnect();
		}


		/// <summary>
		/// Queues a message to be sent on the bus.  The message will be sent as soon as
		/// the processing of all other previous messages is complete.
		/// </summary>
		/// <param name="message">The message to publish on the bus.</param>
		public void Send(Message message)
		{
			_wrappedSender.Send(message);
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			_statsTimer.Change(_stopTimer, _timerInterval);
			_wrappedSender.Dispose();
		}


		/// <summary>
		/// Callback when timer event expires
		/// </summary>
		/// <param name="state">This parameter is not used</param>
		private void OnTimerElapsed(object state)
		{
			if (IsConnected)
			{
				Send(CreateStatistics());
			}
		}


		/// <summary>
		/// Helper method that creates a new statistics message based on the monitored receiver.
		/// </summary>
		/// <returns>A message containing the statistics.</returns>
		private Message CreateStatistics()
		{
			DateTime now = DateTime.Now;

			string result =
				string.Format(
					"SERVER={1}{0}MESSAGES={2}{0}PID={3}{0}TIME=" + "{4:00}{5:00}{6:00}{0}" + 
						"SEQ={7}{0}QUEUE={8}{0}EXCEPTION={9}{0}",
					_separator,
					_machineName,
					_monitoredReceiver.ProcessedCount,
					_processId,
					now.Month,
					now.Day,
					now.Year,
					++_lastStatisticsSequenceNumber,
					_monitoredReceiver.AwaitingProcessingCount,
					_monitoredReceiver.LastError);

			return new Message(result);
		}
	}
}
