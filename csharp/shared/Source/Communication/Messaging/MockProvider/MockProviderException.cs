using System;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// This is a mock exception from a mock provider.
	/// </summary>
	public class MockProviderException : Exception
	{
		/// <summary>
		/// Constructs a MockProviderException with the appropriate message text
		/// </summary>
		/// <param name="text">Text of the exception.</param>
		public MockProviderException(string text)
			: base(text)
		{			
		}
	}
}
