namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Class that contains configuration information about a sender
	/// </summary>
	public class SenderProperties 
	{
		/// <summary>
		/// The type of destination to produce messages on.  Can be a queue or topic.  
		/// Topics are broadcast and all receivers get a copy of each message produced, whereas 
		/// with queues, each message is consumed by only one consumer.
		/// </summary>
		public SenderType SenderType { get; private set; }


		/// <summary>
		/// The name of the destination (topic or queue in JMS) to publish to.
		/// </summary>
		public string Destination { get; private set; }


		/// <summary>
		/// True if messages that are published are stored in persistent store in case the 
		/// bus crashes during processing.  Defaults to false.
		/// </summary>
		public bool IsPersistent { get; private set; }


		/// <summary>
		/// If true, publishes will default to being queued for processing before they are sent.
		/// </summary>
		public bool IsBuffered { get; private set; }


		/// <summary>
		/// Constructs a set of sender properties.
		/// </summary>
		/// <param name="senderType">The type of destination (queue or topic).</param>
		/// <param name="destination">The name of the destination queue or topic.</param>
		/// <param name="isPersistent">True if the published messages will survive even if bus goes down.</param>
		/// <param name="isBuffered">True if should buffer messages before publishing to provider.</param>
		public SenderProperties(SenderType senderType, string destination, bool isPersistent, 
			bool isBuffered)
		{
			SenderType = senderType;
			Destination = destination;
			IsPersistent = isPersistent;
			IsBuffered = isBuffered;
		}

	
		/// <summary>
		/// Constructs a set of sender properties that will enable buffering by default.  
		/// </summary>
		/// <param name="senderType">The type of destination (queue or topic).</param>
		/// <param name="destination">The name of the destination queue or topic.</param>
		/// <param name="isPersistent">True if the published messages will survive even if bus goes down.</param>
		public SenderProperties(SenderType senderType, string destination, bool isPersistent)
			: this(senderType, destination, isPersistent, true)
		{
		}

	
		/// <summary>
		/// Constructs a set of sender properties that will enable buffering by default and uses
		/// non-persistent messages on sender.
		/// </summary>
		/// <param name="senderType">The type of destination (queue or topic).</param>
		/// <param name="destination">The name of the destination queue or topic.</param>
		public SenderProperties(SenderType senderType, string destination)
			: this(senderType, destination, false, true)
		{
		}
	}
}
