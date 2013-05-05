using System.Collections.Generic;


namespace SharedAssemblies.Security.Encryption.DataAccess
{
    /// <summary>
    /// Interface for an security encryption keys
    /// </summary>
    public interface IEncryptionKeyDao
    {
        /// <summary>
        /// Gets the encryption keys from the DB
        /// </summary>
        /// <param name="keys">The key values to retrieve from the database</param>
        /// <returns>The requested encryption keys</returns>
		IEnumerable<KeyContainer> GetEncryptionKeysFromDatabase(IEnumerable<int> keys);

		/// <summary>
		/// Gets the encryption keys from the DB
		/// </summary>
		/// <returns>The current master encryption key</returns>
		byte[] GetMasterKeyFromDatabase();
    }
}
