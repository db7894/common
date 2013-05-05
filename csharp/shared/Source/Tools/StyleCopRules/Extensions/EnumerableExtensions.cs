using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// An enumerable extension to provide null-safe count.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Provides a null-safe count of a collection.
        /// </summary>
        /// <typeparam name="T">Type of collection.</typeparam>
        /// <param name="collection">The collection to check.</param>
        /// <returns>Count in collection or zero if null.</returns>
        public static int NullSafeCount<T>(this IEnumerable<T> collection)
        {
            return collection != null ? collection.Count() : 0;
        }


        /// <summary>
        /// Summarizes a collection into a string.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="collection">The collection of items.</param>
        /// <param name="stringifier">The lambda to convert T to string.</param>
        /// <param name="depth">The depth to dive into the collection.</param>
        /// <returns>String version.</returns>
        public static string Summarize<T>(this IEnumerable<T> collection, Func<T, string> stringifier, 
            int depth)
        {
            int count = collection.NullSafeCount();
            int diveTo = Math.Min(depth, count);

            var builder = new StringBuilder("(" + count);

            if(collection != null)
            {
                int current = 0;
                foreach(var item in collection)
                {
                    if(current++ < depth)
                    {
                        builder.Append(" {");
                        builder.Append(stringifier(item));
                        builder.Append("}");
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return builder.Append(')').ToString();
        }

        
        /// <summary>
        /// Summarizes a collection into a string.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="collection">The collection of items.</param>
        /// <param name="depth">The depth to dive into the collection.</param>
        /// <returns>String version.</returns>
        public static string Summarize<T>(this IEnumerable<T> collection, int depth)
        {
            return Summarize(collection, item => item.ToString(), depth);
        }


        /// <summary>
        /// Returns the last item int he collection or default if empty.
        /// </summary>
        /// <typeparam name="T">Type of items in the collection.</typeparam>
        /// <param name="collection">The colleciton to scan.</param>
        /// <returns>The last item or the default if last is not found.</returns>
        public static T LastOrDefault<T>(this IEnumerable<T> collection)
        {
            T result = default(T);

            foreach(var item in collection)
            {
                result = item;
            }

            return result;
        }
    }
}
