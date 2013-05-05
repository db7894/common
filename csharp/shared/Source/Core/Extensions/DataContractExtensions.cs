using SharedAssemblies.Core.DataContracts;


namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Extension method for any type to convert them to JSON and XML using DataContractSerializers.
	/// </summary>
	public static class DataContractExtensions
	{
		/// <summary>
		/// Converts an object to JSON (DataContractJsonSerializer flavor).
		/// </summary>
		/// <typeparam name="T">Type of object to serialize.</typeparam>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The JSON string representation of the object.</returns>
		public static string ToDataContractJson<T>(this T objectToSerialize)
		{
			return DataContractUtility.ToJson<T>(objectToSerialize);
		}

		/// <summary>
		/// Converts an object to XML (DataContractSerializer flavor).
		/// </summary>
		/// <typeparam name="T">Type of object to serialize.</typeparam>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The XML string representation of the object.</returns>
		public static string ToDataContractXml<T>(this T objectToSerialize)
		{
			return DataContractUtility.ToXml<T>(objectToSerialize);
		}
	}
}
