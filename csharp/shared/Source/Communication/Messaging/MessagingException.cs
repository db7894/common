using System;


namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// The generic messaging exception that wraps any provider specific exceptions.
	/// </summary>
	public class MessagingException : ApplicationException
	{
		/// <summary>
		/// Returns true if the exception received was fatal and requires a reconnect.
		/// </summary>
		public bool IsFatal { get; private set; }


		/// <summary>
		/// Constructs a messaging exception with no message and no inner exception.
		/// </summary>
		public MessagingException()
		{
		}


		/// <summary>
		/// Constructs a messaging exception with the given message and no inner exception.
		/// </summary>
		/// <param name="message">The message that details the exception reason.</param>
		public MessagingException(string message) 
			: base(message)
		{			
		}


		/// <summary>
		/// Constructs a messaging exception with the given message and no inner exception.
		/// </summary>
		/// <param name="message">The message that details the exception reason.</param>
		/// <param name="isFatal">True if the exception received is fatal and requires a messaging reconnection.</param>
		public MessagingException(string message, bool isFatal)
			: base(message)
		{
			IsFatal = isFatal;
		}


		/// <summary>
		/// Constructs a messaging exception with the given message and an inner exception
		/// that caused this exception to be raised.
		/// </summary>
		/// <param name="message">The message that details the exception reason.</param>
		/// <param name="innerException">The exception that caused this exception to raise.</param>
		public MessagingException(string message, Exception innerException)
			: base(message, innerException)
		{			
		}


		/// <summary>
		/// Constructs a messaging exception with the given message and an inner exception
		/// that caused this exception to be raised.
		/// </summary>
		/// <param name="message">The message that details the exception reason.</param>
		/// <param name="innerException">The exception that caused this exception to raise.</param>
		/// <param name="isFatal">True if the exception received is fatal and requires a messaging reconnection.</param>
		public MessagingException(string message, Exception innerException, bool isFatal)
			: base(message, innerException)
		{
			IsFatal = isFatal;
		}
	}
}
