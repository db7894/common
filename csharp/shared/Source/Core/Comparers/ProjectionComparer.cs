using System;
using System.Collections.Generic;


namespace SharedAssemblies.Core.Comparers
{
	/// <summary>
	/// Class that implements a ProjectionComparer for any type that projects the type to another type (usually a key field).
	/// </summary>
	/// <typeparam name="TCompare">The type of items to compare.</typeparam>
	/// <typeparam name="TProjected">The resulted projection type that can be compared.</typeparam>
	public sealed class ProjectionComparer<TCompare, TProjected> : EqualityComparer<TCompare>, IComparer<TCompare>
	{
		private readonly Func<TCompare, TProjected> _projection;

		/// <summary>
		/// Initialize a new instance of the GenericEqualityComparer class
		/// </summary>
		/// <param name="projection">The delegate for extracting the key field.</param>
		public ProjectionComparer(Func<TCompare, TProjected> projection)
		{
			if (projection == null)
			{
				throw new ArgumentNullException("projection");
			}

			_projection = projection;
		}

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <returns>Integer result comparing the relative values of left and right.</returns>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		public int Compare(TCompare left, TCompare right)
		{
			// if both same object or both null, return zero automatically
			if (ReferenceEquals(left, right))
			{
				return 0;
			}

			// can only happen if left null and right not null
			if (left == null)
			{
				return -1;
			}

			// can only happen if right null and left non-null
			if (right == null)
			{
				return 1;
			}

			// otherwise compare the projections
			return Comparer<TProjected>.Default.Compare(_projection(left), _projection(right));
		}

		/// <summary>
		/// Template equals method that calls the comparison function
		/// </summary>
		/// <param name="left">The left value to compare</param>
		/// <param name="right">The right value to compare</param>		
		/// <returns>true if the two are equal, false otherwise</returns>
		public override bool Equals(TCompare left, TCompare right)
		{
			// why bother to extract if they refer to same object...
			if (ReferenceEquals(left, right))
			{
				return true;
			}

			// if either is null, no sense checking either (both are null is handled by ReferenceEquals())
			if (left == null || right == null)
			{
				return false;
			}

			return Equals(_projection(left), _projection(right));
		}

		/// <summary>
		/// Template GetHashCode method that calls the comparison function
		/// </summary>
		/// <param name="obj">The value to retrieve a hash code for</param>
		/// <returns>The hash code of the given object</returns>
		public override int GetHashCode(TCompare obj)
		{
			// unlike Equals, GetHashCode() should never be called on a null object
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			var key = _projection(obj);

			// I decided since obj is non-null, i'd return zero if key was null.
			return key == null ? 0 : key.GetHashCode();
		}
	}
}
