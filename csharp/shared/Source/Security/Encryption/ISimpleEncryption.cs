using System.Collections.Generic;
using SharedAssemblies.Security.Encryption.DataAccess;


namespace SharedAssemblies.Security.Encryption
{
    /// <summary>
    /// Interface to a simple encryption utility
    /// </summary>
    public interface ISimpleEncryption
    {
        /// <summary>
        /// Flag to see if the encryption keys have been initialized
        /// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Initializes the encryption library by retrieving the keys
		/// </summary>
		/// <param name="database">The data access object used to retrieve the master key</param>
		/// <returns>True if successful, false otherwise</returns>
		bool Initialize(IEncryptionKeyDao database);

        /// <summary>
        /// Encrypts the passed in data using the initialized key manager.
        /// </summary>
        /// <param name="data">The data to decrypt</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
        string Encrypt(string data);

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		string Encrypt(string data, IEnumerable<IEnumerable<byte>> keys);

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		byte[] EncryptToByte(string data);

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		byte[] EncryptToByte(string data, IEnumerable<IEnumerable<byte>> keys);

        /// <summary>
        /// Decrypts the passed in data using the initialized key manager
        /// </summary>
        /// <param name="data">The data to decrypt</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
        string Decrypt(string data);

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		string Decrypt(string data, IEnumerable<IEnumerable<byte>> keys);

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		string Decrypt(byte[] data);

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		string Decrypt(byte[] data, IEnumerable<IEnumerable<byte>> keys);
    }
}