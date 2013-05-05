using System;
using System.Collections.Generic;


namespace SharedAssemblies.Core.Comparers
{
	/// <summary>
	/// The generic comparer factory that creates generic comparers through implicit typing
	/// </summary>
	public static class EqualityComparer
	{
		/// <summary>
		/// Creates a generic comparer based on the comparer method and hash method passed in.
		/// </summary>
		/// <typeparam name="TCompare">The type of items to compare.</typeparam>
		/// <param name="comparer">The delegate that checks for equality.</param>
		/// <param name="hasher">The delegate that generates the hash code.</param>
		/// <returns>A GenericEqualityComparer instance.</returns>
		public static IEqualityComparer<TCompare> Create<TCompare>(Func<TCompare, TCompare, bool> comparer,
			Func<TCompare, int> hasher)
		{
			return new GenericEqualityComparer<TCompare>(comparer, hasher);
		}

		/// <summary>
		/// Creates a generic comparer based on a key extraction delegate.
		/// </summary>
		/// <typeparam name="TCompare">The type of items to compare.</typeparam>
		/// <typeparam name="TKey">The type of key field to compare.</typeparam>
		/// <param name="keyExtractor">A delegate that extracts a key to be compared.</param>
		/// <returns>A GenericEqualityComparer instance.</returns>
		public static IEqualityComparer<TCompare> Create<TCompare, TKey>(Func<TCompare, TKey> keyExtractor)
		{
			return new KeyEqualityComparer<TCompare, TKey>(keyExtractor);
		}
	}
}
