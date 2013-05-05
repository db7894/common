using System;
using System.Collections.Generic;

namespace SharedAssemblies.Core.Comparers
{
	/// <summary>
	/// A helper class that can be used to implement a generic IEqualityComparer
	/// by comparing them based on a given key field.
	/// </summary>
	/// <typeparam name="TCompare">The type of the items to compare.</typeparam>
	/// <typeparam name="TKey">The type of the key field.</typeparam>
	public sealed class KeyEqualityComparer<TCompare, TKey> : EqualityComparer<TCompare>
	{
		private readonly Func<TCompare, TKey> _keyExtractor;

		/// <summary>
		/// Initialize a new instance of the GenericEqualityComparer class
		/// </summary>
		/// <param name="keyExtractor">The delegate for extracting the key field.</param>
		public KeyEqualityComparer(Func<TCompare, TKey> keyExtractor)
		{
			if (keyExtractor == null)
			{
				throw new ArgumentNullException("keyExtractor");
			}

			_keyExtractor = keyExtractor;
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

			return Equals(_keyExtractor(left), _keyExtractor(right));
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

			var key = _keyExtractor(obj);

			// I decided since obj is non-null, i'd return zero if key was null.
			return key != null ? key.GetHashCode() : 0;
		}
	}
}
