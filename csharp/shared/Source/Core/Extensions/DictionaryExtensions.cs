using System;
using System.Collections.Generic;
using SharedAssemblies.Core.Containers;

namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// A collection of extension methods that are used in this library.
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Returns an operation result for the specified dictionary request
		/// </summary>
		/// <typeparam name="TKey">The key value of the dictionary</typeparam>
		/// <typeparam name="TValue">The value type of the dictionary</typeparam>
		/// <param name="dictionary">The dictionary instance to query</param>
		/// <param name="key">The key to retrieve the value for</param>
		/// <returns>The result of the operation</returns>
		public static OperationResult<TValue> SafeGet<TKey, TValue>(
			this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			dictionary.Guard("dictionary");

			TValue result;
			var exists = dictionary.TryGetValue(key, out result);

			return OperationResult.Create(exists, result);
		}

		/// <summary>
		/// Returns the value at the specified key or a default value
		/// </summary>
		/// <typeparam name="TKey">The key value of the dictionary</typeparam>
		/// <typeparam name="TValue">The value type of the dictionary</typeparam>
		/// <param name="dictionary">The dictionary instance to query</param>
		/// <param name="key">The key to retrieve the value for</param>
		/// <param name="error">The default value to return if the key doesn't exist</param>
		/// <returns>The result of the operation</returns>
		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
			TKey key, TValue error = default(TValue))
		{
			dictionary.Guard("dictionary");

			TValue result;
			var exists = dictionary.TryGetValue(key, out result);

			return (exists) ? result : error;
		}
	}
}
