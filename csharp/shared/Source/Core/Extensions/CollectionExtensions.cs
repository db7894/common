using System.Collections.Generic;
using System.Linq;

namespace SharedAssemblies.Core.Extensions
{
    /// <summary>
    /// A collection of reusable extension methods
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Extension to make sure collection is not empty
        /// </summary>
        /// <typeparam name="T">The type of item the collection contains.</typeparam>
        /// <param name="input">The collection to test</param>
        /// <returns>True if the collection is not empty</returns>
        public static bool IsNotEmpty<T>(this ICollection<T> input)
        {
            return (input.Count > 0);
        }
        
        /// <summary>
        /// Extension to check if empty if you don't care about it being null.
        /// </summary>
        /// <typeparam name="T">The type of item the collection contains.</typeparam>
        /// <param name="input">The collection to test</param>
        /// <returns>True if the collection is empty</returns>
        public static bool IsEmpty<T>(this ICollection<T> input)
        {
            return (input.Count == 0);
        }

        /// <summary>
        /// Extension to mimic the string method
        /// </summary>
        /// <typeparam name="T">The type of item the collection contains.</typeparam>
        /// <param name="input">The collection to test</param>
        /// <returns>True if the collection is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> input)
        {
            return (input == null) || (input.Count == 0);
        }
        
        /// <summary>
        /// Extension to make sure the collection is not null and is not empty
        /// </summary>
        /// <typeparam name="T">The type of item the collection contains.</typeparam>
        /// <param name="input">The collection to test</param>
        /// <returns>True if the collection is not null or empty</returns>
        public static bool IsNotNullOrEmpty<T>(this ICollection<T> input)
        {
            return (input != null) && (input.Count > 0);
        }

		/// <summary>
		/// Returns the count of the collection or null if the collection instance is null.
		/// </summary>
		/// <typeparam name="T">Type of item stored in the collection.</typeparam>
		/// <param name="collection">The collection to check.</param>
		/// <returns>The count of the collection, or zero if null.</returns>
		/// <remarks>Version 1.2</remarks>
		public static int NullSafeCount<T>(this ICollection<T> collection)
		{
			return collection != null ? collection.Count : 0;
		}
	}
}