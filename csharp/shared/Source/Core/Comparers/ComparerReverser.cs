using System;
using System.Collections.Generic;


namespace SharedAssemblies.Core.Comparers
{
	/// <summary>
	/// A generic comparer that reverses the action of a wrapped comparer.
	/// </summary>
	/// <typeparam name="T">Type of item to compare, need not implement IComparable.</typeparam>
	public sealed class ComparerReverser<T> : IComparer<T>
	{
		private readonly IComparer<T> _wrappedComparer;

		/// <summary>
		/// Initializes an instance of a ComparerReverser that takes a wrapped comparer
		/// and returns the inverse of the comparison.
		/// </summary>
		/// <param name="wrappedComparer">The comparer to wrap and reverse.</param>
		public ComparerReverser(IComparer<T> wrappedComparer)
		{
			if (wrappedComparer == null)
			{
				throw new ArgumentNullException("wrappedComparer");
			}

			_wrappedComparer = wrappedComparer;
		}

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <returns>-1 if x &lt; y, 0 if x == y, and +1 if x &gt; y</returns>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		public int Compare(T x, T y)
		{
			// to reverse compare, just invert the operands....
			return _wrappedComparer.Compare(y, x);
		}
	}
}
