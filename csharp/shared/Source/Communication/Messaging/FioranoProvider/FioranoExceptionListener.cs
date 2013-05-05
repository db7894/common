using System;
using System.Diagnostics.CodeAnalysis;
using fiorano.csharp.fms;
using fiorano.csharp.runtime.exception;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// Listens for Fiorano asynchronous errors.
	/// </summary>
	internal class FioranoExceptionListener : ExceptionListener
	{
		private readonly Action<MessagingException> _errorHandler;

		/// <summary>
		/// Constructs a FioranoExceptionListener instance that routes new exceptions to the delegate specified.
		/// </summary>
		/// <param name="errorHandler">The delegate to call on asynchronous exceptions.</param>
		public FioranoExceptionListener(Action<MessagingException> errorHandler)
		{
			_errorHandler = errorHandler;
		}

		/// <summary>
		/// This method is called whenever Fiorano throws an asynchronous error message.
		/// </summary>
		/// <param name="error">The asynchronous error from Fiorano.</param>
		[SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", 
			"SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fiorano override, can't change.")]
		public void onException(FioranoException error)
		{
			_errorHandler(new MessagingException("Asynchronous error: " + error.Message, error, true));
		}
	}
}