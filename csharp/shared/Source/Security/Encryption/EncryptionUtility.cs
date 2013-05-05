using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Security.Encryption
{
	/// <summary>
	/// Encryption and Message Authentication static class.
	/// </summary>
	public static class EncryptionUtility
	{
		/// <summary>
		/// Randomly generate a random number as a string
		/// </summary>
		/// <param name="numberOfBytes">1 to X bytes.</param>
		/// <returns>The random number as a string</returns>
		public static string GenerateRandomStringToken(int numberOfBytes)
		{
			string result = null;
			byte[] randomToken = GenerateRandomByteToken(numberOfBytes);

			if (!randomToken.IsNullOrEmpty())
			{
				// the replace makes the random string url-parameter safe
				result = Convert.ToBase64String(randomToken)
					.Substring(0, numberOfBytes)
					.Replace('+', '-').Replace('/', '_');
			}
			return result;
		}

		/// <summary>
		/// Randomly generate an array of bytes. Can be used to generate encryption keys.
		/// </summary>
		/// <param name="numberOfBytes">1 to X bytes.</param>
		/// <returns>byte array or null if error</returns>
		public static byte[] GenerateRandomByteToken(int numberOfBytes)
		{
			byte[] randomToken = null;

			if (numberOfBytes > 0)
			{
				randomToken = new byte[numberOfBytes];
				RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();
				generator.GetBytes(randomToken);
			}
			return randomToken;
		}

		/// <summary>
		/// Randonly generates a random +integer number
		/// </summary>
		/// <returns>A new random number</returns>
		public static int GenerateRandomInteger()
		{
			byte[] randomToken = GenerateRandomByteToken(4);

			int result = BitConverter.ToInt32(randomToken, 0);
			return (result < 0) ? ~result + 1 : result;
		}

		/// <summary>
		/// Encrypt data using the specified encryption key.
		/// </summary>
		/// <param name="dataToEncrypt">Data to encrypt.</param>
		/// <param name="encryptionKey">Encryption key.</param>
		/// <returns>Encrypted data or null if error.</returns>
		public static byte[] Encrypt(byte[] dataToEncrypt, byte[] encryptionKey)
		{
			byte[] encryptedData = null;

			if (!dataToEncrypt.IsNullOrEmpty() && !encryptionKey.IsNullOrEmpty())
			{
				var encryptor = EncryptionImplementation(encryptionKey).CreateEncryptor();
				encryptedData = Transform(dataToEncrypt, encryptor);
			}

			return encryptedData;
		}

		/// <summary>
		/// Encrypt data using the specified encryption key.
		/// </summary>
		/// <param name="input">Data to encrypt.</param>
		/// <param name="key">Encryption key.</param>
		/// <returns>Encrypted data or null if error.</returns>
		public static string Encrypt(string input, string key)
		{
			string result = null;

			if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(key))
			{
				var encryptedData = Encrypt(Encoding.UTF8.GetBytes(input),
					Convert.FromBase64String(key));
				result = Convert.ToBase64String(encryptedData);
			}
			return result;
		}

		/// <summary>
		/// Decrypt data using the specified encryption key.
		/// </summary>
		/// <param name="dataToDecrypt">Data to decrypt.</param>
		/// <param name="encryptionKey">Encryption key.</param>
		/// <returns>String data or null if error.</returns>
		public static byte[] Decrypt(byte[] dataToDecrypt, byte[] encryptionKey)
		{
			byte[] decryptedData = null;

			if (!dataToDecrypt.IsNullOrEmpty() && !encryptionKey.IsNullOrEmpty())
			{
				var decryptor = EncryptionImplementation(encryptionKey).CreateDecryptor();
				decryptedData = Transform(dataToDecrypt, decryptor);
			}

			return decryptedData;
		}

		/// <summary>
		/// Decrypt data using the specified encryption key.
		/// </summary>
		/// <param name="input">Data to decrypt.</param>
		/// <param name="key">Encryption key.</param>
		/// <returns>String data or null if error.</returns>
		public static string Decrypt(string input, string key)
		{
			string result = null;

			if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(key))
			{
				var decryptedData = Decrypt(Convert.FromBase64String(input),
					Convert.FromBase64String(key));
				result = Encoding.UTF8.GetString(decryptedData);
			}
			return result;
		}

		/// <summary>
		/// Generate message authentication code using the HMAC algorithm. 
		/// </summary>
		/// <param name="data">Binary Data used to generate an authentication code.</param>
		/// <param name="validationKey">Validation key used by the HMAC algorithm.</param>
		/// <returns>HMAC or null if error.</returns>
		public static byte[] GenerateAuthenticationCode(byte[] data, byte[] validationKey)
		{
			byte[] authCode = null;

			if (!data.IsNullOrEmpty() && !validationKey.IsNullOrEmpty())
			{
				HMACSHA1 hmac = new HMACSHA1 { Key = validationKey };
				authCode = hmac.ComputeHash(data);
			}
			return authCode;
		}

		/// <summary>
		/// Generate message authentication code using the HMAC algorithm.
		/// </summary>
		/// <param name="input">String Data used to generate an authentication code.</param>
		/// <param name="key">Validation key used by the HMAC algorithm.</param>
		/// <returns>HMAC or null if error.</returns>
		public static string GenerateAuthenticationCode(string input, string key)
		{
			string result = null;

			if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(key))
			{
				var authCode = GenerateAuthenticationCode(Encoding.UTF8.GetBytes(input),
					Convert.FromBase64String(key));
				result = Convert.ToBase64String(authCode);
			}
			return result;
		}

		#region Private Methods
        
		/// <summary>
		/// Helper method to abstract initializing and creating the
		/// encryption algorithm implementation.
		/// </summary>
		/// <param name="encryptionKey">The encryption key to initialize with</param>
		/// <returns>The encryption algorithm implementation to use</returns>
		private static SymmetricAlgorithm EncryptionImplementation(byte[] encryptionKey)
		{
			byte[] keyHash = new SHA256Managed().ComputeHash(encryptionKey);

			return new AesManaged
			{
				Padding = PaddingMode.PKCS7,
				Mode = CipherMode.CBC,
				Key = encryptionKey,
				IV = keyHash.Take(16).ToArray(),
			};
		}

		/// <summary>
		/// Helper method to abstract the crypto transformation process
		/// </summary>
		/// <param name="data">The data to transform</param>
		/// <param name="transform">The transformation to apply to the data</param>
		/// <returns>The result of the transformation process</returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5005:NoEmptyCatchBlocks",
			Justification = "Initialization of the key manager failures should not bleed information.")]
		private static byte[] Transform(byte[] data, ICryptoTransform transform)
		{
			byte[] result = null;

			try
			{
				using (MemoryStream stream = new MemoryStream())
				{
					using (CryptoStream crypto = new CryptoStream(stream, transform,
						CryptoStreamMode.Write))
					{
						crypto.Write(data, 0, data.Length);
					}
					result = stream.ToArray(); // must let the crypto stream dispose!
				}
			}
			catch (CryptographicException)
			{
				// This is allowed to prevent secure information from escaping
			}

			return result;
		}

		#endregion
	}
}
