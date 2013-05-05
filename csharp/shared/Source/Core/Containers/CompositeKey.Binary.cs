using System;
using System.Collections.Generic;


namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// Helper class to handle composite keys for hashing and equality.
	/// </summary>
	/// <typeparam name="TPrimary">The 1st component of the key.</typeparam>
	/// <typeparam name="TSecondary">The 2nd component of the key.</typeparam>
	public class CompositeKey<TPrimary, TSecondary> : IEquatable<CompositeKey<TPrimary, TSecondary>>, IComparable<CompositeKey<TPrimary, TSecondary>>, IComparable
		where TPrimary : IEquatable<TPrimary>, IComparable<TPrimary>
		where TSecondary : IEquatable<TSecondary>, IComparable<TSecondary>
	{
		// static comparer to compare all arrays
		private static readonly EqualityComparer<TPrimary> _primaryComparer = EqualityComparer<TPrimary>.Default;
		private static readonly EqualityComparer<TSecondary> _secondaryComparer = EqualityComparer<TSecondary>.Default;

		/// <summary>
		/// Gets the primary key
		/// </summary>
		public TPrimary Primary { get; private set; }

		/// <summary>
		/// Gets the secondary key.
		/// </summary>
		public TSecondary Secondary { get; private set; }

		/// <summary>
		/// Initializes an instance of CompositeKey with the given primary and secondary key values.
		/// </summary>
		/// <param name="primary">The primary key.</param>
		/// <param name="secondary">The secondary key.</param>
		public CompositeKey(TPrimary primary, TSecondary secondary)
		{
			if (primary == null)
			{
				throw new ArgumentNullException("primary");
			}

			if (secondary == null)
			{
				throw new ArgumentNullException("secondary");
			} 
			
			Primary = primary;
			Secondary = secondary;
		}

		/// <summary>
		/// Compares two CompositeKeys to see if they refer to the same key combination. 
		/// </summary>
		/// <param name="other">The second key to compare to this key.</param>
		/// <returns>True if both composite keys are the same.</returns>
		public bool Equals(CompositeKey<TPrimary, TSecondary> other)
		{
			// two method calls are the same if their method names are the same
			// and the arguments supplied are the same.
			return Equals(Primary, other.Primary) && Equals(Secondary, other.Secondary);
		}

		/// <summary>
		/// Compares two CompositeKeys to see if they refer to the same key combination. 
		/// </summary>
		/// <param name="other">The second key to compare to this key.</param>
		/// <returns>True if both composite keys are the same.</returns>
		public override bool Equals(object other)
		{
			// call IEquatable<CompositeKey<TPrimary,TSecondary>.Equals() instead
			return other is CompositeKey<TPrimary, TSecondary> 
				? Equals((CompositeKey<TPrimary, TSecondary>) other) 
				: false;
		}

		/// <summary>
		/// Gets the hash code for the method call by combining the hash code of the key components.
		/// </summary>
		/// <remarks>
		/// See Jon Skeet (C# MVP) response in the StackOverflow thread 
		/// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
		/// </remarks>
		/// <returns>The hash code for the CompositeKey.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 23;

				hash = 17 * hash + _primaryComparer.GetHashCode(Primary);
				hash = 17 * hash + _secondaryComparer.GetHashCode(Secondary);

				return hash;
			}
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// Zero if equal, less than 
		/// </returns>
		/// <param name="other">An object to compare with this object.
		///                 </param>
		public int CompareTo(CompositeKey<TPrimary, TSecondary> other)
		{
			int result = Primary.CompareTo(other.Primary);

			// if primary compare is not equal, check secondary, otherwise return primary
			if (result == 0)
			{
				result = Secondary.CompareTo(other.Secondary);
			}

			return result;
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// Zero if equal, less than 
		/// </returns>
		/// <param name="other">An object to compare with this object.
		///                 </param>
		public int CompareTo(object other)
		{
			// if other is null, then we are greater than it (since we are non-null), so return +1.
			if (other == null)
			{
				return 1;
			}

			return CompareTo((CompositeKey<TPrimary, TSecondary>) other);
		}

		/// <summary>
		/// Serializes the MethodCall by creating a string with the method name and arguments.
		/// </summary>
		/// <returns>String representation of the MethodCall.</returns>
		public override string ToString()
		{
			return string.Format("[CompositeKey - Primary: [{0}], Secondary: [{1}]]", Primary, Secondary);
		}
	}
}
