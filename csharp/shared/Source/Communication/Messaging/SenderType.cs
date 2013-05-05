namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Specifies the type of connection to use for messaging.
	/// </summary>
	public enum SenderType
	{
		/// <summary>
		/// A sender is typically a multi-cast publish.  This is typically through a topic medium on a JMS bus.
		/// </summary>
		Publisher,

		/// <summary>
		/// A producer is a pure publish.  This is typically through a queue medium on a JMS bus.
		/// </summary>
		Producer,
	}
}
