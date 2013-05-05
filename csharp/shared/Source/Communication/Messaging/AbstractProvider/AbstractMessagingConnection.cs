using System;
using log4net;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{
	/// <summary>
	/// The basic connection/disconnection functionality of senders and receivers.
	/// </summary>
	public abstract class AbstractMessagingConnection : AbstractConnection, IMessagingConnection
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(AbstractMessagingConnection));
		private readonly Action<MessagingException> _exceptionHandler;


		/// <summary>
		/// Gets the properties for the provider.
		/// </summary>
		public ProviderProperties Provider
		{
			get { return Context.Provider; }
		}


		/// <summary>
		/// Gets the name of the connected host.
		/// </summary>
		public string ConnectedHost
		{
			get { return Context.ConnectedHost; }
		}


		/// <summary>
		/// The number of asynchronous errors received from the provider.
		/// </summary>
		public long ErrorCount { get; private set; }


		/// <summary>
		/// The last error message received from the provider.
		/// </summary>
		public string LastError { get; private set; }


		/// <summary>
		/// Gets the current messaging context for the provider.
		/// </summary>
		protected IMessagingProviderContext Context { get; private set; }


		/// <summary>
		/// Constructs an instance of the abstract connection class with just the
		/// provider connection details.
		/// </summary>
		/// <param name="context">The initial context of the provider.</param>
		/// <param name="exceptionHandler">The exception handler for asynchronous exceptions.</param>
		protected AbstractMessagingConnection(AbstractMessagingProviderContext context, 
			Action<MessagingException> exceptionHandler)
			: base(context.Provider.ConnectionFailureBehavior)
		{
			Context = context;
			_exceptionHandler = exceptionHandler;
		}


		/// <summary>
		/// Helper method to report an error to log and such.
		/// </summary>
		/// <param name="errorMessage">The error message to log and put in the exception description.</param>
		/// <param name="ex">The inner exception that caused the error.</param>
		public void ThrowError(string errorMessage, Exception ex)
		{
			ThrowError(errorMessage, new MessagingException(errorMessage, ex));
		}


		/// <summary>
		/// Helper method to report an error to log and such.
		/// </summary>
		/// <param name="errorMessage">The error message to log and put in the exception description.</param>
		/// <param name="ex">The inner exception that caused the error.</param>
		public void ThrowError(string errorMessage, MessagingException ex)
		{
			LogError(errorMessage, ex);

			throw ex;
		}


		/// <summary>
		/// Logs an error but does not throw it.
		/// </summary>
		/// <param name="errorMessage">The error message to log.</param>
		/// <param name="ex">The exception to log.</param>
		public void LogError(string errorMessage, Exception ex)
		{
			++ErrorCount;

			LastError = ex.Message;

			_log.Error(errorMessage, ex);
		}


		/// <summary>
		/// Handles an asynchronous message from the provider and pushes it up to any registered listener.
		/// </summary>
		/// <param name="asynchronousException">The exception received asynchronously.</param>
		public void RaiseAsynchronousError(MessagingException asynchronousException)
		{
			LogError("Asynchronous exception received.", asynchronousException);

			if (_exceptionHandler != null)
			{
				try
				{
					if (Provider.ConnectionFailureBehavior == ConnectionFailureBehavior.ReconnectOnFailure)
					{
						_log.Warn("Asynchronous exception: reconnecting...", asynchronousException);
						Reconnect();
					}

					_exceptionHandler(asynchronousException);
				}
				catch (Exception ex)
				{
					_log.Error("Error: exception in asynchronous exception handler.", ex);
				}
			}
		}
	}
}
