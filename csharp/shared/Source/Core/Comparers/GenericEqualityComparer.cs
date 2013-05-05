using System;
using System.Collections.Generic;

namespace SharedAssemblies.Core.Comparers
{
	/// <summary>
	/// A helper class that can be used to implement a generic IEqualityComparer
	/// using lambdas (say for LINQ).
	/// </summary>
	/// <typeparam name="TCompare">The type to compare</typeparam>
	public sealed class GenericEqualityComparer<TCompare> : EqualityComparer<TCompare>
	{
		private readonly Func<TCompare, TCompare, bool> _comparer;
		private readonly Func<TCompare, int> _hasher;

		/// <summary>
		/// Initialize a new instance of the GenericEqualityComparer class
		/// </summary>
		/// <param name="comparer">The comparison function to use</param>
		/// <param name="hasher">The hash function to use</param>
		public GenericEqualityComparer(Func<TCompare, TCompare, bool> comparer, Func<TCompare, int> hasher)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			if (hasher == null)
			{
				throw new ArgumentNullException("hasher");
			}

			_comparer = comparer;
			_hasher = hasher;
		}

		/// <summary>
		/// Template equals method that calls the comparison function
		/// </summary>
		/// <param name="left">The left value to compare</param>
		/// <param name="right">The right value to compare</param>		
		/// <returns>true if the two are equal, false otherwise</returns>
		public override bool Equals(TCompare left, TCompare right)
		{
			// if they are same reference, why bother checking?
			if (ReferenceEquals(left, right))
			{
				return true;
			}

			// if either is null, no sense checking either (both are null is handled by ReferenceEquals())
			if (left == null || right == null)
			{
				return false;
			}

			return _comparer(left, right);
		}

		/// <summary>
		/// Template GetHashCode method that calls the comparison function
		/// </summary>
		/// <param name="obj">The value to retrieve a hash code for</param>
		/// <returns>The hash code of the given object</returns>
		public override int GetHashCode(TCompare obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			return _hasher(obj);
		}
	}
}
