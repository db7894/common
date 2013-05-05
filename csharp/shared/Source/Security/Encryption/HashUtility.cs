using System;
using System.Text;
using System.Security.Cryptography;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Security.Encryption
{
	/// <summary>
	/// Helper facade around data hashing to provide for consistent
	/// Bashwork hashing requirements supplied by Security.
	/// </summary>
	public static class HashUtility
	{
		/// <summary>
		/// Generates a fixed 32 byte hash of the input data
		/// </summary>
		/// <param name="input">The data to hash</param>
		/// <param name="salt">An optional salt to hash the data with</param>
		/// <returns>The resulting hash of the given data</returns>
		public static byte[] ComputeHash(byte[] input, byte[] salt=null)
		{
			if (input == null)
			{
				return null;
			}

			var buffer = (salt != null)
				? new byte[input.Length + salt.Length]
				: input;

			if (salt != null)
			{
				Buffer.BlockCopy(salt, 0, buffer, 0, salt.Length);
				Buffer.BlockCopy(input, 0, buffer, salt.Length, input.Length);
			}

			return SHA256.Create().ComputeHash(buffer);
		}

		/// <summary>
		/// Generates a fixed 32 byte hash of the input data
		/// </summary>
		/// <param name="input">The data to hash</param>
		/// <param name="salt">An optional salt to hash the data with</param>
		/// <returns>The resulting hash of the given data</returns>
		public static byte[] ComputeHash(string input, byte[] salt=null)
		{
			return (input != null)
				? ComputeHash(Encoding.UTF8.GetBytes(input), salt)
				: null;
		}

		/// <summary>
		/// Generates a fixed 32 byte hash of the input data
		/// </summary>
		/// <typeparam name="T">The type of item to hash</typeparam>
		/// <param name="input">The data to hash</param>
		/// <param name="salt">An optional salt to hash the data with</param>
		/// <returns>The resulting hash of the given data</returns>
		public static byte[] ComputeHash<T>(T input, byte[] salt = null)
			where T : class
		{
			return (input != null)
				? ComputeHash(input.ToXml(), salt)
				: null;
		}

		/// <summary>
		/// Generates a fixed 32 byte hash of the input data
		/// </summary>
		/// <typeparam name="T">The type of item to hash</typeparam>
		/// <param name="input">The data to hash</param>
		/// <param name="salt">An optional salt to hash the data with</param>
		/// <returns>The resulting hash of the given data</returns>
		public static byte[] ComputeStructHash<T>(T? input, byte[] salt = null)
			where T : struct
		{
			return input.HasValue
				? ComputeHash(input.Value.ToString(), salt)
				: null;
		}

		/// <summary>
		/// Generates a fixed 32 byte hash of the input data
		/// </summary>
		/// <typeparam name="T">The type of item to hash</typeparam>
		/// <param name="input">The data to hash</param>
		/// <param name="salt">An optional salt to hash the data with</param>
		/// <returns>The resulting hash of the given data</returns>
		public static byte[] ComputeStructHash<T>(T input, byte[] salt = null)
			where T : struct
		{
			return ComputeHash(input.ToString(), salt);
		}
	}
}
