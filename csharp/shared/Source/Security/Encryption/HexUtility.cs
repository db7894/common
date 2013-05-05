using System.Text;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Security.Encryption
{
	/// <summary>
	/// Helper class to convert to and from hex strings
	/// </summary>
	public static class HexUtility
	{
		#region Private Constants

		/// <summary>
		/// The number of bytes in a single hex character
		/// </summary>
		private const int _bytesInHexCharacter = 2;

		/// <summary>
		/// The number of bits in base-16
		/// </summary>
		private const int _bitsInBase16 = 16;

		#endregion

		/// <summary>
		/// Convert a binary byte array to a hex string
		/// </summary>
		/// <param name="input">The binary byte array to convert</param>
		/// <returns>The resulting hex string or string.Empty if error</returns>
		public static string Convert(byte[] input)
		{
			string hexString = null;

			if (!input.IsNullOrEmpty())
			{
				var builder = new StringBuilder(input.Length * _bytesInHexCharacter);
				foreach (byte b in input)
				{
					builder.AppendFormat("{0:x2}", b);
				}
				hexString = builder.ToString();
			}

			return hexString;
		}

		/// <summary>
		/// Convert a hex string to a binary byte array
		/// </summary>
		/// <param name="input">The hex string to convert</param>
		/// <returns>The resulting binary byte array or null if error</returns>
		public static byte[] Convert(string input)
		{
			byte[] result = null;

			if (!string.IsNullOrEmpty(input))
			{
				int length = input.Length; 
				result = new byte[length / _bytesInHexCharacter];

				for (int i = 0; i < length; i += _bytesInHexCharacter)
				{
					result[i / _bytesInHexCharacter] = System.Convert.ToByte(input.Substring(i,
						_bytesInHexCharacter), _bitsInBase16);
				}
			}

			return result;
		}
	}
}
