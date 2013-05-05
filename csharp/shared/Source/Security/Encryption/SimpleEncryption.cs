using System;
using System.Collections.Generic;
using System.Text;
using SharedAssemblies.Security.Encryption.DataAccess;

namespace SharedAssemblies.Security.Encryption
{
	/// <summary>
	/// A facade around implementing encryption. This takes care of key management,
	///  encrypting, and decrypting data with a simple interface.
	/// </summary>
	/// <code>
	/// <para>
	/// // First build the data access
	/// var connection = Configuration["EncryptionConnection"];
	/// var database = new SqlEncryptionKeyDao(ClientProviderType.SqlServer, connection);
    /// </para>
    /// <para>
	/// // Then create your key manager provider
	/// var identifiers = new List&lt;int&gt; { 12, 27, 349 };
	/// var manager = new SqlKeyManager(database, identifiers);
    /// </para>
    /// <para>
	/// // Then construct and initialize the simple encryption manager
	/// var encryption = new SimpleEncryption(manager);
	/// if (encryption.Initialize(database))
	/// {
	///		Console.WriteLine(encryption.Encrypt("Successul Encryption"));
	///	}
    /// </para>
	/// </code>
	public sealed class SimpleEncryption : ISimpleEncryption
	{
		#region Private Fields

		/// <summary>
		/// Handle to the data access object for security components
		/// </summary>
		private readonly AbstractKeyManager _keyManager;

		#endregion

		/// <summary>
		/// Flag to see if the encryption keys have been initialized
		/// </summary>
		public bool IsInitialized { get; private set; }

		/// <summary>
		/// Initializes a new instance of the SimpleEncryption class
		/// </summary>
		/// <param name="keyManager">The <see cref="AbstractKeyManager"/> to use for generating keys</param>
		public SimpleEncryption(AbstractKeyManager keyManager)
		{
			_keyManager = keyManager;
		}

		/// <summary>
		/// Initializes the encryption library by retrieving the keys
		/// </summary>
		/// <param name="database">The data access object used to retrieve the master key</param>
		/// <returns>True if successful, false otherwise</returns>
		public bool Initialize(IEncryptionKeyDao database)
		{
			if (!IsInitialized
				&& (_keyManager != null) && (database != null))
			{
				IsInitialized = _keyManager.Initialize(database);
			}

			return IsInitialized;
		}

		#region Encrypt Methods

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public string Encrypt(string data)
		{
			string encryptedData = null;
			var result = EncryptToByte(data);

			if (result != null)
			{
				encryptedData = Convert.ToBase64String(result);
			}

			return encryptedData;
		}

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public string Encrypt(string data, IEnumerable<IEnumerable<byte>> keys)
		{
			string encryptedData = null;
			var result = EncryptToByte(data, keys);

			if (result != null)
			{
				encryptedData = Convert.ToBase64String(result);
			}
			
			return encryptedData;
		}

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public byte[] EncryptToByte(string data)
		{
			byte[] encryptedData = null;

			if (IsInitialized)
			{
				encryptedData = EncryptionUtility.Encrypt(
					Encoding.UTF8.GetBytes(data ?? string.Empty),
					_keyManager.GetKey().EncryptionKey);
			}

			return encryptedData;
		}

		/// <summary>
		/// Encrypts the passed in data using the initialized key manager.
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The encrypted data if successful, null if failure</returns>
		public byte[] EncryptToByte(string data, IEnumerable<IEnumerable<byte>> keys)
		{
			byte[] encryptedData = null;

			if (IsInitialized)
			{
				encryptedData = EncryptionUtility.Encrypt(
					Encoding.UTF8.GetBytes(data ?? string.Empty),
					_keyManager.GetKey(keys).EncryptionKey);
			}

			return encryptedData;
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
			return Decrypt(Convert.FromBase64String(data ?? string.Empty));
		}

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		public string Decrypt(string data, IEnumerable<IEnumerable<byte>> keys)
		{
			return Decrypt(Convert.FromBase64String(data ?? string.Empty), keys);
		}

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		public string Decrypt(byte[] data)
		{
			string decryptedData = null;

			if (IsInitialized)
			{
				var result = EncryptionUtility.Decrypt(data,
					_keyManager.GetKey().EncryptionKey);

				if (result != null)
				{
					decryptedData = Encoding.UTF8.GetString(result);
				}
			}

			return decryptedData;
		}

		/// <summary>
		/// Decrypts the passed in data using the initialized key manager
		/// </summary>
		/// <param name="data">The data to decrypt</param>
		/// <param name="keys">Additional key components to use</param>
		/// <returns>The decrypted data if successful, null if failure</returns>
		public string Decrypt(byte[] data, IEnumerable<IEnumerable<byte>> keys)
		{
			string decryptedData = null;

			if (IsInitialized)
			{
				var result = EncryptionUtility.Decrypt(data,
					_keyManager.GetKey(keys).EncryptionKey);

				if (result != null)
				{
					decryptedData = Encoding.UTF8.GetString(result);
				}
			}

			return decryptedData;
		}

		#endregion
	}
}