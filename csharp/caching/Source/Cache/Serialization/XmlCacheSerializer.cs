using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SharedAssemblies.General.Caching.Serialization
{
	/// <summary>
	/// A serializer that converts a type to an xml string.
	/// </summary>
	public class XmlCacheSerializer : ICacheSerializer
	{
		/// <summary>
		/// Serialize a native type to some other form
		/// </summary>
		/// <typeparam name="TInput">The input type to serialize</typeparam>
		/// <param name="input">The native type to serialize</param>
		/// <returns>The serialized output type</returns>
		public string Serialize<TInput>(TInput input)
		{
			var serializer = new XmlSerializer(input.GetType());

			using (var stream = new MemoryStream())
			using (var writer = new XmlTextWriter(stream, new UTF8Encoding()))
			{
				serializer.Serialize(writer, input);
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
			var serializer = new XmlSerializer(typeof(TInput));

			using (var stream = new StringReader(output))
			using (var reader = new XmlTextReader(stream))
			{
				return (TInput)serializer.Deserialize(reader);
			}
		}
	}
}
