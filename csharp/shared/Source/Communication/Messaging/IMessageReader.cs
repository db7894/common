using System;


namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// A synchronous read interface for receivers.
	/// </summary>
	public interface IMessageReader : IMessagingConnection
	{
		/// <summary>
		/// Gets the properties for this receiver.
		/// </summary>
		ReceiverProperties ReceiverProperties { get; }


		/// <summary>
		/// Gets the number of messages completely processed.
		/// </summary>
		long ProcessedCount { get; }


		/// <summary>
		/// Reads the next message off of the queue, waits infinitely for a new message to arrive.
		/// </summary>
		/// <returns>The new message.</returns>
		Message Read();


		/// <summary>
		/// Reads the next message off of the queue for up to the timeout specified.  If no message arrives
		/// by the timeout specified will return null.
		/// </summary>
		/// <param name="timeout">The time to wait for a new message before timing out.</param>
		/// <returns>The new message.</returns>
		Message Read(TimeSpan timeout);
	}
}
