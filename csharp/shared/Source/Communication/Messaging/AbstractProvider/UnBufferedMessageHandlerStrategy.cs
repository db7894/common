using System;
using SharedAssemblies.General.Threading;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// Receives messages into a buffer which is used to decouple the messages from the bus so that
	/// slow-downs in handling the message do not delay in taking messages off the bus.  The down-side to this is
	/// if the client does not consume messages fast enough, the buffer could consume a lot of memory.
	/// </summary>
	internal class UnBufferedMessageHandlerStrategy : IMessageHandlerStrategy
	{
		private readonly Action<Message> _wrappedHandler;


		/// <summary>
		/// Gets the number of messages that have been processed from the buffer.
		/// </summary>
		public long ProcessedCount { get; private set; }


		/// <summary>
		/// Gets the number of messages still in the buffer awaiting processing.
		/// </summary>
		public long AwaitingProcessingCount
		{
			get { return 0; }
		}


		/// <summary>
		/// Constructs a buffered message handler which will receive all messages into a buffer and 
		/// then call the handler indicated to process them from the buffer instead of straight off the bus.
		/// </summary>
		/// <param name="newMessageHandler">The true handler to be called for messages in the buffer.</param>
		public UnBufferedMessageHandlerStrategy(Action<Message> newMessageHandler)
		{
			_wrappedHandler = newMessageHandler;
		}


		/// <summary>
		/// This method is called on a message handler whenever a new message
		/// has been received from the receiver.
		/// </summary>
		/// <param name="receivedMessage">The message that was received.</param>
		public void OnMessageReceived(Message receivedMessage)
		{
			if (_wrappedHandler != null)
			{
				_wrappedHandler(receivedMessage);
			}
			++ProcessedCount;
		}


		/// <summary>
		/// Dispose of the bucket and clean up resources
		/// </summary>
		public void Dispose()
		{
		}
	}
}
