using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;


namespace SharedAssemblies.Core.DataContracts
{
	/// <summary>
	/// Class that implements serialization to and from JSON or XML to object.
	/// </summary>
	public static class DataContractUtility
	{
		/// <summary>
		/// Serializes an object to XML.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The XML string in UTF8 encoding.</returns>
		public static string ToXml<T>(T objectToSerialize)
		{
			return Serialize(new DataContractSerializer(typeof(T)), objectToSerialize);
		}

		/// <summary>
		/// Serializes an object to JSON.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The JSON string in UTF8 encoding.</returns>
		public static string ToJson<T>(T objectToSerialize)
		{
			return Serialize(new DataContractJsonSerializer(typeof(T)), objectToSerialize);
		}

		/// <summary>
		/// Serializes an object to XML.
		/// </summary>
		/// <param name="objectType">The type of object to serialize.</param>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The XML string in UTF8 encoding.</returns>
		public static string ToXml(Type objectType, object objectToSerialize)
		{
			return Serialize(new DataContractSerializer(objectType), objectToSerialize);
		}

		/// <summary>
		/// Serializes an object to JSON.
		/// </summary>
		/// <param name="objectType">The type of object to serialize.</param>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The JSON string in UTF8 encoding.</returns>
		public static string ToJson(Type objectType, object objectToSerialize)
		{
			return Serialize(new DataContractJsonSerializer(objectType), objectToSerialize);
		}

		/// <summary>
		/// Deserializes an XML string into an instance of type T.
		/// </summary>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		/// <param name="serializedText">The string to deserialize.</param>
		/// <returns>The deserialized object.</returns>
		public static T FromXml<T>(string serializedText)
		{
			return (T)Deserialize(new DataContractSerializer(typeof(T)), serializedText);
		}

		/// <summary>
		/// Deserializes a JSON string into an instance of type T.
		/// </summary>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		/// <param name="serializedText">The string to deserialize.</param>
		/// <returns>The deserialized object.</returns>
		public static T FromJson<T>(string serializedText)
		{
			return (T)Deserialize(new DataContractJsonSerializer(typeof(T)), serializedText);
		}

		/// <summary>
		/// Serializes an object using the object serializer to a UTF8 string.
		/// </summary>
		/// <param name="serializer">Object serializer</param>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The serialized string.</returns>
		public static string Serialize(XmlObjectSerializer serializer, object objectToSerialize)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}

			using (var buffer = new MemoryStream())
			{
				serializer.WriteObject(buffer, objectToSerialize);

				return Encoding.UTF8.GetString(buffer.ToArray());
			}
		}

		/// <summary>
		/// Deserializes an object using the object serializer and a UTF8 source string.
		/// </summary>
		/// <param name="serializer">Object serializer.</param>
		/// <param name="serializedText">Serialized text to deserialized.</param>
		/// <returns>The deserialized object.</returns>
		public static object Deserialize(XmlObjectSerializer serializer, string serializedText)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}

			using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(serializedText)))
			{
				return serializer.ReadObject(buffer);
			}
		}
	}
}
