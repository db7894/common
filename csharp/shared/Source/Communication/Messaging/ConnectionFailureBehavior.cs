namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Enumeration that specifies the different types of behavior that can be used for connection.
	/// </summary>
	public enum ConnectionFailureBehavior
	{
		/// <summary>
		/// If the initial connect fails it will immediately throw an exception.  If the connection fails
		/// while already connected, will throw an asynchronous exception.
		/// </summary>
		ThrowOnFailure,

		/// <summary>
		/// If the initial connect fails or the connection fails while connected, a reconnect loop will be entered.
		/// Anything sent during a down-time, however, will be lost.
		/// </summary>
		ReconnectOnFailure,
	}
}
