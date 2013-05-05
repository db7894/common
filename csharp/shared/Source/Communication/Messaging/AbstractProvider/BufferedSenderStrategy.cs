using System;
using SharedAssemblies.General.Threading;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// Publishes messages into a buffer which is used to decouple the messages from the bus so that
	/// slow-downs in publishing the message do not delay the client caller.  
	/// </summary>
	internal class BufferedSenderStrategy : ISenderStrategy
	{
		private const int _initialBucketSize = 1000;
		private const int _maxGetSize = 50;
		private readonly Action<MessagingException> _asyncErrorHandler;
		private readonly Consumer<Message> _consumer;
		private readonly Action<Message> _publishHandler;


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
		/// Constructs a buffered message handler which will queue up all published messages.
		/// A separate thread will work the published messages out of the buffer to avoid holding up the 
		/// client.
		/// </summary>
		/// <param name="publishHandler">The true handler to be called for messages in the buffer.</param>
		/// <param name="asyncErrorHandler">The handler for asynchronous messaging errors.</param>
		public BufferedSenderStrategy(Action<Message> publishHandler, 
			Action<MessagingException> asyncErrorHandler)
		{
			_asyncErrorHandler = asyncErrorHandler;
			_publishHandler = publishHandler;
			_consumer = new Consumer<Message>(ConsumeMessage);
			_consumer.Start();
		}


		/// <summary>
		/// This method is called to publish a message.  In this case of this handler, the message will
		/// be buffered and then processed from the buffer.
		/// </summary>
		/// <param name="messageToPublish">The message to buffer for publishing.</param>
		public void Publish(Message messageToPublish)
		{
			_consumer.Add(messageToPublish);
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
				_publishHandler(bufferedMessage);
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
