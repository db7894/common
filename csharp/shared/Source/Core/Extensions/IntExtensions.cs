using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedAssemblies.Core.Extensions
{
    /// <summary>
    /// A set of integer generator extension methods
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Begins counting to inifity, use To() to range this.
        /// </summary>
        /// <param name="start">Place to begin count</param>
        /// <returns>Iterator of current count</returns>
        public static IEnumerable<int> Every(this int start)
        {
			// deliberately avoiding condition because keeps going
			// to infinity for as long as values are pulled.
			for (var i = start;; ++i)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Begins counting to infinity by the given step value, use To() to
        /// range this.
        /// </summary>
        /// <param name="start">Place to start count.</param>
        /// <param name="byEvery">Amount to skip by.</param>
        /// <returns>Iterator of current count</returns>
        public static IEnumerable<int> Every(this int start, int byEvery)
        {
			// deliberately avoiding condition because keeps going
			// to infinity for as long as values are pulled.
            for (var i = start;; i += byEvery)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Begins counting to inifity, use To() to range this.
        /// </summary>
        /// <param name="start">Place to begin count.</param>
        /// <param name="end">Place to end count.</param>
        /// <returns>Iterator of current count.</returns>
        public static IEnumerable<int> To(this int start, int end)
        {
            for (var i = start; i <= end; ++i)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Ranges the count by specifying the upper range of the count.
        /// </summary>
        /// <param name="collection">The collection to iterate.</param>
        /// <param name="end">The item to end after.</param>
        /// <returns>Iterator of current count</returns>
        public static IEnumerable<int> To(this IEnumerable<int> collection, int end)
        {
            return collection.TakeWhile(item => item <= end);
        }

		/// <summary>
		/// Returns the range of values in the enumeration.  This returns a Tuple with Item1 being the minimum value and 
		/// Item2 being the maximum value in the enumeration.
		/// </summary>
		/// <param name="source">The enumeration.</param>
		/// <returns>A Tuple containing the min and max value of the sequence.</returns>
		public static Tuple<int, int> GetRange(this IEnumerable<int> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			// define min and max values
			int min;
			int max;

			// get the default comparer for the source type, and default min and max to the first (or default) value.
			min = max = source.FirstOrDefault();

			// go through all other values and see if any are smaller or larger
			// could have done Skip(1) but turned out to be SLOWER than just comparing first even though no-op.
			foreach (var item in source)
			{
				if (item < min)
				{
					min = item;
				}

				if (item > max)
				{
					max = item;
				}
			}

			return Tuple.Create(min, max);
		}
	}
}
