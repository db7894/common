using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// The base connection interface that dictates how all connections behave.
	/// </summary>
	public interface IConnection : IDisposable
	{
		/// <summary>
		/// True if the sender is connected to the messaging provider.
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// True if the connection has been disposed already.  Calling any methods on this class
		/// after it has been disposed is an error.
		/// </summary>
		bool IsDisposed { get; }

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
