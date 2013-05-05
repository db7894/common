using System.Collections.Generic;


namespace SharedAssemblies.Core.Math
{
	/// <summary>
	/// A utility class that holds math utility functions.
	/// </summary>
	public static class MathUtility
	{
		/// <summary>
		/// <para>
		/// This method returns the fibonacci sequence which is an 
		/// infinite sequence of numbers where each result is the
		/// sum of the previous two results.
		/// </para>
		/// <para>
		/// The sequence would be 1, 1, 2, 3, 5, 8, 13, ...
		/// </para>
		/// </summary>
		/// <returns>The infinite fibonacci sequence.</returns>
		public static IEnumerable<int> GetFibonacciSequence()
		{
			int first = 1;
			int second = 1;

			// first and second result are always 1.
			yield return first;
			yield return second;

			// this enumerable sequence is bounded by the caller.
			while(true)
			{
				int current = first + second;
				yield return current;

				// wind up for next number if we're requesting one
				first = second;
				second = current;
			}
		}
	}
}
