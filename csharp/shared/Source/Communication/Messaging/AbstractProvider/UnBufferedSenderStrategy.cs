using System;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// Receives messages into a buffer which is used to decouple the messages from the bus so that
	/// slow-downs in handling the message do not delay in taking messages off the bus.  The down-side to this is
	/// if the client does not consume messages fast enough, the buffer could consume a lot of memory.
	/// </summary>
	internal class UnBufferedSenderStrategy : ISenderStrategy
	{
		private readonly Action<Message> _publishHandler;


		/// <summary>
		/// Gets the number of messages that have been published.
		/// </summary>
		public long ProcessedCount { get; private set; }


		/// <summary>
		/// Gets the number of messages awaiting sender, in our case this is zero since there is no buffering.
		/// </summary>
		public long AwaitingProcessingCount
		{
			get { return 0; }
		}


		/// <summary>
		/// Constructs a buffered message handler which will receive all messages into a buffer and 
		/// then call the handler indicated to process them from the buffer instead of straight off the bus.
		/// </summary>
		/// <param name="publishHandler">The true handler to be called for messages in the buffer.</param>
		public UnBufferedSenderStrategy(Action<Message> publishHandler)
		{
			_publishHandler = publishHandler;
		}


		/// <summary>
		/// This method is called to publish a message.  In this case of this handler, the message will
		/// be buffered and then processed from the buffer.
		/// </summary>
		/// <param name="messageToPublish">The message to buffer for publishing.</param>
		public void Publish(Message messageToPublish)
		{
			_publishHandler(messageToPublish);
		}


		/// <summary>
		/// Dispose of the bucket and clean up resources
		/// </summary>
		public void Dispose()
		{
		}
	}
}
