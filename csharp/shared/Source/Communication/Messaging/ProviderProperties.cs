using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Basic properties for any messaging regardless of sender or receiver.
	/// </summary>
	public class ProviderProperties
	{
		/// <summary>
		/// Gets the type of the messaging provider.
		/// </summary>
		public ProviderType ProviderType { get; private set; }

		/// <summary>
		/// Gets the messaging user for the messaging provider's authentication.
		/// </summary>
		public string UserName { get; private set; }

		/// <summary>
		/// Gets the messaging password for the messaging provider's authentication.
		/// </summary>
		public string Password { get; private set; }

		/// <summary>
		/// <para>
		///		Gets the lookup name or class name that handles creating initial context 
		///		for the messaging.  
		/// </para>
		/// <para>
		///		If this is empty or null, will default to the default specified for the given
		///		provider type.  For example, Fiorano will default to 
		///     "fiorano.jms.runtime.naming.FioranoInitialContextFactory".  
		/// </para>
		/// </summary>
		public string InitialContextFactory { get; private set; }

		/// <summary>
		/// <para>
		///		Gets the lookup name or class name that handles constructing pub/sub topics
		///		on the messaging host.  
		/// </para>
		/// <para>
		///		If this is empty or null, will default to the default specified for the given
		///		provider type.  For example, Fiorano will default to "PRIMARYTCF".
		/// </para>
		/// </summary>
		public string TopicFactory { get; private set; }

		/// <summary>
		/// <para>
		///		Gets the lookup name or class name that handles constructing queues on the 
		///		messaging host.  
		/// </para>
		/// <para>
		///		If this is empty or null, will default to the default specified for the given
		///		provider type.  For example, Fiorano will default to "PRIMARYQCF".  
		/// </para>
		/// </summary>
		public string QueueFactory { get; private set; }

		/// <summary>
		/// Gets the list of URLs to rotate through to find a connection.  In Fiorano, this will
		/// be the primary URL and the secondary URL.
		/// </summary>
		public List<string> Hosts { get; private set; }

		/// <summary>
		/// Gets the behavior to perform when a connection fails during Connect() or after connected.
		/// </summary>
		public ConnectionFailureBehavior ConnectionFailureBehavior { get; private set; }

		/// <summary>
		/// Constructs a new list of provider properties.
		/// </summary>
		/// <param name="provider">The type of provider to connect to.</param>
		/// <param name="hosts">The list of hosts to attempt to connect to.</param>
		/// <param name="userName">The user name to use to connect to the provider.</param>
		/// <param name="password">The password to use to connect to the provider.</param>
		/// <param name="initialContextFactory">The lookup name for the provider's initial context (if applicable).</param>
		/// <param name="topicFactory">The lookup name for the provider's topic factory (if applicable).</param>
		/// <param name="queueFactory">The lookup name for the provider's queue factory (if applicable).</param>
		/// <param name="connectionFailureBehavior">The behavior to execute when a connection fails.</param>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.ReadabilityAnalyzer", 
			"ST2006:MethodCannotExceedMaxParameters", Justification = "Immutable object.")]
		public ProviderProperties(ProviderType provider, IEnumerable<string> hosts, string userName,
			string password, string initialContextFactory, string topicFactory, string queueFactory,
			ConnectionFailureBehavior connectionFailureBehavior)
		{
			ProviderType = provider;
			Hosts = hosts.ToList();
			UserName = userName;
			Password = password;
			InitialContextFactory = initialContextFactory;
			TopicFactory = topicFactory;
			QueueFactory = queueFactory;
			ConnectionFailureBehavior = connectionFailureBehavior;
		}

		/// <summary>
		/// Constructs a new list of provider properties.  Default behavior of the provider is to throw an exception 
		/// if the Connect() fails or the connection fails after connected.
		/// </summary>
		/// <param name="provider">The type of provider to connect to.</param>
		/// <param name="hosts">The list of hosts to attempt to connect to.</param>
		/// <param name="userName">The user name to use to connect to the provider.</param>
		/// <param name="password">The password to use to connect to the provider.</param>
		/// <param name="initialContextFactory">The lookup name for the provider's initial context (if applicable).</param>
		/// <param name="topicFactory">The lookup name for the provider's topic factory (if applicable).</param>
		/// <param name="queueFactory">The lookup name for the provider's queue factory (if applicable).</param>
		public ProviderProperties(ProviderType provider, IEnumerable<string> hosts, string userName,
			string password, string initialContextFactory, string topicFactory, string queueFactory)
			: this(provider, hosts, userName, password, initialContextFactory, topicFactory, queueFactory,
				ConnectionFailureBehavior.ThrowOnFailure)
		{
		}

		/// <summary>
		/// Constructs a new list of provider properties with default topic and queue topic factories of null
		/// which will cause the connection to choose the defaults for that provider.
		/// </summary>
		/// <param name="provider">The type of provider to connect to.</param>
		/// <param name="hosts">The list of hosts to attempt to connect to.</param>
		/// <param name="userName">The user name to use to connect to the provider.</param>
		/// <param name="password">The password to use to connect to the provider.</param>
		/// <param name="initialContextFactory">The lookup name for the provider's initial context (if applicable).</param>
		/// <param name="connectionFailureBehavior">The behavior to execute when a connection fails.</param>
		public ProviderProperties(ProviderType provider, IEnumerable<string> hosts, string userName,
				string password, string initialContextFactory,
				ConnectionFailureBehavior connectionFailureBehavior)
			: this(provider, hosts, userName, password, initialContextFactory, null, null,
				connectionFailureBehavior)
		{
		}

		/// <summary>
		/// Constructs a new list of provider properties with default topic and queue topic factories of null
		/// which will cause the connection to choose the defaults for that provider.  Default behavior of the provider is to throw
		/// an exception if the Connect() fails or the connection fails after connected.
		/// </summary>
		/// <param name="provider">The type of provider to connect to.</param>
		/// <param name="hosts">The list of hosts to attempt to connect to.</param>
		/// <param name="userName">The user name to use to connect to the provider.</param>
		/// <param name="password">The password to use to connect to the provider.</param>
		/// <param name="initialContextFactory">The lookup name for the provider's initial context (if applicable).</param>
		public ProviderProperties(ProviderType provider, IEnumerable<string> hosts, string userName, 
				string password, string initialContextFactory)
			: this(provider, hosts, userName, password, initialContextFactory, null, null,
				ConnectionFailureBehavior.ThrowOnFailure)
		{
		}

		/// <summary>
		/// Constructs a new list of provider properties with default initial context, topic, and queue 
		/// topic factories of null which will cause the connection to choose the defaults for that provider.
		/// </summary>
		/// <param name="provider">The type of provider to connect to.</param>
		/// <param name="hosts">The list of hosts to attempt to connect to.</param>
		/// <param name="userName">The user name to use to connect to the provider.</param>
		/// <param name="password">The password to use to connect to the provider.</param>
		/// <param name="connectionFailureBehavior">The behavior to execute when a connection fails.</param>
		public ProviderProperties(ProviderType provider, IEnumerable<string> hosts, string userName,
				string password, ConnectionFailureBehavior connectionFailureBehavior)
			: this(provider, hosts, userName, password, null, null, null, connectionFailureBehavior)
		{
		}

		/// <summary>
		/// Constructs a new list of provider properties with default initial context, topic, and queue 
		/// topic factories of null which will cause the connection to choose the defaults for that provider.
		/// Default behavior of the provider is to throw an exception if the Connect() fails or the connection fails after connected.
		/// </summary>
		/// <param name="provider">The type of provider to connect to.</param>
		/// <param name="hosts">The list of hosts to attempt to connect to.</param>
		/// <param name="userName">The user name to use to connect to the provider.</param>
		/// <param name="password">The password to use to connect to the provider.</param>
		public ProviderProperties(ProviderType provider, IEnumerable<string> hosts, string userName, 
				string password)
			: this(provider, hosts, userName, password, null, null, null, 
				ConnectionFailureBehavior.ThrowOnFailure)
		{
		}
	}
}