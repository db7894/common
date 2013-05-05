using System;


namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// The base interface for both IMessageSender and IMessageReceiver with common elements of both.
	/// </summary>
	public interface IMessagingConnection : IDisposable
	{
		/// <summary>
		/// Gets true if the sender is connected to the messaging provider.
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Gets true if the connection has been disposed already.  Calling any methods on this class
		/// after it has been disposed is an error.
		/// </summary>
		bool IsDisposed { get; }

		/// <summary>
		/// Gets the provider properties this receiver was created with.
		/// </summary>
		ProviderProperties Provider { get; }


		/// <summary>
		/// Gets the number of asynchronous errors received from the provider.
		/// </summary>
		long ErrorCount { get; }


		/// <summary>
		/// Gets the last error message received from the provider.
		/// </summary>
		string LastError { get; }


		/// <summary>
		/// Gets the host we are currently connected to.
		/// </summary>
		string ConnectedHost { get; }


		/// <summary>
		/// Attempt to connect to the messaging provider.
		/// </summary>
		void Connect();


		/// <summary>
		/// Attempt to disconnect from the messaging provider.
		/// </summary>
		void Disconnect();
	}
}
