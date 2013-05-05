using System;
using log4net;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// The abstract class that represents the basic functionality of all messaging receivers.
	/// </summary>
	public abstract class AbstractMessageReceiver : 
		AbstractMessagingConnection, 
		IMessageReceiver,
		IMessageReader
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(AbstractMessageReceiver));
		private IMessageHandlerStrategy _handlerStrategy;
		private StatisticsSender _statsSender;

		/// <summary>
		/// Gets the receiver specific properties this receiver was created with
		/// </summary>
		public ReceiverProperties ReceiverProperties { get; private set; }


		/// <summary>
		/// Gets the number of messages received but not yet processed.
		/// </summary>
		public long AwaitingProcessingCount
		{
			get { return _handlerStrategy.AwaitingProcessingCount; }
		}


		/// <summary>
		/// Gets the number of messages completely processed.
		/// </summary>
		public long ProcessedCount
		{
			get { return _handlerStrategy.ProcessedCount;  }
		}


		/// <summary>
		/// Returns true if the receiver is synchronous.
		/// </summary>
		public bool IsSynchronous { get; private set; }


		/// <summary>
		/// Constructs the abstract base message sender by storing the appropriate properties.
		/// </summary>
		/// <param name="context">The connection to the provider.</param>
		/// <param name="receiver">The properties that represent a message sender.</param>
		/// <param name="newMessageAction">Action to take when new message is received.</param>
		/// <param name="asyncErrorHandler">Action to take when an asynchronous error is received.</param>
		protected AbstractMessageReceiver(AbstractMessagingProviderContext context, 
			ReceiverProperties receiver, Action<Message> newMessageAction,
			Action<MessagingException> asyncErrorHandler)
			: base(context, asyncErrorHandler)
		{
			ReceiverProperties = receiver;

			// this is a synchronous-reader if the newMessageAction is null.
			IsSynchronous = (newMessageAction == null);

			if (!IsSynchronous && ReceiverProperties.IsBuffered)
			{
				_handlerStrategy = new BufferedMessageHandlerStrategy(newMessageAction,
					RaiseAsynchronousError);
			}
			else
			{
				_handlerStrategy = new UnBufferedMessageHandlerStrategy(newMessageAction);
			}

			// create a statistics sender if the stats topic is not empty
			_statsSender = CreateStatisticsSender(ReceiverProperties.StatisticsDestination);
		}


		/// <summary>
		/// Reads the next message off of the queue, waits infinitely for a new message to arrive.
		/// </summary>
		/// <returns>The new message.</returns>
		public Message Read()
		{
			// read with an infinite timeout span
			return Read(TimeSpan.FromMilliseconds(-1));
		}


		/// <summary>
		/// Reads the next message off of the queue for up to the timeout specified.  If no message arrives
		/// by the timeout specified will return null.
		/// </summary>
		/// <param name="timeout">The time to wait for a new message before timing out.</param>
		/// <returns>The new message.</returns>
		public Message Read(TimeSpan timeout)
		{
			Message result = null;

			// can't do a synchronous read if we're an asynchronous reader
			if (!IsSynchronous)
			{
				throw new InvalidOperationException(
					"Synchronous read attempted from asynchronous receiver.");
			}

			try
			{
				// read from the provider for the time specified
				result = OnSynchronousRead(timeout);
				_handlerStrategy.OnMessageReceived(result);
			}
			catch (MessagingException ex)
			{
				ThrowError("Error reading message from reader.", ex);
			}
			catch (Exception ex)
			{
				ThrowError("Error reading message from reader.", ex);
			}

			return result;
		}


		/// <summary>
		/// Handles the connection to the provider and then branches off for a call for the receiver.
		/// </summary>
		protected sealed override void OnConnect()
		{
			// connect to provider and then create receiver and stats connection (if any)
			Context.Connect();

			OnStartReceiver();

			// create the sender for stats if specified
			if (_statsSender != null)
			{
				_statsSender.Connect();
			}
		}


		/// <summary>
		/// Stops the connection to the provider
		/// </summary>
		protected sealed override void OnDisconnect()
		{
			// bring down the stats auto-sender first
			if (_statsSender != null)
			{
				_statsSender.Disconnect();
			}

			// stop the receiver and then stop the connection.
			OnStopReceiver();

			Context.Disconnect();
		}


		/// <summary>
		/// Disposes of the connection to the provider and any other resources
		/// </summary>
		protected sealed override void OnDispose()
		{
			// dispose the stats auto-sender resources
			if (_statsSender != null)
			{
				_statsSender.Dispose();
				_statsSender = null;
			}

			// dispose the receiver strategy
			if (_handlerStrategy != null)
			{
				_handlerStrategy.Dispose();
				_handlerStrategy = null;
			}

			// and dispose any receiver resources
			OnDisposeReceiver();

			// then dispose connection resources
			Context.Dispose();
		}


		/// <summary>
		/// Abstract method to override to start the receiver on the connection.
		/// </summary>
		protected abstract void OnStartReceiver();


		/// <summary>
		/// Abstract method to override to stop the receiver on the connection.
		/// </summary>
		protected abstract void OnStopReceiver();


		/// <summary>
		/// Abstract method a provider must override to specify how to read a message asynchronously.
		/// This method is expected to throw provider-specified errors if an error occurs or return null if
		/// the timeout expires.
		/// </summary>
		/// <param name="timeout">The time to wait for a message before returning null if none arrives.</param>
		/// <returns>The new message.</returns>
		protected abstract Message OnSynchronousRead(TimeSpan timeout);


		/// <summary>
		/// Abstract method to override to dispose of the receiver.
		/// </summary>
		protected abstract void OnDisposeReceiver();


		/// <summary>
		/// Method to call when a new message has been received from the provider.
		/// </summary>
		/// <param name="newMessage">The new message that was received.</param>
		protected void OnMessageReceived(Message newMessage)
		{
			_handlerStrategy.OnMessageReceived(newMessage);
		}


		/// <summary>
		/// Attempt to create a new sender for statistics on the receiver's consumption rates.
		/// </summary>
		/// <param name="destination">The stats topic to publish to.</param>
		/// <returns>The sender for the statistics.</returns>
		private StatisticsSender CreateStatisticsSender(string destination)
		{
			StatisticsSender statsSender = null;

			try
			{
				if (destination.IsNotNullOrEmpty())
				{
					var statsSenderProperties = new SenderProperties(
						SenderType.Publisher, destination, false, false);

					var underlyingSender = MessagingFactory.CreateSender(Provider, 
						statsSenderProperties);

					if (underlyingSender != null)
					{
						statsSender = new StatisticsSender(underlyingSender, this);
					}
				}
			}

			catch (Exception ex)
			{
				// catch and consume -- while we should report this error, the failure of
				// creating a stats topic should not stop the rest of our processing.
				_log.Warn("Could not create statistics sender for receiver.", ex);
			}

			return statsSender;
		}
	}
}
