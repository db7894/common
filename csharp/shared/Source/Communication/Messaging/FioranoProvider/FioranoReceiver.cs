using System;
using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// The Fiorano implementation of a message receiver.
	/// </summary>
	internal sealed class FioranoReceiver : AbstractMessageReceiver
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
		/// Construct the Fiorano messaging receiver instance.
		/// </summary>
		/// <param name="provider">The provider-specific properties.</param>
		/// <param name="receiver">ReceiverProperties properties for the receiver.</param>
		/// <param name="newMessageAction">The action to take when a new message is received.</param>
		/// <param name="asyncErrorHandler">The asynchronous error handler.</param>
		public FioranoReceiver(ProviderProperties provider, ReceiverProperties receiver, 
			Action<Message> newMessageAction, Action<MessagingException> asyncErrorHandler)
			: base(new FioranoContext(provider), receiver, newMessageAction, asyncErrorHandler)
		{
		}


		/// <summary>
		/// Abstract method to override to start the receiver on the connection.
		/// </summary>
		protected override void OnStartReceiver()
		{
			if (ReceiverProperties.ReceiverType == ReceiverType.Consumer)
			{
				_strategy = new FioranoQueueStrategy(this, OnMessageReceived);
			}
			else
			{
				_strategy = new FioranoTopicStrategy(this, OnMessageReceived);
			}
		}

		/// <summary>
		/// Abstract method to override to stop the receiver on the connection.
		/// </summary>
		protected override void OnStopReceiver()
		{
			if (_strategy != null)
			{
				_strategy.Dispose();
				_strategy = null;
			}
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
			return _strategy.Read(timeout);
		}


		/// <summary>
		/// Abstract method to override to dispose of the receiver.
		/// </summary>
		protected override void OnDisposeReceiver()
		{
			OnStopReceiver();
		}
	}
}
