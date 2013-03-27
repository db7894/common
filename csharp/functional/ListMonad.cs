using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	/// <summary>
	/// C# "do" notation
	/// </summary>
	public static class ListExtensions
	{
		/// <summary>
		/// Unit
		/// </summary>
		public static IEnumerable<T> ToListM<T>(this T value)
		{
			yield return value;
		}

		/// <summary>
		/// Bind
		/// </summary>
		public static IEnumerable<U> SelectMany<T, U>(this IEnumerable<T> id, Func<T, IEnumerable<U>> k)
		{
			foreach (var x in id)
				foreach (var y in k(x))
					yield return y;
		}

		/// <summary>
		/// Bind + Unit
		/// </summary>
		public static IEnumerable<V> SelectMany<T, U, V>(this IEnumerable<T> id, Func<T, IEnumerable<U>> k, Func<T, U, V> s)
		{
			return id.SelectMany(x =>
				 k(x).SelectMany(y => s(x, y).ToListM()));
		}
	}

	/// <summary>
	/// A simple example of using the either monad
	/// </summary>
	public static class ListExample
	{
		private static IEnumerable<Account> getAccount(string name)
		{
			if (name.Equals("john"))
			{
				var account = new Account { Name = name, Id = 1 };
				return account.ToListM();
			}
			return Enumerable.Empty<Account>();
		}

		private static IEnumerable<Balance> getBalance(Account account)
		{
			if (account.Id == 1)
			{
				return new List<Balance> {
					new Balance { Id = account.Id, Value = 22.35m },
					new Balance { Id = account.Id, Value = 50.95m },
				};
			}
			return Enumerable.Empty<Balance>();
		}

		private static IEnumerable<Loan> getLoan(Balance balance)
		{
			if (balance.Value >= 20.0m)
			{
				var loan = new Loan { Id = balance.Id, Value = balance.Value * 10 };
				return loan.ToListM();
			}
			return Enumerable.Empty<Loan>();
		}

		public static void main()
		{
			Console.WriteLine("\n\nList Monad Example\n");
			var name = "john";
			var loans = from account in getAccount(name)
					   from balance in getBalance(account)
					   from money in getLoan(balance)
					   select money;

			foreach (var loan in loans)
			{
				Console.WriteLine("user got loan for {0} dollars", loan.Value);
			}
		}
	}
}
