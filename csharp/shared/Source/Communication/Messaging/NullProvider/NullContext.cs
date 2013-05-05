using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.NullProvider
{
	/// <summary>
	/// Implements a null provider context that does nothing.
	/// </summary>
	internal class NullContext : AbstractMessagingProviderContext
	{
		/// <summary>
		/// Instantiates the null provider context.
		/// </summary>
		/// <param name="providerProperties">The provider specific properties.</param>
		public NullContext(ProviderProperties providerProperties)
			: base(providerProperties)
		{
		}

		/// <summary>
		/// Implemented by the provider connection to connect to the provider and return a string
		/// indicating the host name a connection was made to.
		/// </summary>
		/// <returns>Host name that a connection was successful to.</returns>
		protected override string OnProviderConnect()
		{
			return Provider.Hosts[0];
		}

		/// <summary>
		/// Implemented by the provider connection to handle a disconnect from the provider.
		/// </summary>
		protected override void OnProviderDisconnect()
		{
		}

		/// <summary>
		/// Implemented by the provider connection to handle any resource disposal needs, if any.
		/// </summary>
		protected override void OnProviderDispose()
		{
		}
	}
}
