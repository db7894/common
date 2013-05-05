using System.Collections.Generic;
using System.Text;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Security.Encryption.DataAccess;

namespace SharedAssemblies.Security.Encryption
{
	/// <summary>
	/// Implementation of <see cref="ISimpleEncryption"/> that is mocked out
	/// so it can be quickly substituted for the full implementation for testing.
	/// </summary>
	public sealed class MockSimpleEncryption : ISimpleEncryption
	{
		/// <summary>
		/// Flag to see if the encryption keys have been initialized
		/// </summary>
		public bool IsInitialized { get; private set; }

		/// <summary>
		/// Initializes the encryption library by retrieving the keys
		/// </summary>
		/// <param name="database">The data access object used to retrieve the master key</param>
		/// <returns>True if successful, false otherwise</returns>
		public bool Initialize(IEncryptionKeyDao database)
		{
			return IsInitialized = true;
		}

		#region Encrypt Methods

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public string Encrypt(string data)
		{
			return string.IsNullOrEmpty(data)
				? null
				: data;
		}

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public string Encrypt(string data, IEnumerable<IEnumerable<byte>> keys)
		{
			return string.IsNullOrEmpty(data)
				? null
				: data;
		}

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public byte[] EncryptToByte(string data)
		{
			return string.IsNullOrEmpty(data) 
				? null
				: Encoding.UTF8.GetBytes(data);
		}

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public byte[] EncryptToByte(string data, IEnumerable<IEnumerable<byte>> keys)
		{
			return string.IsNullOrEmpty(data)
				? null
				: Encoding.UTF8.GetBytes(data);
		}

		#endregion

		#region Decrypt Methods

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		public string Decrypt(string data)
		{
			return string.IsNullOrEmpty(data)
				? null
				: data;
		}

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		public string Decrypt(string data, IEnumerable<IEnumerable<byte>> keys)
		{
			return string.IsNullOrEmpty(data)
				? null
				: data;
		}

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		public string Decrypt(byte[] data)
		{
			return data.IsNullOrEmpty()
				? null
				: Encoding.UTF8.GetString(data);
		}

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		public string Decrypt(byte[] data, IEnumerable<IEnumerable<byte>> keys)
		{
			return data.IsNullOrEmpty()
				? null
				: Encoding.UTF8.GetString(data);
		}

		#endregion
	}
}
