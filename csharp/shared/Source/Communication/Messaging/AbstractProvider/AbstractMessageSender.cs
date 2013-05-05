using System;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// The base abstract class for message publishing
	/// </summary>
	public abstract class AbstractMessageSender : AbstractMessagingConnection, IMessageSender
	{
		private ISenderStrategy _senderStrategy;


		/// <summary>
		/// Gets the sender specific properties this sender was created with
		/// </summary>
		public SenderProperties SenderProperties { get; private set; }


		/// <summary>
		/// Gets the number of messages currently queued for publishing.
		/// </summary>
		public long AwaitingProcessingCount
		{
			get { return _senderStrategy.AwaitingProcessingCount; }
		}


		/// <summary>
		/// Gets the number of messages that have been published.
		/// </summary>
		public long ProcessedCount
		{
			get { return _senderStrategy.ProcessedCount; }
		}


		/// <summary>
		/// Constructs the abstract base message sender by storing the appropriate properties.
		/// </summary>
		/// <param name="context">The connection to the provider.</param>
		/// <param name="sender">The properties that represent a message sender.</param>
		/// <param name="asyncErrorHandler">The handler to call when an asynchronous error occurs.</param>
		protected AbstractMessageSender(AbstractMessagingProviderContext context, 
			SenderProperties sender, Action<MessagingException> asyncErrorHandler)
			: base(context, asyncErrorHandler)
		{
			SenderProperties = sender;

			if (sender.IsBuffered)
			{
				_senderStrategy = new BufferedSenderStrategy(Publish, RaiseAsynchronousError);
			}
			else
			{
				_senderStrategy = new UnBufferedSenderStrategy(Publish);
			}
		}


		/// <summary>
		/// Queues a message to be sent on the bus.  The message will be sent as soon as
		/// the processing of all other previous messages is complete.
		/// </summary>
		/// <param name="message">The message to publish on the bus.</param>
		public void Send(Message message)
		{
			if (IsDisposed)
			{
				throw new InvalidOperationException("You cannot send on a sender that has been disposed.");
			}

			if (!IsConnected)
			{
				throw new InvalidOperationException("Cannot publish when not connected.");
			}

			_senderStrategy.Publish(message);
		}


		/// <summary>
		/// The implementation of this method will send the message to the actual provider.  If an error
		/// is encountered, it is expected that this method will throw a provider-specific exception.
		/// </summary>
		/// <param name="message">The message to send to the provider.</param>
		protected abstract void SendToProvider(Message message);


		/// <summary>
		/// Handles the connection to the provider and then branches off for a call for the sender.
		/// </summary>
		protected sealed override void OnConnect()
		{
			// connect to provider and then create sender
			Context.Connect();

			OnStartSender();
		}


		/// <summary>
		/// Stops the connection to the provider
		/// </summary>
		protected sealed override void OnDisconnect()
		{
			// stop the sender and then stop the connection.
			OnStopSender();

			Context.Disconnect();
		}


		/// <summary>
		/// Disposes of the connection to the provider and any other resources
		/// </summary>
		protected sealed override void OnDispose()
		{
			// dispose the sender strategy
			if (_senderStrategy != null)
			{
				_senderStrategy.Dispose();
				_senderStrategy = null;
			}

			// and dispose any sender resources
			OnDisposeSender();

			// then dispose connection resources
			Context.Dispose();
		}


		/// <summary>
		/// Abstract method to override to start the sender on the connection.
		/// </summary>
		protected abstract void OnStartSender();


		/// <summary>
		/// Abstract method to override to stop the sender on the connection.
		/// </summary>
		protected abstract void OnStopSender();

		
		/// <summary>
		/// Abstract method to override to dispose of the sender.
		/// </summary>
		protected abstract void OnDisposeSender();


		/// <summary>
		/// Performs the publishing for the strategy, this just calls the provider specific publish and wraps any exceptions.
		/// </summary>
		/// <param name="message">The message to publish.</param>
		private void Publish(Message message)
		{
			try
			{
				SendToProvider(message);
			}
			catch (MessagingException ex)
			{
				ThrowError("Could not Publish().", ex);
			}
			catch (Exception ex)
			{
				ThrowError("Could not Publish().", ex);
			}
		}
	}
}
