using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedAssemblies.Core.Containers;


namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// This class contains helper additions to linq.
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Adds the ForEach method to an IEnumerable.
		/// </summary>
		/// <code>
		/// IEnumerable&lt;T&gt; handle = GetEnumerable();
		/// handle.ForEach(a =&gt; Console.Write(a));
		/// </code>
		/// <param name="source">The enumerable source to iterate over.</param>
		/// <param name="action">Action to perform on each element.</param>
		/// <typeparam name="T">The type of element the enumeration holds.</typeparam>
		/// <returns>The source collection after all actions have been performed.</returns>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source,
			Action<T> action)
		{
			if (source != null)
			{
				foreach (T element in source)
				{
					action(element);
				}
			}

			return source;
		}

		/// <summary>
		/// Returns true if the enumerable collection is empty.
		/// </summary>
		/// <typeparam name="T">The type of the collection</typeparam>
		/// <param name="source">The source collection</param>
		/// <returns>True if the collection is empty.</returns>
		public static bool IsEmpty<T>(this IEnumerable<T> source)
		{
			return !source.Any();
		}

		/// <summary>
		/// Returns true if the enumerable collection is not empty.
		/// </summary>
		/// <typeparam name="T">The type of the collection</typeparam>
		/// <param name="source">The source collection</param>
		/// <returns>True if the collection is not empty.</returns>
		public static bool IsNotEmpty<T>(this IEnumerable<T> source)
		{
			return source.Any();
		}

		/// <summary>
		/// Checks if the supplied enumerable is null or empty.
		/// </summary>
		/// <typeparam name="T">The underlying type of the IEnumerable.</typeparam>
		/// <param name="source">The enumerable source to iterate over.</param>
		/// <remarks>Revised to use Any() which is constant-time on non-collections.</remarks>
		/// <returns>True if the source is null or empty, false otherwise.</returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
		{
			return (source == null) || (!source.Any());
		}

		/// <summary>
		/// Checks if the supplied enumerable is null or empty.
		/// </summary>
		/// <typeparam name="T">The underlying type of the IEnumerable.</typeparam>
		/// <param name="source">The enumerable source to iterate over.</param>
		/// <remarks>Revised to use Any() which is constant-time on non-collections.</remarks>
		/// <returns>True if the source is null or empty, false otherwise.</returns>
		public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
		{
			return (source != null) && (source.Any());
		}

		/// <summary>
		/// Checks if the value is not in the collection.
		/// </summary>
		/// <typeparam name="T">The type to convert to.</typeparam>
		/// <param name="collection">Collection to check non-existence in.</param>
		/// <param name="value">Value of object.</param>
		/// <returns>True if not in the collection, otherwise false.</returns>
		public static bool DoesNotContain<T>(this IEnumerable<T> collection, T value)
		{
			return !collection.Contains(value);
		}
		
		/// <summary>
		/// Performs an action on each item in the enumeration.
		/// </summary>
		/// <typeparam name="T">Type of item in the enumeration.</typeparam>
		/// <param name="source">Source list of items.</param>
		/// <param name="action">Action to perform on each item.</param>
		/// <returns>The item after action performed.</returns>
		public static IEnumerable<T> Pipeline<T>(this IEnumerable<T> source,
			Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
				yield return item;
			}
		}

		/// <summary>
		/// Performs an action on each item in the list that satisfies a predicate condition.
		/// </summary>
		/// <typeparam name="T">Type of item in the source enumeration.</typeparam>
		/// <param name="source">Source enumeration of items to perform the action on.</param>
		/// <param name="action">Action to perform on each item.</param>
		/// <param name="filter">The filter predicate to determine if the action should be performed.</param>
		/// <returns>The item regardless of whether or not the action was performed.</returns>
		public static IEnumerable<T> Pipeline<T>(this IEnumerable<T> source,
			Action<T> action, Predicate<T> filter)
		{
			foreach (var item in source)
			{
				if (filter(item))
				{
					action(item);
				}

				// return regardless of whether the item had the action performed
				// so the pipeline can be continued.
				yield return item;
			}
		}

		/// <summary>
		/// Ends a chain of extensions to pull items through the chain.
		/// </summary>
		/// <typeparam name="T">Type of item in the source enumeration.</typeparam>
		/// <param name="source">The source enumeration.</param>
		public static void Pull<T>(this IEnumerable<T> source)
		{
			foreach (var item in source)
			{
				// simply iterates over each item, so this caps a chain of extensions to drive them
			}
		}

		/// <summary>
		/// Returns a sub-collection iterator consisting of every nth item.
		/// </summary>
		/// <typeparam name="T">Type of collection.</typeparam>
		/// <param name="source">Source collection.</param>
		/// <param name="byEvery">Specifies how big gap is between items.</param>
		/// <returns>Subset of collection containing every nth item.</returns>
		public static IEnumerable<T> Every<T>(this IEnumerable<T> source, int byEvery)
		{
			int count = 0;

			foreach (var item in source)
			{
				if ((count++ % byEvery) == 0)
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Returns the given collection in a random manner
		/// </summary>
		/// <typeparam name="T">Type of collection.</typeparam>
		/// <param name="source">Source collection.</param>
		/// <returns>A random representation of the given collection</returns>
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return source.OrderBy(element => Guid.NewGuid());
		}

		/// <summary>
		/// Continues processing items in a collection until the end condition is true.
		/// </summary>
		/// <typeparam name="T">The type of the collection.</typeparam>
		/// <param name="collection">The collection to iterate.</param>
		/// <param name="endCondition">The condition that returns true if iteration should stop.</param>
		/// <returns>Iterator of sub-list.</returns>
		public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> collection, Predicate<T> endCondition)
		{
			return collection.TakeWhile(item => !endCondition(item));
		}

		/// <summary>
		/// Returns the count of the collection or null if the collection instance is null.
		/// </summary>
		/// <typeparam name="T">Type of item stored in the collection.</typeparam>
		/// <param name="collection">The collection to check.</param>
		/// <returns>The count of the collection, or zero if null.</returns>
		/// <remarks>Version 1.2</remarks>
		public static int NullSafeCount<T>(this IEnumerable<T> collection)
		{
			return collection != null ? collection.Count() : 0;
		}


		/// <summary>
		/// Returns the calling collection if it is not null, or returns an empty collection
		/// if null.
		/// </summary>
		/// <typeparam name="T">The type of the collection.</typeparam>
		/// <param name="collection">The collection to check.</param>
		/// <returns>The collection if not null or empty collection if null.</returns>
		/// <remarks>Version 1.2</remarks>
		public static IEnumerable<T> NullSafe<T>(this IEnumerable<T> collection)
		{
			return collection ?? Enumerable.Empty<T>();
		}


		/// <summary>
		/// Summarizes a collection into a string.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="collection">The collection of items.</param>
		/// <param name="stringifier">The lambda to convert T to string.</param>
		/// <param name="depth">The depth to dive into the collection.</param>
		/// <returns>String version.</returns>
		public static string Summarize<T>(this IEnumerable<T> collection, Func<T, string> stringifier,
			int depth)
		{
			int count = collection.NullSafeCount();
			int diveTo = System.Math.Min(depth, count);

			var builder = new StringBuilder("(" + count);

			if (collection != null)
			{
				int current = 0;
				foreach (var item in collection)
				{
					if (current++ < depth)
					{
						builder.Append(" {");
						builder.Append(stringifier(item));
						builder.Append("}");
					}
					else
					{
						break;
					}
				}
			}

			return builder.Append(')').ToString();
		}


		/// <summary>
		/// Summarizes a collection into a string.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="collection">The collection of items.</param>
		/// <param name="depth">The depth to dive into the collection.</param>
		/// <returns>String version.</returns>
		public static string Summarize<T>(this IEnumerable<T> collection, int depth)
		{
			return Summarize(collection, item => item.ToString(), depth);
		}

		/// <summary>
		/// Converts an enumeration of groupings into a Dictionary of those groupings.
		/// </summary>
		/// <typeparam name="TKey">Key type of the grouping and dictionary.</typeparam>
		/// <typeparam name="TValue">Element type of the grouping and dictionary list.</typeparam>
		/// <param name="groupings">The enumeration of groupings from a GroupBy() clause.</param>
		/// <returns>A dictionary of groupings such that the key of the dictionary is TKey type and the value is List of TValue type.</returns>
		public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings)
		{
			return groupings.ToDictionary(group => group.Key, group => group.ToList());
		}

		/// <summary>
		/// Creates a HashSet of TSource from an IEnumerable of TSource using the identity selector and default equality comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of elements of source.</typeparam>
		/// <param name="source">An IEnumerable of TSource to create a HashSet of TSource from.</param>
		/// <returns>A HashSet of TSource that contains the unique elements.</returns>
		public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
		{
			return source.ToHashSet<TSource, TSource>(item => item, null);
		}

		/// <summary>
		/// Creates a HashSet of TSource from an IEnumerable of TSource using the identity selector and the specified
		/// equality comparer for TSource.
		/// </summary>
		/// <typeparam name="TSource">The type of elements of source.</typeparam>
		/// <param name="source">An IEnumerable of TSource to create a HashSet of TSource from.</param>
		/// <param name="comparer">A comparer that compares two elements of type TSource.</param>
		/// <returns>A HashSet of TSource that contains the unique elements.</returns>
		public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			return source.ToHashSet<TSource, TSource>(item => item, comparer);
		}

		/// <summary>
		/// Creates a HashSet of TElement from an IEnumerable of TSource using the specified element selector and default equality comparer.
		/// </summary>
		/// <typeparam name="TSource">The type of elements of source.</typeparam>
		/// <typeparam name="TElement">The type of elements returned by the element selector.</typeparam>
		/// <param name="source">An IEnumerable of TSource to create a HashSet of TElement from.</param>
		/// <param name="elementSelector">A function to extract an element value from each source element.</param>
		/// <returns>A HashSet of TElement that contains the unique elements.</returns>
		public static HashSet<TElement> ToHashSet<TSource, TElement>(this IEnumerable<TSource> source, 
			Func<TSource, TElement> elementSelector)
		{
			return source.ToHashSet<TSource, TElement>(elementSelector, null);
		}

		/// <summary>
		/// Creates a HashSet of TElement from an IEnumerable of TSource using the specified element selector and the specified
		/// equality comparer for TElement.
		/// </summary>
		/// <typeparam name="TSource">The type of elements of source.</typeparam>
		/// <typeparam name="TElement">The type of elements returned by the element selector.</typeparam>
		/// <param name="source">An IEnumerable of TSource to create a HashSet of TElement from.</param>
		/// <param name="elementSelector">A function to extract an element value from each source element.</param>
		/// <param name="comparer">A comparer that compares two elements of type TElement.</param>
		/// <returns>A HashSet of TElement that contains the unique elements.</returns>
		public static HashSet<TElement> ToHashSet<TSource, TElement>(this IEnumerable<TSource> source, 
			Func<TSource, TElement> elementSelector, IEqualityComparer<TElement> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (elementSelector == null)
			{
				throw new ArgumentNullException("elementSelector");
			}

			var result = new HashSet<TElement>(comparer);

			// unrolling this LINQ as just a for loop to keep it fast.
			foreach (var item in source)
			{
				result.Add(elementSelector(item));
			}

			return result;
		}

		/// <summary>
		/// Returns the range of values in the enumeration.  This returns a Tuple with Item1 being the minimum value and 
		/// Item2 being the maximum value in the enumeration.
		/// </summary>
		/// <typeparam name="TSource">The type of the enumeration.</typeparam>
		/// <param name="source">The enumeration.</param>
		/// <returns>A Tuple containing the min and max value of the sequence.</returns>
		public static Tuple<TSource, TSource> GetRange<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			// define min and max values
			TSource min;
			TSource max;

			// get the default comparer for the source type, and default min and max to the first (or default) value.
			var comparer = Comparer<TSource>.Default;
			min = max = source.FirstOrDefault();

			// go through all other values and see if any are smaller or larger
			// could have done Skip(1) but turned out to be SLOWER than just comparing first even though no-op.
			foreach (var item in source)
			{
				if (comparer.Compare(item, min) < 0)
				{
					min = item;
				}

				if (comparer.Compare(item, max) > 0)
				{
					max = item;
				}
			}

			return Tuple.Create(min, max);
		}
	}
}
