using System.Collections.Generic;
using System.Linq;

namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// Implementation of <see cref="AbstractKeyManager"/> which allows a user
	/// retrieve bashwork encryption keys from a predefined database
	/// </summary>
	public class SqlKeyManager : AbstractKeyManager
	{
		/// <summary>
		/// Handle to our underlying database for retrieving keys
		/// </summary>
		private readonly IEncryptionKeyDao _initialDatabase;

		/// <summary>
		/// Handle to the vendor associated with the keys
		/// </summary>
		private readonly IEnumerable<int> _keyIdentifiers;

		/// <summary>
		/// Initialize a new instance of the SqlKeyManager class
		/// </summary>
		/// <param name="database">The data access object to use to retrieve the keys</param>
		/// <param name="keys">The list of keys to retrieve from the database</param>
		public SqlKeyManager(IEncryptionKeyDao database, IEnumerable<int> keys)
		{
			_initialDatabase = database;
			_keyIdentifiers = keys;
		}

		/// <summary>
		/// Perform any initialization needed to retrieve the keys
		/// </summary>
		/// <returns>A collection of key components to create final key</returns>
		protected override IEnumerable<KeyContainer> InitializeKeys()
		{
			IEnumerable<KeyContainer> result = null;

			if ((_initialDatabase != null) && (_keyIdentifiers.Count() > 0))
			{
				result = _initialDatabase.GetEncryptionKeysFromDatabase(_keyIdentifiers);
			}

			return result;
		}
	}
}
