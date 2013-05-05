using System;
using fiorano.csharp.fms;
using log4net;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// Publishes messages to a Fiorano queue.
	/// </summary>
	internal class FioranoQueueStrategy : IFioranoStrategy
	{
		private static ILog _log = LogManager.GetLogger(typeof(FioranoQueueStrategy));
		private MessageProducer _sender;
		private MessageConsumer _consumer;
		private QueueConnection _connection;
		private QueueSession _session;
		private Queue _queue;


		/// <summary>
		/// Instantiates the queue sender strategy.
		/// </summary>
		/// <param name="sender">The sender details.</param>
		public FioranoQueueStrategy(FioranoSender sender)
		{
			CreateConnection(sender.FioranoContext, sender.SenderProperties.Destination);
			CreateSender(sender);
		}


		/// <summary>
		/// Instantiates the queue receiver strategy
		/// </summary>
		/// <param name="receiver">The receiver information.</param>
		/// <param name="newMessageHandler">The handler for new messages.</param>
		public FioranoQueueStrategy(FioranoReceiver receiver, Action<Message> newMessageHandler)
		{
			CreateConnection(receiver.FioranoContext, receiver.ReceiverProperties.Source);
			CreateReceiver(receiver, newMessageHandler);
		}


		/// <summary>
		/// Sends the message on the queue.
		/// </summary>
		/// <param name="message">The message to send.</param>
		public void Send(Message message)
		{
			_sender.send(FioranoMessageAdapter.Convert(message, _session));
		}


		/// <summary>
		/// Reads a message from the queue and returns it or null if the TimeSpan expires before a new message arrives.
		/// </summary>
		/// <param name="timeout">The time to wait for a new message to arrive.</param>
		/// <returns>The new message or null if none arrives before the TimeSpan expires.</returns>
		public Message Read(TimeSpan timeout)
		{
			TextMessage receivedMessage = null;

			long totalMillisecondsToWait = (long)timeout.TotalMilliseconds;

			// based on the TimeSpan, either wait infinitely (if < 0),
			// for specified time (if > 0) or no-wait (if == 0)
			if (totalMillisecondsToWait < 0)
			{
				receivedMessage = (TextMessage)_consumer.receive();
			}
			else if (totalMillisecondsToWait > 0)
			{
				receivedMessage = (TextMessage)_consumer.receive(totalMillisecondsToWait);
			}
			else
			{
				receivedMessage = (TextMessage)_consumer.receiveNoWait();
			}

			// if got a message and it's a text message, convert to generic message
			if (receivedMessage != null)
			{
				return FioranoMessageAdapter.Convert(receivedMessage);
			}

			// otherwise, we got nothing.
			return null;
		}


		/// <summary>
		/// Cleans up all resources for the queue sender
		/// </summary>
		public void Dispose()
		{
			// close all resources if they were created.
			if (_sender != null)
			{
				_log.Info("Closing queue producer.");
				_sender.close();
			}

			if (_consumer != null)
			{
				_log.Info("Closing queue consumer.");
				_consumer.close();
			}

			if (_session != null)
			{
				_log.Info("Closing queue session.");
				_session.close();
			}

			if (_connection != null)
			{
				_log.Info("Closing queue connection.");
				_connection.close();
			}
		}


		/// <summary>
		/// Creates the sender to the queue.
		/// </summary>
		/// <param name="context">The Fiorano context.</param>
		/// <param name="destination">The destination to create a connection to.</param>
		private void CreateConnection(FioranoContext context, string destination)
		{
			// create the queue and queue connection
			_log.Info("Looking up queue: " + destination);
			_queue = (Queue)context.NamingContext.lookup(destination);

			_log.Info("Creating connection to queue: " + destination);
			_connection = context.QueueFactory.createQueueConnection(
				context.Provider.UserName, context.Provider.Password);
		}


		/// <summary>
		/// Creates a sender to the queue
		/// </summary>
		/// <param name="sender">The sender connection details.</param>
		private void CreateSender(FioranoSender sender)
		{
			// then create the session and sender
			_log.Info("Creating queue session and sender.");
			_connection.setExceptionListener(new FioranoExceptionListener(sender.RaiseAsynchronousError));
			_session = _connection.createQueueSession(false, SessionConstants.AUTO_ACKNOWLEDGE);

			// create sender and set the delivery mode
			_sender = _session.createSender(_queue);
			_sender.setDeliveryMode(sender.SenderProperties.IsPersistent ?
				DeliveryMode.PERSISTENT : DeliveryMode.NON_PERSISTENT);

			_log.Info("Starting connection.");
			_connection.start();
		}


		/// <summary>
		/// Creates a receiver to the queue
		/// </summary>
		/// <param name="receiver">The receiver connection details.</param>
		/// <param name="newMessageHandler">The new message handler delegate.</param>
		private void CreateReceiver(FioranoReceiver receiver, Action<Message> newMessageHandler)
		{
			// then create the session and sender
			_log.Info("Creating queue session and receiver.");
			_connection.setExceptionListener(new FioranoExceptionListener(receiver.RaiseAsynchronousError));
			_session = _connection.createQueueSession(false, SessionConstants.AUTO_ACKNOWLEDGE);
			_consumer = _session.createConsumer(_queue);

			// if new message handler is null, synchronous reads only are intended.
			if (newMessageHandler != null)
			{
				_log.Info("Creating asynchronous queue listener.");
				_consumer.setMessageListener(new FioranoMessageListener(newMessageHandler));
			}

			_log.Info("Starting queue connection.");
			_connection.start();
		}
	}
}
