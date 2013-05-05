using System.Collections.Generic;


namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// Implementation of <see cref="AbstractKeyManager"/> which allows a user to directly
	/// inject their own key to be managed
	/// </summary>
	public class QuickKeyManager : AbstractKeyManager
	{
		/// <summary>
		/// The key size to return for each invocation
		/// </summary>
		private readonly KeyContainer _initialContainer;

		/// <summary>
		/// Initializes a new instance of the QuickKeyManager
		/// </summary>
		/// <param name="container">A container with the keys to use for encryption</param>
		public QuickKeyManager(KeyContainer container)
		{
			_initialContainer = container;
		}

		/// <summary>
		/// Perform any initialization needed to retrieve the keys
		/// </summary>
		/// <returns>A collection of key components to create final key</returns>
		protected override IEnumerable<KeyContainer> InitializeKeys()
		{
			return new List<KeyContainer> { _initialContainer };
		}
	}
}
