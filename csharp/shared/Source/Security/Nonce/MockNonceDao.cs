using System.Collections.Generic;
using SharedAssemblies.Core.Xml;
using SharedAssemblies.Security.Encryption;


namespace SharedAssemblies.Security.Nonce
{
	/// <summary>
	/// Implementation of INonceDao backed with a Mocked database
	/// </summary>
	public sealed class MockNonceDao : INonceDao
	{
        #region private members

		/// <summary>
		/// The handle to our database for storing keys
		/// </summary>
		private readonly Dictionary<string, string> _database =
			new Dictionary<string, string>();

        #endregion

		#region Implementation of INonceDao

		/// <summary>
		/// Store a value and get a key to retrieve it with
		/// </summary>
		/// <param name="data">The data to store</param>
		/// <returns>The key associated with the data</returns>
		public string Store(string data)
		{
			string result = null;

			if (!string.IsNullOrEmpty(data))
			{
				result = EncryptionUtility.GenerateRandomStringToken(24);
				_database[result] = data;
			}

			return result;
		}

		/// <summary>
		/// Store a value and get a key to retrieve it with
		/// </summary>
		/// <typeparam name="T">The type of data to serialize</typeparam>
		/// <param name="data">The data to store</param>
		/// <returns>The key associated with the data</returns>
		public string Store<T>(T data)
		{
			return Store(XmlUtility.XmlFromType(data));
		}

		/// <summary>
		/// Retrieve data associated with the given key
		/// </summary>
		/// <param name="name">The key to retrieve data for</param>
		/// <returns>The data associated with the key</returns>
		public string Retrieve(string name)
		{
			string result;

			if (_database.TryGetValue(name, out result))
			{
				_database.Remove(name);
			}

			return result;
		}

		/// <summary>
		/// Retrieve data associated with the given key
		/// </summary>
		/// <typeparam name="T">The type of data to serialize</typeparam>
		/// <param name="name">The key to retrieve data for</param>
		/// <returns>The data associated with the key</returns>
		public T Retrieve<T>(string name)
		{
			string result = Retrieve(name);

			return string.IsNullOrEmpty(result)
				? default(T)
				: XmlUtility.TypeFromXml<T>(result);
		}

		#endregion
	}
}
