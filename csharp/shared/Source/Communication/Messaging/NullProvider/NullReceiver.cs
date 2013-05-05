using System;
using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.NullProvider
{
	/// <summary>
	/// A class that is a null receiver.  Basically this is useful if you want to disable messaging without
	/// ripping out code.  A null receiver will just absorb any requests made to it.
	/// </summary>
	internal class NullReceiver : AbstractMessageReceiver
	{
		/// <summary>
		/// Create the null receiver.
		/// </summary>
		/// <param name="properties">Provider specific properties.</param>
		/// <param name="receiver">ReceiverProperties specific properties.</param>
		/// <param name="newMessageAction">Action to take when new messages arrive.</param>
		/// <param name="asyncErrorHandler">Action to take when an asynchronous error happens.</param>
		public NullReceiver(ProviderProperties properties, ReceiverProperties receiver, 
			Action<Message> newMessageAction, Action<MessagingException> asyncErrorHandler)
			: base(new NullContext(properties), receiver, newMessageAction, asyncErrorHandler)
		{
		}

		/// <summary>
		/// Abstract method to override to start the receiver on the connection.
		/// </summary>
		protected override void OnStartReceiver()
		{
		}

		/// <summary>
		/// Abstract method to override to stop the receiver on the connection.
		/// </summary>
		protected override void OnStopReceiver()
		{
		}

		/// <summary>
		/// Abstract method a provider must override to specify how to read a message asynchronously.
		/// This method is expected to throw provider-specified errors if an error occurs or return null if
		/// the timeout expires.
		/// </summary>
		/// <param name="timeout">The time to wait for a message before returning null if none arrives.</param>
		/// <returns>The new message.</returns>
		protected override Message OnSynchronousRead(TimeSpan timeout)
		{
			return null;
		}

		/// <summary>
		/// Abstract method to override to dispose of the receiver.
		/// </summary>
		protected override void OnDisposeReceiver()
		{
		}
	}
}
