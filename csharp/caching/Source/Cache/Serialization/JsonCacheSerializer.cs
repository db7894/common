using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace SharedAssemblies.General.Caching.Serialization
{
	/// <summary>
	/// A serializer that converts a given type to a json string.
	/// </summary>
	public class JsonCacheSerializer : ICacheSerializer
	{
		/// <summary>
		/// Serialize a native type to some other form
		/// </summary>
		/// <typeparam name="TInput">The input type to serialize</typeparam>
		/// <param name="input">The native type to serialize</param>
		/// <returns>The serialized output type</returns>
		public string Serialize<TInput>(TInput input)
		{
			var serializer = new DataContractJsonSerializer(typeof(TInput));

			using (var stream = new MemoryStream())
			{
				serializer.WriteObject(stream, input);

				return Encoding.UTF8.GetString(stream.ToArray());
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
			var serializer = new DataContractJsonSerializer(typeof(TInput));
			var encoding = Encoding.UTF8.GetBytes(output);

			using (var stream = new MemoryStream(encoding))
			{
				return (TInput)serializer.ReadObject(stream);
			}
		}
	}
}
