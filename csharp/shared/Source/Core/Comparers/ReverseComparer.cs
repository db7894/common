using System;
using System.Collections.Generic;


namespace SharedAssemblies.Core.Comparers
{
	/// <summary>
	/// A generic comparer that reverses the comparisons of IComparable instances
	/// </summary>
	/// <typeparam name="T">Type of item to compare, need not implement IComparable.</typeparam>
	public sealed class ReverseComparer<T> : IComparer<T> where T : IComparable<T>
	{
		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <returns>-1 if x &lt; y, 0 if x == y, and +1 if x &gt; y</returns>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		public int Compare(T x, T y)
		{
			if (x == null)
			{
				if (y == null)
				{
					// if both null (if ref types) they are "equal"
					return 0;
				}

				// otherwise, if x is null but right is non-null, then x < y (by MSDN description)
				// but since we're reversing, that should be a positive result.
				return 1;
			}

			// okay, so X isn't null if we get here, just check for y is null, if 
			// x is null but y is not null, then x > y according to MSDN description,
			// so to reverse this we return a negative result.
			if (y == null)
			{
				return -1;
			}

			// if neither arg is null, pass on to CompareTo in reverse order.  
			// That is, y.CompareTo(x) will return a negative result of x.CompareTo(y)
			return y.CompareTo(x);
		}
	}
}
