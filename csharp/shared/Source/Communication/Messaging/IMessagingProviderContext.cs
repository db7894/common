using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// This interface represents a connection to the provider's context.
	/// </summary>
	public interface IMessagingProviderContext : IConnection
	{
		/// <summary>
		/// The provider properties this receiver was created with.
		/// </summary>
		ProviderProperties Provider { get; }

		/// <summary>
		/// The host we have succeeded in connecting to.
		/// </summary>
		string ConnectedHost { get; }
	}
}
