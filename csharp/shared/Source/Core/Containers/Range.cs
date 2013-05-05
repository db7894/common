using System;

namespace SharedAssemblies.Core.Containers
{
    /// <summary>
    /// A class for a range of comparable objects.
    /// </summary>
    /// <typeparam name="T">The type to compare.</typeparam>
    public class Range<T> where T : IComparable
    {
		/// <summary>
		/// The start of the range.
		/// </summary>
		public T Start { get; set; }

		/// <summary>
		/// The end of the range.
		/// </summary>
		public T End { get; set; }
		
		/// <summary>
        /// Default empty constructor. Not ideal to have one, but needed for serialization.
        /// </summary>
        public Range()
        {
            Start = default(T);
            End = default(T);
        }

        /// <summary>
        /// Constructor to define start and end of range.
        /// </summary>
        /// <param name="start">Start of the range (inclusive).</param>
        /// <param name="end">End of the range (inclusive).</param>
        public Range(T start, T end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Determines if the specified value is in the range or not.
        /// </summary>
        /// <param name="value">The value to determine if is in range.</param>
        /// <returns>True if in range, False otherwise.</returns>
        public bool IsInRange(T value)
        {
            return value.CompareTo(Start) >= 0 && value.CompareTo(End) <= 0;
        }
    }
}