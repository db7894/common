using System;
using System.Diagnostics.CodeAnalysis;
using fiorano.csharp.fms;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// Listens to incoming messages from Fiorano.
	/// </summary>
	internal class FioranoMessageListener : MessageListener
	{
		private Action<Message> _delegate;

		/// <summary>
		/// Constructs a FioranoMessageListener that listens for incoming messages and calls the specified delegate.
		/// </summary>
		/// <param name="messageDelegate">The delegate to call on new messages.</param>
		public FioranoMessageListener(Action<Message> messageDelegate)
		{
			_delegate = messageDelegate;
		}

		/// <summary>
		/// The Fiorano call-back when a new message is received.
		/// </summary>
		/// <param name="message">The new Fiorano message received.</param>
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", 
			"SA1300:ElementMustBeginWithUpperCaseLetter",
			Justification = "Fiorano uses camelCase method naming, can't change since override.")]
		public void onMessage(fiorano.csharp.fms.Message message)
		{
			var genericMessage = FioranoMessageAdapter.Convert((TextMessage)message);
			_delegate(genericMessage);
		}
	}
}