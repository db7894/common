using System;

namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// Class that creates arrays based on given criteria
	/// </summary>
	public static class ArrayFactory
	{
		/// <summary>
		/// Factory method that creates a new array of the given size.
		/// </summary>
		/// <typeparam name="T">The type of array of item to create</typeparam>
		/// <param name="size">The size of the array.</param>
		/// <returns>The new array.</returns>
		public static T[] Create<T>(int size)
		{
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "The array size cannot be negative");
			}

			return new T[size];
		}

		/// <summary>
		/// Factory method that creates a new array of the given size, each element initialized to the
		/// given initial value.
		/// </summary>
		/// <typeparam name="T">The type of array of item to create.</typeparam>
		/// <param name="size">The size of the array.</param>
		/// <param name="initialValue">The value to assign to all entries.</param>
		/// <returns>The new array.</returns>
		public static T[] Create<T>(int size, T initialValue)
		{
			// going for speed instead of conciseness here (avoiding Linq)
			var result = Create<T>(size);

			for (int i = 0; i < size; ++i)
			{
				result[i] = initialValue;
			}

			return result;
		}

		/// <summary>
		/// Factory method that creates a new array of the given size, each element initialized to the
		/// value produced by the no-arg generator.
		/// </summary>
		/// <typeparam name="T">The type of array of item to create.</typeparam>
		/// <param name="size">The size of the array.</param>
		/// <param name="generator">A generator to provide a value to each entry.</param>
		/// <returns>The new array.</returns>
		public static T[] Create<T>(int size, Func<T> generator)
		{
			// going for speed instead of conciseness here (avoiding Linq)
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}

			var result = Create<T>(size);

			for (int i = 0; i < size; ++i)
			{
				result[i] = generator();
			}

			return result;
		}

		/// <summary>
		/// Factory method that creates a new array of the given size, each element initialized to the
		/// value produced by the generator which takes the current array index as an argument.
		/// </summary>
		/// <typeparam name="T">The type of array of item to create.</typeparam>
		/// <param name="size">The size of the array.</param>
		/// <param name="generator">A generator to provide a value to each entry.</param>
		/// <returns>The new array.</returns>
		public static T[] Create<T>(int size, Func<int, T> generator)
		{
			// going for speed instead of conciseness here (avoiding Linq)
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}

			var result = Create<T>(size);

			for (int i = 0; i < size; ++i)
			{
				result[i] = generator(i);
			}

			return result;
		}
	}
}
