namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Class that contains configuration information about a sender
	/// </summary>
	public class ReceiverProperties 
	{
		/// <summary>
		/// The type of the message source.  If the source is a queue, then consuming the message will
		/// remove it and no other consumers will get it.  If the source is a topic, then consuming the
		/// message only consumes the copy of the message sent to this consumer.
		/// </summary>
		public ReceiverType ReceiverType { get; private set; }


		/// <summary>
		/// The name of the source (topic or queue in JMS) to consume from.
		/// </summary>
		public string Source { get; private set; }


		/// <summary>
		/// The name of the topic to send receiver statistics to.  If this is blank or null no
		/// statistics will be sent.
		/// </summary>
		public string StatisticsDestination { get; private set; }


		/// <summary>
		/// True if messages that are consumed are durable.  This means that if the receiver is temporarily
		/// unavailable the messaging provider will hold onto the message for a limited amount of time.  This
		/// only applies to topics - this property is ignored for queues.
		/// </summary>
		public bool IsDurable { get; private set; }


		/// <summary>
		/// If true, messages will not be queued from the messaging provider but will be handled
		/// immediately, it is not advised that you turn this option on as it could cause slow receivers.
		/// </summary>
		public bool IsBuffered { get; private set; }


		/// <summary>
		/// Constructs a set of receiver properties.
		/// </summary>
		/// <param name="receiverType">The type of message source (queue or topic) to read from.</param>
		/// <param name="source">The source queue or topic to read from.</param>
		/// <param name="isDurable">True if the subscriptions should be durable.</param>
		/// <param name="isBuffered">True if should buffer messages read before processing them.</param>
		public ReceiverProperties(ReceiverType receiverType, string source, bool isDurable, bool isBuffered)
		{
			ReceiverType = receiverType;
			Source = source;
			IsDurable = isDurable;
			IsBuffered = isBuffered;
		}

	
		/// <summary>
		/// Constructs a set of receiver properties with buffering on by default.
		/// </summary>
		/// <param name="receiverType">The type of message source (queue or topic) to read from.</param>
		/// <param name="source">The source queue or topic to read from.</param>
		/// <param name="isDurable">True if the subscriptions should be durable.</param>
		public ReceiverProperties(ReceiverType receiverType, string source, bool isDurable)
			: this(receiverType, source, isDurable, true)
		{
		}


		/// <summary>
		/// Constructs a set of receiver properties with buffering on and durability off.
		/// </summary>
		/// <param name="receiverType">The type of message source (queue or topic) to read from.</param>
		/// <param name="source">The source queue or topic to read from.</param>
		public ReceiverProperties(ReceiverType receiverType, string source)
			: this(receiverType, source, false, true)
		{
		}
	}
}
