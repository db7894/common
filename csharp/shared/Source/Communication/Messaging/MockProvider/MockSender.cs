using System;
using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// Creates a mock sender that can be used to test code w/o needing a messaging provider present.
	/// </summary>
	internal class MockSender : AbstractMessageSender	
	{
		/// <summary>
		/// Gets the queue of messages that have been received by mock senders.
		/// </summary>
		public static MockSenderMessageQueue MockResults { get; private set; }		


		/// <summary>
		/// static constructor for mock results
		/// </summary>
		static MockSender()
		{
			MockResults = new MockSenderMessageQueue();
		}
		

		/// <summary>																								 
		/// Instantiate the mock sender.
		/// </summary>
		/// <param name="properties">The provider properties.</param>
		/// <param name="sender">The sender information.</param>
		/// <param name="asyncErrorHandler">The error handler.</param>
		public MockSender(ProviderProperties properties, SenderProperties sender, 
			Action<MessagingException> asyncErrorHandler)
			: base(new MockContext(properties), sender, asyncErrorHandler)
		{
		}


		/// <summary>
		/// Resets the mock results
		/// </summary>
		public static void ResetMockResults()
		{
			MockResults.Clear();
		}


		/// <summary>
		/// The implementation of this method will send the message to the actual provider.  If an error
		/// is encountered, it is expected that this method will throw a provider-specific exception.
		/// </summary>
		/// <param name="message">The message to send to the provider.</param>
		protected override void SendToProvider(Message message)
		{
			if (MockResults[Provider, SenderProperties].ShouldThrowOnSend)
			{
				throw new MockProviderException("Could not send message.");
			}

			MockResults[Provider, SenderProperties].SentMessages.Add(message);
		}


		/// <summary>
		/// Abstract method to override to start the sender on the connection.
		/// </summary>
		protected override void OnStartSender()
		{
			if (MockResults[Provider, SenderProperties].ShouldThrowOnSenderStart)
			{
				throw new MockProviderException("Could not start sender.");
			}
		}


		/// <summary>
		/// Abstract method to override to stop the sender on the connection.
		/// </summary>
		protected override void OnStopSender()
		{
			if (MockResults[Provider, SenderProperties].ShouldThrowOnSenderStop)
			{
				throw new MockProviderException("Could not stop sender.");
			}
		}


		/// <summary>
		/// Abstract method to override to dispose of the sender.
		/// </summary>
		protected override void OnDisposeSender()
		{
		}
	}
}
