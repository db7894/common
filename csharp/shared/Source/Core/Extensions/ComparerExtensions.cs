using System;
using System.Collections.Generic;
using SharedAssemblies.Core.Comparers;


namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Extension method that will take any comparer and reverse it.
	/// </summary>
	public static class ComparerExtensions
	{
		/// <summary>
		/// Reverses the comparer by wrapping it in a ComparerReverser instance.
		/// </summary>
		/// <typeparam name="T">Type of item being compared.</typeparam>
		/// <param name="comparer">The comparer to wrap and reverse.</param>
		/// <returns>A reverse comparer for the comparer given.</returns>
		public static IComparer<T> Reverse<T>(this IComparer<T> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}

			return new ComparerReverser<T>(comparer);
		}
	}
}
