namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Specifies the type of connection to use for messaging.
	/// </summary>
	public enum ReceiverType
	{
		/// <summary>
		/// A receiver listens to a sender.  All receivers listening to a sender will get the same copy of each message.
		/// This is usually accomplished with topics in JMS.
		/// </summary>
		Subscriber,

		/// <summary>
		/// A consumer receives a message and consumes it.  No other consumers listening to the sender will get a copy 
		/// of the message.  This is usually accomplished with queues in JMS.
		/// </summary>
		Consumer,
	}
}
