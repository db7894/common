using SharedAssemblies.Communication.Messaging.AbstractProvider;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// The messaging provider context for the mock provider.
	/// </summary>
	internal class MockContext : AbstractMessagingProviderContext
	{
		/// <summary>
		/// Instantiate a mock context.
		/// </summary>
		/// <param name="providerProperties">The properties for the provider</param>
		public MockContext(ProviderProperties providerProperties)
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
