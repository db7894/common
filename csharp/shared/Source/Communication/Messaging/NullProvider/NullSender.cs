using System;
using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.NullProvider
{
	/// <summary>
	/// A null sender simply publishes to oblivion.  Useful if you need to disable messaging without
	/// ripping out the code.
	/// </summary>
	internal class NullSender : AbstractMessageSender
	{
		/// <summary>
		/// Constructs the null sender given the details.
		/// </summary>
		/// <param name="properties">Provider specific properties.</param>
		/// <param name="sender">SenderProperties specific properties.</param>
		/// <param name="asyncErrorHandler">Handler for asynchronous errors.</param>
		public NullSender(ProviderProperties properties, SenderProperties sender, 
			Action<MessagingException> asyncErrorHandler)
			: base(new NullContext(properties), sender, asyncErrorHandler)
		{
		}

		/// <summary>
		/// The implementation of this method will send the message to the actual provider.  If an error
		/// is encountered, it is expected that this method will throw a provider-specific exception.
		/// </summary>
		/// <param name="message">The message to send to the provider.</param>
		protected override void SendToProvider(Message message)
		{
		}

		/// <summary>
		/// Abstract method to override to start the sender on the connection.
		/// </summary>
		protected override void OnStartSender()
		{
		}

		/// <summary>
		/// Abstract method to override to stop the sender on the connection.
		/// </summary>
		protected override void OnStopSender()
		{
		}

		/// <summary>
		/// Abstract method to override to dispose of the sender.
		/// </summary>
		protected override void OnDisposeSender()
		{
		}
	}
}
