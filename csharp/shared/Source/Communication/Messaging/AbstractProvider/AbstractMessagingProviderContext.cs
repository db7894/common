using System.Timers;


namespace SharedAssemblies.Communication.Messaging.AbstractProvider
{	
	/// <summary>
	/// The basic connection/disconnection functionality of senders and receivers.
	/// </summary>
	public abstract class AbstractMessagingProviderContext : AbstractConnection, IMessagingProviderContext
	{
		/// <summary>
		/// The provider properties this receiver was created with.
		/// </summary>
		public ProviderProperties Provider { get; private set; }


		/// <summary>
		/// The host we have succeeded in connecting to.
		/// </summary>
		public string ConnectedHost { get; private set; }


		/// <summary>
		/// Constructs an instance of the abstract connection class with just the
		/// provider connection details.
		/// </summary>
		/// <param name="providerProperties">The provider's properties.</param>
		protected AbstractMessagingProviderContext(ProviderProperties providerProperties)
			: base(providerProperties.ConnectionFailureBehavior)
		{
			Provider = providerProperties;
		}

	
		/// <summary>
		/// Connect to the underlying provider and set the host name.
		/// </summary>
		protected override void OnConnect()
		{
			ConnectedHost = OnProviderConnect();			
		}


		/// <summary>
		/// Dispose the underlying provider and clear the host name.
		/// </summary>
		protected override void OnDispose()
		{
			OnProviderDispose();
			ConnectedHost = null;
		}


		/// <summary>
		/// Disconnect from the underlying provider and clear the host name.
		/// </summary>
		protected override void OnDisconnect()
		{
			OnProviderDisconnect();
			ConnectedHost = null;
		}


		/// <summary>
		/// Implemented by the provider connection to connect to the provider and return a string
		/// indicating the host name a connection was made to.
		/// </summary>
		/// <returns>Host name that a connection was successful to.</returns>
		protected abstract string OnProviderConnect();


		/// <summary>
		/// Implemented by the provider connection to handle a disconnect from the provider.
		/// </summary>
		protected abstract void OnProviderDisconnect();


		/// <summary>
		/// Implemented by the provider connection to handle any resource disposal needs, if any.
		/// </summary>
		protected abstract void OnProviderDispose();
	}
}
