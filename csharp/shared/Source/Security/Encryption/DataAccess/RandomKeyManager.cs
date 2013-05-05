using System.Collections.Generic;


namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// Implementation of <see cref="AbstractKeyManager"/> to always return unique
	/// random keys
	/// </summary>
	public class RandomKeyManager : AbstractKeyManager
	{
		/// <summary>
		/// The key size to return for each invocation
		/// </summary>
		private int KeySize { get; set; }

		/// <summary>
		/// Initializes a new instance of the RandomKeyManager
		/// </summary>
		/// <param name="keysize">The keysize to return (defaults to 32 if 0)</param>
		public RandomKeyManager(int keysize)
		{
			KeySize = (keysize <= 0) ? 32 : keysize;
		}

		/// <summary>
		/// Perform any initialization needed to retrieve the keys
		/// </summary>
		/// <returns>A collection of key components to create final key</returns>
		protected override IEnumerable<KeyContainer> InitializeKeys()
		{
			return new List<KeyContainer>
			{
				new KeyContainer
				{
					EncryptionKey = EncryptionUtility.GenerateRandomByteToken(KeySize),
					SigningKey = EncryptionUtility.GenerateRandomByteToken(KeySize),
				},
			};
		}
	}
}
