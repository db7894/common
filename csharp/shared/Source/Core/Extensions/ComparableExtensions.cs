using System;


namespace SharedAssemblies.Core.Extensions
{
    /// <summary>
    /// A set of IComparable extension methods.
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        /// Checks two comparables to see if a value is between the 
        /// high and low value.  The comparison parameter instructs the comparison as to
        /// whether or not to check for inclusive or exclusive.
        /// </summary>
        /// <typeparam name="T">A type that implements IComparable&lt;T&gt;"/>.</typeparam>
        /// <param name="value">Value to compare.</param>
        /// <param name="low">The low value, inclusive.</param>
        /// <param name="high">The high value, inclusive.</param>
        /// <param name="comparison">Determines if IsBetween is inclusive or exclusive.</param>
        /// <returns>True if value is between low and high.</returns>
        public static bool IsBetween<T>(this T value, T low, T high, BetweenComparison comparison) 
            where T : IComparable<T>
        {
            return comparison == BetweenComparison.Inclusive
                       ? value.CompareTo(low) >= 0 && value.CompareTo(high) <= 0
                       : value.CompareTo(low) > 0 && value.CompareTo(high) < 0;
        }

        
        /// <summary>
        /// Checks two comparables to see if a value is inclusively between the 
        /// high and low value.  
        /// </summary>
        /// <typeparam name="T">A type that implements IComparable&lt;T&gt;"/>.</typeparam>
        /// <param name="value">Value to compare.</param>
        /// <param name="low">The low value, inclusive.</param>
        /// <param name="high">The high value, inclusive.</param>
        /// <returns>True if value is between low and high.</returns>
        public static bool IsBetween<T>(this T value, T low, T high)
            where T : IComparable<T>
        {
            return value.CompareTo(low) >= 0 && value.CompareTo(high) <= 0;
        }

		/// <summary>
		/// This constrains a given input value to the specified high and low range.
		/// </summary>
		/// <typeparam name="T">The type of the element to constrain</typeparam>
		/// <param name="input">The value to constrain</param>
		/// <param name="low">The low value to constrain at</param>
		/// <param name="high">The high value to constrain at</param>
		/// <returns>The result of the operation</returns>
		public static T ConstrainTo<T>(this T input, T low, T high)
			 where T : IComparable<T>
		{
			return (input.CompareTo(low) < 0) ? low
				: (input.CompareTo(high) > 0) ? high
				: input;
		}
    }
}
