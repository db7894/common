using System;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// Generic interface for Fiorano publishing.
	/// </summary>
	internal interface IFioranoStrategy : IDisposable
	{
		/// <summary>
		/// Send a message on the Fiorano sender.
		/// </summary>
		/// <param name="message">The generic message to send.</param>
		void Send(Message message);

		/// <summary>
		/// Receive a message synchronously from the Fiorano receiver.
		/// </summary>
		/// <param name="timeout">The timeout to wait for a new message to arrive.</param>
		/// <returns>A new message or null if none arrives within the timeout specified.</returns>
		Message Read(TimeSpan timeout);
	}
}