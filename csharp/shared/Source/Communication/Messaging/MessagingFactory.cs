using System;
using SharedAssemblies.Communication.Messaging.FioranoProvider;
using SharedAssemblies.Communication.Messaging.MockProvider;
using SharedAssemblies.Communication.Messaging.NullProvider;


namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Factory that creates instances of senders and receivers for messaging.
	/// </summary>
	public static class MessagingFactory
	{
		/// <summary>
		/// Constructs a sender for the specified messaging provider.  Optionally, you can specify a handler for asynchronous
		/// errors (like provider drops).  If this is null, the default handler will log the exceptions to log4net.
		/// </summary>
		/// <param name="provider">The properties of the messaging host.</param>
		/// <param name="sender">The sender details for the sender.</param>
		/// <param name="asyncErrorHandler">A handler for any asynchronous errors from the provider.</param>
		/// <returns>An instance to a message sender.</returns>
		public static IMessageSender CreateSender(ProviderProperties provider, 
			SenderProperties sender, Action<MessagingException> asyncErrorHandler)
		{
			// properties for the host must be filled in
			if (provider == null || sender == null)
			{
				throw new ArgumentNullException(provider == null ? "provider" : "sender");
			}

			// construct appropriate producer
			switch (provider.ProviderType)
			{
				case ProviderType.Fiorano:
					return new FioranoSender(provider, sender, asyncErrorHandler);

				case ProviderType.Mock:
					return new MockSender(provider, sender, asyncErrorHandler);

				case ProviderType.Null:
					return new NullSender(provider, sender, asyncErrorHandler);					

				default:
					throw new ArgumentOutOfRangeException("provider");
			}
		}

		/// <summary>
		/// Constructs a sender for the specified messaging provider.  This form uses the default asynchronous exception
		/// handler which simply logs the exceptions to log4net.
		/// </summary>
		/// <param name="provider">The properties of the messaging host.</param>
		/// <param name="sender">The sender details for the sender.</param>
		/// <returns>An instance to a message sender.</returns>
		public static IMessageSender CreateSender(ProviderProperties provider,
			SenderProperties sender)
		{
			return CreateSender(provider, sender, null);
		}


		/// <summary>
		/// Creates an asynchronous receiver for the messaging provider specified.  This receiver will receive new messages and
		/// call the messageHandler delegate.  Asynchronous errors can optionally be handled by the asynchErrorHandler if
		/// specified.  
		/// </summary>
		/// <param name="provider">The properties of the messaging host.</param>
		/// <param name="receiver">The receiver details for the receiver.</param>
		/// <param name="messageHandler">The delegate to be called when a new message arrives.</param>
		/// <param name="asyncErrorHandler">A handler for any asynchronous errors from the provider.</param>
		/// <returns>An instance to a message receiver.</returns>
		public static IMessageReceiver CreateReceiver(ProviderProperties provider,
			ReceiverProperties receiver, Action<Message> messageHandler,
			Action<MessagingException> asyncErrorHandler)
		{
			// check parameters to make sure valid.
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}

			if (receiver == null)
			{
				throw new ArgumentNullException("receiver");
			}

			if (messageHandler == null)
			{
				throw new ArgumentNullException("messageHandler");
			}

			// construct appropriate producer
			switch (provider.ProviderType)
			{
				case ProviderType.Fiorano:
					return new FioranoReceiver(provider, receiver, messageHandler, asyncErrorHandler);

				case ProviderType.Mock:
					return new MockReceiver(provider, receiver, messageHandler, asyncErrorHandler);

				case ProviderType.Null:
					return new NullReceiver(provider, receiver, messageHandler, asyncErrorHandler);

				default:
					throw new ArgumentOutOfRangeException("provider");
			}
		}


		/// <summary>
		/// Creates an asynchronous  receiver for the messaging provider specified.  This receiver will receive new messages and
		/// call the messageHandler delegate.  
		/// </summary>
		/// <param name="provider">The properties of the messaging host.</param>
		/// <param name="receiver">The receiver details for the receiver.</param>
		/// <param name="messageHandler">The delegate to be called when a new message arrives.</param>
		/// <returns>An instance to a message receiver.</returns>
		public static IMessageReceiver CreateReceiver(ProviderProperties provider,
			ReceiverProperties receiver, Action<Message> messageHandler)
		{
			return CreateReceiver(provider, receiver, messageHandler, null);
		}


		/// <summary>
		/// Creates a synchronous reader for the messaging provider specified.  The reader must be called explicitly to consume
		/// a new message and takes on all responsibility for consuming messages in a timely manner.  
		/// </summary>
		/// <param name="provider">The properties of the messaging host.</param>
		/// <param name="receiver">The receiver details for the receiver.</param>
		/// <param name="asyncErrorHandler">A handler for any asynchronous errors from the provider.</param>
		/// <returns>An instance to a message receiver.</returns>
		public static IMessageReader CreateReader(ProviderProperties provider,
			ReceiverProperties receiver, Action<MessagingException> asyncErrorHandler)
		{
			// check parameters to make sure valid.
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}

			if (receiver == null)
			{
				throw new ArgumentNullException("receiver");
			}

			if (receiver.IsBuffered)
			{
				throw new ArgumentException("IsBuffered must be false for synchronous readers.");
			}

			// construct appropriate producer
			switch (provider.ProviderType)
			{
				case ProviderType.Fiorano:
					return new FioranoReceiver(provider, receiver, null, asyncErrorHandler);

				case ProviderType.Mock:
					return new MockReceiver(provider, receiver, null, asyncErrorHandler);

				case ProviderType.Null:
					return new NullReceiver(provider, receiver, null, asyncErrorHandler);

				default:
					throw new ArgumentOutOfRangeException("provider");
			}
		}


		/// <summary>
		/// Creates a synchronous reader for the messaging provider specified.  The reader must be called explicitly to consume
		/// a new message and takes on all responsibility for consuming messages in a timely manner.  
		/// </summary>
		/// <param name="provider">The properties of the messaging host.</param>
		/// <param name="receiver">The receiver details for the receiver.</param>
		/// <returns>An instance to a message receiver.</returns>
		public static IMessageReader CreateReader(ProviderProperties provider,
			ReceiverProperties receiver)
		{
			return CreateReader(provider, receiver, null);
		}
	}
}
