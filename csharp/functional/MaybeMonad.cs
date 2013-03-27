using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	/// <summary>
	/// Think null or nullable, but with better support
	/// </summary>
	public class Maybe<T> : IMonad
	{
		public readonly static Maybe<T> Nothing = new Maybe<T>();
		public T Value { get; set; }
		public bool HasValue { get; private set; }

		public Maybe()
		{
			Value = default(T);
			HasValue = false;
		}

		public Maybe(T value)
		{
			Value = value;
			HasValue = true;
		}
	}

	#region Bind Example Utility

	/// <summary>
	/// Is this the best we can do?
	/// </summary>
	public static class MaybeBind
	{
		/// <summary>
		/// Bind
		/// </summary>
		public static Maybe<U> Bind<T, U>(this Maybe<T> id, Func<T, Maybe<U>> k)
		{
			return (id.HasValue) ? k(id.Value) : Maybe<U>.Nothing;
		}
	}

	#endregion

	#region Super Secret

	/// <summary>
	/// C# "do" notation
	/// </summary>
	public static class MaybeExtensions
	{
		/// <summary>
		/// Unit
		/// </summary>
		public static Maybe<T> ToMaybe<T>(this T value)
		{
			return new Maybe<T>(value);
		}

		/// <summary>
		/// Bind
		/// </summary>
		public static Maybe<U> SelectMany<T, U>(this Maybe<T> id, Func<T, Maybe<U>> k)
		{
			return (id.HasValue) ? k(id.Value) : Maybe<U>.Nothing;
		}

		/// <summary>
		/// Bind + Unit
		/// </summary>
		public static Maybe<V> SelectMany<T, U, V>(this Maybe<T> id, Func<T, Maybe<U>> k, Func<T, U, V> s)
		{
			return id.SelectMany(x =>
				 k(x).SelectMany(y => s(x, y).ToMaybe()));
		}
	}

	#endregion

	/// <summary>
	/// A simple example of using the maybe monad
	/// </summary>
	public static class MaybeExample
	{
		private static Maybe<Account> getAccount(string name)
		{
			if (name.Equals("john"))
			{
				var account = new Account { Name = name, Id = 1 };
				return account.ToMaybe();
			}
			return Maybe<Account>.Nothing;
		}

		private static Maybe<Balance> getBalance(Account account)
		{
			if (account.Id == 1)
			{
				var balance = new Balance { Id = account.Id, Value = 22.35m };
				return balance.ToMaybe();
			}
			return Maybe<Balance>.Nothing;
		}

		private static Maybe<Loan> getLoan(Balance balance)
		{
			if (balance.Value >= 20.0m)
			{
				var loan = new Loan { Id = balance.Id, Value = 100.0m };
				return loan.ToMaybe();
			}
			return Maybe<Loan>.Nothing;
		}

		#region Bind Example Code

		public static void mainBind()
		{
			Console.WriteLine("\n\nMaybe Monad Example\n");
			var name = "john";
			var loan = getAccount(name).Bind(getBalance).Bind(getLoan);

			Console.WriteLine("recieved {0} loan", loan.HasValue ? loan.Value.Value + " dollars " : "no");

		}

		#endregion

		#region Also Secret

		public static void main()
		{
			Console.WriteLine("\n\nMaybe Monad Example\n");
			var name = "john";
			var loan = from account in getAccount(name)
					   from balance in getBalance(account)
					   from money in getLoan(balance)
					   select money;

			Console.WriteLine("recieved {0} loan", loan.HasValue ? loan.Value.Value + " dollars " : "no");
		}

		#endregion
	}
}
