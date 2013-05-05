using System;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// Receiver message handling strategy interface
	/// </summary>
	internal interface IMessageHandlerStrategy : IDisposable
	{
		/// <summary>
		/// Gets the number of messages received and already processed since start.
		/// </summary>
		long ProcessedCount { get; }

		/// <summary>
		/// Gets the number of messages received but not yet processed since start.
		/// </summary>
		long AwaitingProcessingCount { get; }

		/// <summary>
		/// This method is called on a message handler whenever a new message
		/// has been received from the receiver.
		/// </summary>
		/// <param name="receivedMessage">The message that was received.</param>
		void OnMessageReceived(Message receivedMessage);
	}
}
