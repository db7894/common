using System;
using System.Threading;
using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// The MockReceiver is the mock provider implementation of a receiver.
	/// </summary>
	public class MockReceiver : AbstractMessageReceiver
	{
		/// <summary>
		/// Index of last consumed message 
		/// </summary>
		public static int LastConsumed = 0;

		/// <summary>
		/// Gets the mock results that can be set for the mock receiver
		/// </summary>
		public static MockReceiverMessageQueue MockResults { get; private set; }

		/// <summary>
		/// static constructor for mock results
		/// </summary>
		static MockReceiver()
		{
			MockResults = new MockReceiverMessageQueue();
		}
		

		/// <summary>
		/// Creates a MockReceiver that fakes out a real receiver.
		/// </summary>
		/// <param name="properties">The provider properties.</param>
		/// <param name="receiver">The receiver properties.</param>
		/// <param name="newMessageAction">The action to take on new messages.</param>
		/// <param name="asyncErrorHandler">The handler for asynchronous errors.</param>
		public MockReceiver(ProviderProperties properties, ReceiverProperties receiver, 
			Action<Message> newMessageAction, Action<MessagingException> asyncErrorHandler)
			: base(new MockContext(properties), receiver, newMessageAction, asyncErrorHandler)
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
		/// Abstract method to override to start the receiver on the connection.
		/// </summary>
		protected override void OnStartReceiver()
		{
			if (MockResults[Provider, ReceiverProperties].ShouldThrowOnReceiverStart)
			{
				throw new MockProviderException("Failed to start receiver.");
			}

			if (MockResults[Provider, ReceiverProperties].ShouldThrowOnReceive)
			{
				RaiseAsynchronousError(
					new MessagingException("Asynchronous read error.", 
						new MockProviderException("Could not read.")));
			}

			// if not synchronous, just replay the messages
			if (!IsSynchronous)
			{
				foreach (var message in MockResults[Provider, ReceiverProperties].MessagesToReceive)
				{
					OnMessageReceived(message);
				}
			}
		}


		/// <summary>
		/// Abstract method to override to stop the receiver on the connection.
		/// </summary>
		protected override void OnStopReceiver()
		{
			if (MockResults[Provider, ReceiverProperties].ShouldThrowOnReceiverStop)
			{
				throw new MockProviderException("Failed to stop receiver.");
			}
		}


		/// <summary>
		/// Abstract method to override to dispose of the receiver.
		/// </summary>
		protected override void OnDisposeReceiver()
		{
		}


		/// <summary>
		/// Attempts to read the message synchronously 
		/// </summary>
		/// <param name="timeout">Time to wait for a new message to arrive.</param>
		/// <returns>The new message.</returns>
		protected override Message OnSynchronousRead(TimeSpan timeout)
		{
			if (LastConsumed < MockResults[Provider, ReceiverProperties].MessagesToReceive.Count)
			{
				return MockResults[Provider, ReceiverProperties].MessagesToReceive[LastConsumed++];
			}
			
			if (timeout.TotalMilliseconds >= 0)
			{
				Thread.Sleep(timeout);
			}

			return null;
		}
	}
}
