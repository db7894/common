using System;
using SharedAssemblies.General.Threading;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// Receives messages into a buffer which is used to decouple the messages from the bus so that
	/// slow-downs in handling the message do not delay in taking messages off the bus.  The down-side to this is
	/// if the client does not consume messages fast enough, the buffer could consume a lot of memory.
	/// </summary>
	internal class BufferedMessageHandlerStrategy : IMessageHandlerStrategy
	{
		private const int _initialBucketSize = 1000;
		private const int _maxGetSize = 50;
		private readonly Consumer<Message> _consumer;
		private readonly Action<Message> _wrappedHandler;
		private readonly Action<MessagingException> _asyncErrorHandler;


		/// <summary>
		/// Gets the number of messages that have been processed from the buffer.
		/// </summary>
		public long ProcessedCount { get; private set; }


		/// <summary>
		/// Gets the number of messages still in the buffer awaiting processing.
		/// </summary>
		public long AwaitingProcessingCount
		{
			get { return _consumer.CollectionCount; }
		}


		/// <summary>
		/// Constructs a buffered message handler which will receive all messages into a buffer and 
		/// then call the handler indicated to process them from the buffer instead of straight off the bus.
		/// </summary>
		/// <param name="newMessageHandler">The true handler to be called for messages in the buffer.</param>
		/// <param name="asyncExceptionHandler">The handler for any asynchronous exceptions.</param>
		public BufferedMessageHandlerStrategy(Action<Message> newMessageHandler,
			Action<MessagingException> asyncExceptionHandler)
		{
			_asyncErrorHandler = asyncExceptionHandler;
			_wrappedHandler = newMessageHandler;
			_consumer = new Consumer<Message>(ConsumeMessage);
			_consumer.Start();
		}


		/// <summary>
		/// This method is called on a message handler whenever a new message
		/// has been received from the receiver.
		/// </summary>
		/// <param name="receivedMessage">The message that was received.</param>
		public void OnMessageReceived(Message receivedMessage)
		{
			_consumer.Add(receivedMessage);
		}


		/// <summary>
		/// Dispose of the bucket and clean up resources
		/// </summary>
		public void Dispose()
		{
			_consumer.Stop();
		}


		/// <summary>
		/// Consumes a message from the buffer and sends it out for processing. 
		/// </summary>
		/// <param name="bufferedMessage">The message taken from the buffer.</param>
		private void ConsumeMessage(Message bufferedMessage)
		{
			try
			{
				if (_wrappedHandler != null)
				{
					_wrappedHandler(bufferedMessage);
				}
				++ProcessedCount;
			}
			catch(MessagingException mex)
			{
				_asyncErrorHandler(mex);
			}
			catch(Exception ex)
			{
				_asyncErrorHandler(new MessagingException("Exception in ConsumeMessage().", ex));
			}		
		}
	}
}
