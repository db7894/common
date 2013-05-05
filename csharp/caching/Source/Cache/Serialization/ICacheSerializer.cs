
namespace SharedAssemblies.General.Caching.Serialization
{
	/// <summary>
	/// An interface to a method of serializing and deserializing
	/// a type for easy storage to another medium.
	/// </summary>
	public interface ICacheSerializer
	{
		/// <summary>
		/// Serialize a native type to some other form
		/// </summary>
		/// <typeparam name="TInput">The input type to serialize</typeparam>
		/// <param name="input">The native type to serialize</param>
		/// <returns>The serialized output type</returns>
		string Serialize<TInput>(TInput input);

		/// <summary>
		/// Deserialize some form back to a native type
		/// </summary>
		/// <typeparam name="TInput">The input type to serialize</typeparam>
		/// <param name="output">The serialized output to convert back</param>
		/// <returns>The deserialized native type</returns>
		TInput Deserialize<TInput>(string output);
	}
}
