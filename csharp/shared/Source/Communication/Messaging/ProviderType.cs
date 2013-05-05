namespace SharedAssemblies.Communication.Messaging 
{
	/// <summary>
	/// A list of the available messaging providers that can be used.
	/// </summary>
	public enum ProviderType
	{
		/// <summary>
		/// Uses the Fiorano JMS 7.5 bus for messaging.
		/// </summary>
		Fiorano,


		/// <summary>
		/// A Mock provider allows you to queue up results for publishing and subscribing so that you can
		/// unit test code that depends on messaging.
		/// </summary>
		Mock,


		/// <summary>
		/// A Null provider simply absorbs all requests and returns success.  Useful if you want to
		/// disable messaging but not have to rip out any code.
		/// </summary>
		Null
	}
}
