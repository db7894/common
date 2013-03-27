using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	public class Monoid<TInput, TConcat>
	{
		public Func<TConcat, TInput, TConcat> Mappend { get; private set; }
		public Func<TConcat> Mempty { get; private set; }

		public Monoid(Func<TConcat, TInput, TConcat> mappend, Func<TConcat> mempty)
		{
			Mappend = mappend;
			Mempty = mempty;
		}

		public TConcat Concat(IEnumerable<TInput> items)
		{
			return items.Aggregate<TInput, TConcat>(Mempty(), Mappend);
		}
	}

	/// <summary>
	/// An implementation of the sum monoid for ints
	/// </summary>
	public class SumMonoid : Monoid<int, int>
	{
		public SumMonoid() : base((a, b) => a + b, () => 0)
		{}
	}

	/// <summary>
	/// An implementation of the count monoid for ints
	/// </summary>
	public class CountMonoid : Monoid<int, int>
	{
		public CountMonoid()
			: base((a, b) => a + 1, () => 0)
		{ }
	}

	/// <summary>
	/// An implementation of the product monoid for ints
	/// </summary>
	public class ProductMonoid : Monoid<int, int>
	{
		public ProductMonoid() : base((a, b) => a * b, () => 1)
		{ }
	}

	/// <summary>
	/// An implementation of the all monoid
	/// </summary>
	public class AllMonoid<T> : Monoid<T, bool>
	{
		public AllMonoid(Func<T, bool> predicate)
			: base((a, b) => a && predicate(b), () => true)
		{ }
	}

	/// <summary>
	/// An implementation of the any monoid
	/// </summary>
	public class AnyMonoid<T> : Monoid<T, bool>
	{
		public AnyMonoid(Func<T, bool> predicate)
			: base((a, b) => a || predicate(b), () => false)
		{ }
	}

	/// <summary>
	/// An implementation of the Set monoid
	/// </summary>
	public class SetMonoid<T> : Monoid<T, HashSet<T>>
	{
		public SetMonoid()
			: base((a, b) => { a.Add(b); return a; }, () => new HashSet<T>())
		{ }
	}
	
	/// <summary>
	/// An implementation of the list monoid
	/// </summary>
	public class ListMonoid<T> : Monoid<T, List<T>>
	{
		public ListMonoid()
			: base((a, b) => { a.Add(b); return a; }, () => new List<T>())
		{ }
	}

	/// <summary>
	/// An implementation of the list monoid
	/// </summary>
	public class DictionaryMonoid<T, K, V> : Monoid<T, IDictionary<K, V>>
	{
		public DictionaryMonoid(Func<T, K> key, Func<T, V> value)
			: base((a, b) => { a[key(b)] = value(b); return a; }, () => new Dictionary<K, V>())
		{ }
	}

	/// <summary>
	/// An implementation of the list monoid
	/// </summary>
	public class DictionaryMonoid<T, K> : DictionaryMonoid<T, K, T>
	{
		public DictionaryMonoid(Func<T, K> key)
			: base(key, (x) => x)
		{ }
	}

	/// <summary>
	/// An example monoid test runner
	/// </summary>
	public static class MonoidExample
	{
		public static void main()
		{
			var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			Func<int, bool> isEven = (x) => x % 2 == 0;
			Console.WriteLine("\n\nMonoid Examples\n");
			Console.WriteLine("sum monoid {0}", new SumMonoid().Concat(list));
			Console.WriteLine("count monoid {0}", new CountMonoid().Concat(list));
			Console.WriteLine("product monoid {0}", new ProductMonoid().Concat(list));
			Console.WriteLine("all(isEven) monoid {0}", new AllMonoid<int>(isEven).Concat(list));
			Console.WriteLine("any(isEven) monoid {0}", new AnyMonoid<int>(isEven).Concat(list));
			Console.WriteLine("set monoid {0}", new SetMonoid<int>().Concat(list).Count);
			Console.WriteLine("list monoid {0}", new ListMonoid<int>().Concat(list).Count());
			Console.WriteLine("dictionary monoid {0}", new DictionaryMonoid<int, int, string>(
				(x) => x, (x) => x.ToString()).Concat(list).Count());
		}
	}
}
