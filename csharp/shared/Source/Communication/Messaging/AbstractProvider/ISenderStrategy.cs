using System;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// Interface that represents a strategy for message sender where the messages are not published 
	/// immediately, but are instead enqueued and then a separate process will dequeued them for handling.
	/// </summary>
	internal interface ISenderStrategy : IDisposable
	{
		/// <summary>
		/// Gets the number of messages processed for sender.
		/// </summary>
		long ProcessedCount { get; }

		/// <summary>
		/// Gets the number of messages that have been set for sender but not yet processed.
		/// </summary>
		long AwaitingProcessingCount { get; }

		/// <summary>
		/// Publishes a message according to the publish strategy.
		/// </summary>
		/// <param name="messageToPublish">The message to publish.</param>
		void Publish(Message messageToPublish);
	}
}
