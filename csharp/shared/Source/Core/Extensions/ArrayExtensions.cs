using System.Linq;
using SharedAssemblies.Core.Containers;


namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// A set of extension methods for the array type.
	/// </summary>
	public static class ArrayExtensions
	{
		/// <summary>
		/// Takes any array reference and returns the reference if non-null or
		/// an empty array reference if null.
		/// </summary>
		/// <typeparam name="T">Type of array element.</typeparam>
		/// <param name="array">The array to examine.</param>
		/// <returns>The passed in value if non-null or an empty array if null.</returns>
		/// <remarks>Version 1.2</remarks>
		public static T[] NullSafe<T>(this T[] array)
		{
			return array ?? EmptyArray<T>.Instance;
		}


		/// <summary>
		/// Returns a null-safe length of an array.  If the array is null, returns zero.
		/// </summary>
		/// <remarks>Version 1.2</remarks>
		/// <param name="input">The array to check length upon.</param>
		/// <returns>The length of the array or zero if null.</returns>
		/// <typeparam name="T">The type of the array.</typeparam>
		public static int NullSafeLength<T>(this T[] input)
		{
			return input != null ? input.Length : 0;
		}


		/// <summary>
		/// Checks if the supplied enumerable is null or empty.
		/// </summary>
		/// <typeparam name="T">The underlying type of the IEnumerable.</typeparam>
		/// <param name="source">The enumerable source to iterate over.</param>
		/// <returns>True if the source is null or empty, false otherwise.</returns>
		/// <remarks>Version 1.2</remarks>
		public static bool IsNullOrEmpty<T>(this T[] source)
		{
			return (source == null) || (source.Length == 0);
		}

		/// <summary>
		/// Checks if the supplied enumerable is null or empty.
		/// </summary>
		/// <typeparam name="T">The underlying type of the IEnumerable.</typeparam>
		/// <param name="source">The enumerable source to iterate over.</param>
		/// <returns>True if the source is null or empty, false otherwise.</returns>
		/// <remarks>Version 1.2</remarks>
		public static bool IsNotNullOrEmpty<T>(this T[] source)
		{
			return (source != null) && (source.Length != 0);
		}
	}
}
