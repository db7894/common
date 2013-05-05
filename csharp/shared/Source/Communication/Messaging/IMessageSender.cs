namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Interface for publishing messages in a provider-independent way.
	/// </summary>
	public interface IMessageSender : IMessagingConnection
	{
		/// <summary>
		/// Gets the properties for this sender.
		/// </summary>
		SenderProperties SenderProperties { get; }


		/// <summary>
		/// Gets the number of messages currently waiting to be published.
		/// </summary>
		long AwaitingProcessingCount { get; }


		/// <summary>
		/// Gets the number of messages that have been published.
		/// </summary>
		long ProcessedCount { get; }


		/// <summary>
		/// Queues a message to be sent on the bus.  The message will be sent as soon as
		/// the processing of all other previous messages is complete.
		/// </summary>
		/// <param name="message">The message to publish on the bus.</param>
		void Send(Message message);
	}
}
