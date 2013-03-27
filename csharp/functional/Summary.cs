using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FunctionalShare
{
	/// <summary>
	/// A quick summary of functional programming via code
	/// </summary>
	public static class Summary
	{
		/// <summary>
		/// A quicksort example using recursion
		/// </summary>
		/// <param name="values">The values to sort</param>
		/// <returns>The sorted collection</returns>
		static IEnumerable<int> QuickSort(IEnumerable<int> values)
		{
			if (!values.Any())
				return Enumerable.Empty<int>();
			var head = values.First();
			var smaller = QuickSort(values.Skip(1).Where(x => x <= head));
			var bigger  = QuickSort(values.Skip(1).Where(x => x >  head));
			return smaller.Concat(new[] { head }).Concat(bigger);
		}

		/// <summary>
		/// An example of using recursion to solve a problem
		/// </summary>
		static void Recursion()
		{
			Console.WriteLine("\n\nRecursion Example\n");
			var initial = new[] { 5, 2, 7, 3, 99, 56, 23, 6 };
			var sorted = QuickSort(initial);
			foreach (var value in sorted)
				Console.Write("{0} ", value);
			Console.WriteLine("\nAre the sorted values");
		}
		
		/// <summary>
		/// A helper method that produces an infinite stream
		/// of digits.
		/// </summary>
		/// <param name="start">The value to start from</param>
		/// <returns>An infinite stream of digits</returns>
		static IEnumerable<int> naturals(int start)
		{
			int current = start;
			while (true)
			{
				yield return current;
				current += 1;
			}
		}

		/// <summary>
		/// An example of using higher ordered functions
		/// </summary>
		static void HigherOrderFunctions()
		{
			Console.WriteLine("\n\nHigher Order Functions Example\n");
			Action<Action<string>, string> logger = (stream, message) => stream(message);
			Action<string> normalLogger = (message) => logger(Console.Out.WriteLine, message);
			Action<string> errorLogger = (message) => logger(Console.Error.WriteLine, message);

			errorLogger("This is an error message");
			normalLogger("This is a normal message");

			// same problem solved 3 ways map/filter/reduce
			var evensMap = naturals(1).Select(x => x * 2).Take(10);
			var evensFilter = naturals(2).Where(x => x % 2 == 0).Take(10);
			var evensReduce = naturals(1).Take(10).Aggregate<int, List<int>>(
				new List<int>(),
				(total, next) => { total.Add(next * 2); return total; });
		}

		/// <summary>
		/// A helper method that produces an infinite stream of
		/// primes.
		/// </summary>
		/// <returns>An infinite stream of primes</returns>
		static IEnumerable<int> primes()
		{
			var stream = naturals(2);
			while (true)
			{
				int head = stream.First();
				yield return head;
				stream = stream.Skip(1).Where(x => x % head != 0);
			}
		}

		/// <summary>
		/// An example of lazy evaluation on an infinite stream
		/// </summary>
		static void LazyEvaluation()
		{
			Console.WriteLine("\n\nLazy Evaluation Example\n");
			var evens = naturals(1).Select(i => i * 2);
			var firstTenEvens = evens.Take(10);
			foreach (var even in firstTenEvens)
				Console.Write("{0} ", even);
			Console.WriteLine("\nAre the first ten evens");

			var firstTenPrimes = primes().Take(10);
			foreach (var prime in firstTenPrimes)
				Console.Write("{0} ", prime);
			Console.WriteLine("\nAre the first ten primes");
		}

		/// <summary>
		/// An example of partial application
		/// </summary>
		static void PartialApplication()
		{
			Console.WriteLine("\n\nPartial Application Example\n");
			int currentLevel = 0;
			Action<int, string> logger = (level, message) => {
				if (level > currentLevel)
					Console.WriteLine("{0} {1}: {2}", level, DateTime.Now, message);
			};
			Action<string> errorLogger = (message) => logger(4, message);
			Action stackLogger = () => logger(1, ("\n" + new StackTrace()).ToString());

			logger(0, "a message that isn't printed");
			logger(1, "a message that is printed");
			errorLogger("an error message oh no!");
			stackLogger();
		}

		/// <summary>
		/// The main runner for these examples
		/// </summary>
		public static void main()
		{
			Recursion();
			HigherOrderFunctions();
			LazyEvaluation();
			PartialApplication();
		}
	}
}
