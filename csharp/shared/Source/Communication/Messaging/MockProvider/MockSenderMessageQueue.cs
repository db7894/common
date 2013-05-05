using System.Collections.Generic;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// Defines a message queue for messages produced by a sender.
	/// </summary>
	public class MockSenderMessageQueue : MockMessagingMap<MockSenderBehavior>
	{
		/// <summary>
		/// Convenience indexer that allows you to specify provider and sender properties instead of names.
		/// </summary>
		/// <param name="provider">The properties used to create the provider.</param>
		/// <param name="sender">The properties used to create the sender.</param>
		/// <returns>A queue of messages.</returns>
		public MockSenderBehavior this[ProviderProperties provider, SenderProperties sender]
		{
			get
			{
				return this[provider.Hosts[0], sender.Destination];
			}
		}
	}
}
