using System;
using System.Collections;
using fiorano.csharp.fms;
using fiorano.csharp.naming;
using log4net;
using SharedAssemblies.Communication.Messaging.AbstractProvider;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// This is the Fiorano implementation of the messaging connection.
	/// </summary>
	internal sealed class FioranoContext : AbstractMessagingProviderContext
	{
		// The default initial context factory if the one specified in ProviderProperties is empty or null
		private const string _defaultContext = "fiorano.jms.runtime.naming.FioranoInitialContextFactory";
		private const string _defaultTopicFactory = "PRIMARYTCF";
		private const string _defaultQueueFactory = "PRIMARYQCF";

		private static readonly ILog _log = LogManager.GetLogger(typeof(FioranoContext));

		private readonly string _initialContextName;
		private readonly string _topicFactoryName;
		private readonly string _queueFactoryName;

		/// <summary>
		/// Gets the Fiorano specific naming context for the connection.
		/// </summary>
		internal FioranoNamingContext NamingContext { get; private set; }

		/// <summary>
		/// Gets the Fiorano specific topic connection factory.
		/// </summary>
		internal TopicConnectionFactory TopicFactory { get; private set; }

		/// <summary>
		/// Gets the Fiorano specific queue connection factory.
		/// </summary>
		internal QueueConnectionFactory QueueFactory { get; private set; }
		
		/// <summary>
		/// Constructs the messaging connection to a Fiorano JMS bus.
		/// </summary>
		/// <param name="providerProperties">The properties that govern the provider.</param>
		public FioranoContext(ProviderProperties providerProperties)
			: base(providerProperties)
		{
			_initialContextName = providerProperties.InitialContextFactory.IsNotNullOrEmpty()
			                      	? providerProperties.InitialContextFactory
			                      	: _defaultContext;
			_topicFactoryName = providerProperties.TopicFactory.IsNotNullOrEmpty()
			                    	? providerProperties.TopicFactory
			                    	: _defaultTopicFactory;
			_queueFactoryName = providerProperties.QueueFactory.IsNotNullOrEmpty()
			                    	? providerProperties.QueueFactory
			                    	: _defaultQueueFactory;
		}

		/// <summary>
		/// The actual method to connect to a messaging provider.  This method is expected to throw
		/// a provider-specific exception if a connection cannot be achieved.
		/// </summary>
		/// <returns>The host we succeeded in connecting to.</returns>
		protected override string OnProviderConnect()
		{
			return ConnectToFirstAvailableHost();
		}

		/// <summary>
		/// The actual method to disconnect from the messaging provider.  This method is expected to throw
		/// a provider-specific exception if disconnection cannot be achieved.
		/// </summary>
		protected override void OnProviderDisconnect()
		{
			TopicFactory = null;
			QueueFactory = null;
			NamingContext = null;
		}


		/// <summary>
		/// Dispose of all resources held by the provider.  Should be coded in such a way to
		/// be safe from repeated calls.
		/// </summary>
		protected override void OnProviderDispose()
		{
			OnProviderDisconnect();
		}


		/// <summary>
		/// Construct the Fiorano initial context.
		/// </summary>
		/// <param name="currentHost">The current host to attempt to connect to.</param>
		/// <param name="initialContextName">The initial context factory name to create.</param>
		/// <returns>The Fiorano naming context.</returns>
		private FioranoNamingContext CreateNamingContext(string currentHost, string initialContextName)
		{
			var contextProperties = new Hashtable
				{
					{ fiorano.csharp.naming.FioranoContext.SECURITY_PRINCIPAL, Provider.UserName },
					{ fiorano.csharp.naming.FioranoContext.SECURITY_CREDENTIALS, Provider.Password },
					{ fiorano.csharp.naming.FioranoContext.PROVIDER_URL, currentHost },
					{ fiorano.csharp.naming.FioranoContext.INITIAL_CONTEXT_FACTORY, initialContextName },
				};

			return new FioranoNamingContext(contextProperties);
		}


		/// <summary>
		/// Attempts to connect to all hosts listed in the host array in order until it finds one that works.
		/// </summary>
		/// <returns>The host we succeeded in connecting to.</returns>
		private string ConnectToFirstAvailableHost()
		{
			string connectedTo = null;
			Exception firstFailure = null;

			// attempt each host, if one fails, try another...
			foreach (var host in Provider.Hosts) 
			{
				try
				{
					_log.Info("Looking up initial context on: " + host);
					NamingContext = CreateNamingContext(host, _initialContextName);

					_log.Info("Looking up topic factory " + _topicFactoryName + ".");
					TopicFactory = NamingContext.lookupTCF(_topicFactoryName);

					_log.Info("Looking up queue factory " + _queueFactoryName + ".");
					QueueFactory = NamingContext.lookupQCF(_queueFactoryName);

					// stop on first success
					_log.Info("Connected to host: " + host);
					connectedTo = host;
					break;
				}
				catch (Exception ex)
				{
					_log.Error("Failed to connect to host: " + host, ex);
					if (firstFailure == null)
					{
						firstFailure = ex;
					}
				}
			}

			if (connectedTo == null)
			{
				throw new MessagingException("Could not connect to any host.", firstFailure, true);
			}

			return connectedTo;
		}
	}
}
