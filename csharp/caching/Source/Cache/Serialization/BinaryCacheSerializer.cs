using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace SharedAssemblies.General.Caching.Serialization
{
	/// <summary>
	/// A serializer that converts a type to a byte array.
	/// </summary>
	public class BinaryCacheSerializer : ICacheSerializer
	{
		/// <summary>
		/// Serialize a native type to some other form
		/// </summary>
		/// <typeparam name="TInput">The input type to serialize</typeparam>
		/// <param name="input">The native type to serialize</param>
		/// <returns>The serialized output type</returns>
		public string Serialize<TInput>(TInput input)
		{
			var formatter = new BinaryFormatter();

			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, input);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		/// <summary>
		/// Deserialize some form back to a native type
		/// </summary>
		/// <typeparam name="TInput">The input type to serialize</typeparam>
		/// <param name="output">The serialized output to convert back</param>
		/// <returns>The deserialized native type</returns>
		public TInput Deserialize<TInput>(string output)
		{
			var formatter = new BinaryFormatter();
			var encoding = Convert.FromBase64String(output);

			using (var stream = new MemoryStream(encoding))
			{
				return (TInput)formatter.Deserialize(stream);
			}
		}
	}
}
