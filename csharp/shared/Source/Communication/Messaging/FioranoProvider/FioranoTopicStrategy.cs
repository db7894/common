using System;
using fiorano.csharp.fms;
using log4net;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// Publishes messages to a Fiorano topic.
	/// </summary>
	internal class FioranoTopicStrategy : IFioranoStrategy
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(FioranoTopicStrategy));
		private MessageProducer _sender;
		private MessageConsumer _consumer;
		private TopicConnection _connection;
		private TopicSession _session;
		private Topic _topic;


		/// <summary>
		/// Instantiates the topic sender strategy.
		/// </summary>
		/// <param name="sender">The sender details.</param>
		public FioranoTopicStrategy(FioranoSender sender)
		{
			CreateConnection(sender.FioranoContext, sender.SenderProperties.Destination);
			CreateSender(sender);
		}


		/// <summary>
		/// Instantiates the topic receiver strategy
		/// </summary>
		/// <param name="receiver">The receiver information.</param>
		/// <param name="newMessageHandler">The handler for new messages.</param>
		public FioranoTopicStrategy(FioranoReceiver receiver, Action<Message> newMessageHandler)
		{
			CreateConnection(receiver.FioranoContext, receiver.ReceiverProperties.Source);
			CreateReceiver(receiver, newMessageHandler);
		}


		/// <summary>
		/// Sends the message on the topic.
		/// </summary>
		/// <param name="message">The message to publish</param>
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

			// based on the TimeSpan, either wait infinitely (if < 0), for specified
			// time (if > 0) or no-wait (if == 0)
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
		/// Cleans up all resources for the topic sender
		/// </summary>
		public void Dispose()
		{
			// close all resources if they were created.
			if (_sender != null)
			{
				_log.Info("Closing topic publisher.");
				_sender.close();
			}

			if (_consumer != null)
			{
				_log.Info("Closing topic subscriber.");
				_consumer.close();
			}

			if (_session != null)
			{
				_log.Info("Closing topic session.");
				_session.close();
			}

			if (_connection != null)
			{
				_log.Info("Closing topic connection.");
				_connection.close();
			}
		}


		/// <summary>
		/// Creates the sender to the topic.
		/// </summary>
		/// <param name="context">The Fiorano context.</param>
		/// <param name="destination">The destination to create a connection to.</param>
		private void CreateConnection(FioranoContext context, string destination)
		{
			// create the topic and topic connection
			_log.Info("Looking up topic: " + destination);
			_topic = (Topic)context.NamingContext.lookup(destination);

			_log.Info("Creating connection to topic: " + destination);
			_connection = context.TopicFactory.createTopicConnection(
				context.Provider.UserName, context.Provider.Password);
		}


		/// <summary>
		/// Creates a sender to the topic
		/// </summary>
		/// <param name="sender">The sender connection details.</param>
		private void CreateSender(FioranoSender sender)
		{
			// then create the session and sender
			_log.Info("Creating topic session and publisher.");
			_connection.setExceptionListener(new FioranoExceptionListener(sender.RaiseAsynchronousError));
			_session = _connection.createTopicSession(false, SessionConstants.AUTO_ACKNOWLEDGE);

			// create the sender and set the delivery mode
			_sender = _session.createPublisher(_topic);
			_sender.setDeliveryMode(sender.SenderProperties.IsPersistent
				? DeliveryMode.PERSISTENT : DeliveryMode.NON_PERSISTENT);

			_log.Info("Starting topic connection.");
			_connection.start();
		}

	
		/// <summary>
		/// Creates a receiver to the topic
		/// </summary>
		/// <param name="receiver">The receiver connection details.</param>
		/// <param name="newMessageHandler">The new message handler delegate.</param>
		private void CreateReceiver(FioranoReceiver receiver, Action<Message> newMessageHandler)
		{
			// then create the session and sender
			_log.Info("Creating topic session and subscriber.");
			_connection.setExceptionListener(new FioranoExceptionListener(receiver.RaiseAsynchronousError));
			_session = _connection.createTopicSession(false, SessionConstants.AUTO_ACKNOWLEDGE);
			_consumer = _session.createSubscriber(_topic);

			if (newMessageHandler != null)
			{
				_log.Info("Creating asynchronous topic listener.");
				_consumer.setMessageListener(new FioranoMessageListener(newMessageHandler));
			}

			_log.Info("Starting topic connection.");
			_connection.start();
		}
	}
}
