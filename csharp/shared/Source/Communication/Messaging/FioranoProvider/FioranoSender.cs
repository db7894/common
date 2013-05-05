using System;
using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// The Fiorano specific messaging sender.
	/// </summary>
	internal sealed class FioranoSender : AbstractMessageSender
	{
		private IFioranoStrategy _strategy;


		/// <summary>
		/// Returns the context as a FioranoContext
		/// </summary>
		public FioranoContext FioranoContext
		{
			get { return Context as FioranoContext; }
		}


		/// <summary>
		/// Constructs a Fiorano specific instance of a messaging sender.
		/// </summary>
		/// <param name="provider">The Fiorano-specific provider properties..</param>
		/// <param name="sender">The sender properties for this sender.</param>
		/// <param name="asyncExceptionHandler">Handler to call if an asynchronous error happens.</param>
		public FioranoSender(ProviderProperties provider, SenderProperties sender,
			Action<MessagingException> asyncExceptionHandler)
			: base(new FioranoContext(provider), sender, asyncExceptionHandler)
		{
		}


		/// <summary>
		/// The implementation of this method will send the message to the actual provider.  If an error
		/// is encountered, it is expected that this method will throw a provider-specific exception.
		/// </summary>
		/// <param name="message">The message to send to the provider.</param>
		protected override void SendToProvider(Message message)
		{
			_strategy.Send(message);
		}


		/// <summary>
		/// Abstract method to override to start the sender on the connection.
		/// </summary>
		protected override void OnStartSender()
		{
			if (SenderProperties.SenderType == SenderType.Producer)
			{
				_strategy = new FioranoQueueStrategy(this);
			}
			else
			{
				_strategy = new FioranoTopicStrategy(this);
			}
		}


		/// <summary>
		/// Abstract method to override to stop the sender on the connection.
		/// </summary>
		protected override void OnStopSender()
		{
			if (_strategy != null)
			{
				_strategy.Dispose();
				_strategy = null;
			}
		}


		/// <summary>
		/// Abstract method to override to dispose of the sender.
		/// </summary>
		protected override void OnDisposeSender()
		{
			OnStopSender();
		}
	}
}
