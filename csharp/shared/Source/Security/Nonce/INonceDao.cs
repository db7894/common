
namespace SharedAssemblies.Security.Nonce
{
	/// <summary>
	/// Interface for a data access object for the Nonce assembly
	/// </summary>
	public interface INonceDao
	{
		/// <summary>
		/// Store a value and get a key to retrieve it with
		/// </summary>
		/// <param name="data">The data to store</param>
		/// <returns>The key associated with the data</returns>
		string Store(string data);

		/// <summary>
		/// Store a value and get a key to retrieve it with
		/// </summary>
		/// <typeparam name="T">The type of data to serialize</typeparam>
		/// <param name="data">The data to store</param>
		/// <returns>The key associated with the data</returns>
		string Store<T>(T data);

		/// <summary>
		/// Retrieve data associated with the given key
		/// </summary>
		/// <param name="name">The key to retrieve data for</param>
		/// <returns>The data associated with the key</returns>
		string Retrieve(string name);

		/// <summary>
		/// Retrieve data associated with the given key
		/// </summary>
		/// <typeparam name="T">The type of data to serialize</typeparam>
		/// <param name="name">The key to retrieve data for</param>
		/// <returns>The data associated with the key</returns>
		T Retrieve<T>(string name);
	}
}
