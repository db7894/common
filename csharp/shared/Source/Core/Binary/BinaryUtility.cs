using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace SharedAssemblies.Core.Binary
{
	/// <summary>
	/// A collection of XML Utility classes
	/// </summary>
	public static class BinaryUtility
	{
		/// <summary>
		/// Serialize an object into an binary byte[]
		/// </summary>
		/// <param name="input">The object to serialize.</param>
		/// <returns>The binary result in a byte[].</returns>
		public static byte[] ToBinary(object input)
		{
			var formatter = new BinaryFormatter();

			using (var memoryStream = new MemoryStream())
			{
				formatter.Serialize(memoryStream, input);

				return memoryStream.ToArray();
			}
		}


		/// <summary>
		/// Deserialize an object from a binary byte[]
		/// </summary>
		/// <typeparam name="T">The type to read from the XML.</typeparam>
		/// <param name="binary">XML of objet</param>
		/// <returns>The result</returns>
		public static T ToType<T>(byte[] binary)
		{
			return (T)ToObject(binary);
		}


		/// <summary>
		/// Deserialize an object from a binary byte[]
		/// </summary>
		/// <param name="binary">XML of objet</param>
		/// <returns>The result</returns>
		public static object ToObject(byte[] binary)
		{
			var formatter = new BinaryFormatter();

			using (var stream = new MemoryStream(binary))
			{
				return formatter.Deserialize(stream);
			}
		}
	}
}