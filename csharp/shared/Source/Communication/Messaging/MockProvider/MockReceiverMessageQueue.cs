using System.Collections.Generic;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// Defines a message queue for messages that can be enqueued for testing receivers
	/// </summary>
	public class MockReceiverMessageQueue : MockMessagingMap<MockReceiverBehavior>
	{
		/// <summary>
		/// Indexer that allows access to the receiver queue based on the receiver's properties.
		/// </summary>
		/// <param name="provider">The host provider properties.</param>
		/// <param name="receiver">The receiver properties.</param>
		/// <returns>The queue of messages for receiver consumption.</returns>
		public MockReceiverBehavior this[ProviderProperties provider, ReceiverProperties receiver]
		{
			get
			{
				return this[provider.Hosts[0], receiver.Source];
			}
		}
	}
}
