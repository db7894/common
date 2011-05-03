using System;
using System.Collections.Generic;
using System.Linq;

namespace Bashwork.Validation.Internal
{
	/// <summary>
	/// A collection of extension methods that are used in this library.
	/// </summary>
	internal static class Extensions
	{
		/// <summary>
		/// Returns the calling collection if it is not null,
		/// or returns an empty collection if null.
		/// </summary>
		/// <typeparam name="TObject">The type of the collection.</typeparam>
		/// <param name="collection">The collection to check.</param>
		/// <returns>The collection if not null or empty collection if null.</returns>
		public static IEnumerable<TObject> NullSafe<TObject>(this IEnumerable<TObject> collection)
		{
			return collection ?? Enumerable.Empty<TObject>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static Tuple<bool, TValue> SafeGet<TKey, TValue>(
			this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue result;
			var exists = dictionary.TryGetValue(key, out result);

			return Tuple.Create(exists, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
			TKey key, TValue error = default(TValue))
		{
			TValue result;
			var exists = dictionary.TryGetValue(key, out result);

			return (exists) ? result : error;
		}
		
		/// <summary>
		/// Returns the opposite value of the specified boolean
		/// </summary>
		/// <param name="condition">The boolean value to not</param>
		/// <returns>true if condition is false, false otherwise</returns>
		public static bool Not(this bool condition)
		{
			return !condition;
		}

		/// <summary>
		/// A helper method to perform a null reference check before operating
		/// on a given object.
		/// </summary>
		/// <param name="context">The object context to perform the null check on</param>
		/// <param name="message">The message to supply in the exception</param>
		public static void Guard(this object context, string message)
		{
			if (context == null)
			{
				throw new ArgumentNullException(message);
			}
		}
	}
}
