using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	public class Identity<T> : IMonad
	{
		public T Value { get; private set; }
		public Identity(T value) { Value = value; }
	}

	public static class IdentityExtensions
	{
		/// <summary>
		/// Unit
		/// </summary>
		public static Identity<T> ToIdentity<T>(this T value)
		{
			return new Identity<T>(value);
		}

		/// <summary>
		/// Bind
		/// </summary>
		public static Identity<U> SelectMany<T, U>(this Identity<T> id, Func<T, Identity<U>> k)
		{
			return k(id.Value);
		}

		/// <summary>
		/// Bind + Unit
		/// </summary>
		public static Identity<V> SelectMany<T, U, V>(this Identity<T> id, Func<T, Identity<U>> k, Func<T, U, V> s)
		{
			return id.SelectMany(x =>
				 k(x).SelectMany(y => s(x, y).ToIdentity()));
		}
	}

	/// <summary>
	/// A simple example of the do notation
	/// </summary>
	public static class IdentityExample
	{
		private static Identity<Account> getAccount(string name)
		{
			var account = new Account { Name = name, Id = 1 };
			return account.ToIdentity();
		}

		private static Identity<Balance> getBalance(Account account)
		{
			var balance = new Balance { Id = account.Id, Value = 22.35m };
			return balance.ToIdentity();
		}

		private static Identity<Loan> getLoan(Balance balance)
		{
			var loan = new Loan { Id = balance.Id, Value = 100.0m };
			return loan.ToIdentity();
		}

		public static void main()
		{
			Console.WriteLine("\n\nIdentity Monad Example\n");
			var name = "john";
			var loan = from account in getAccount(name)
					   from balance in getBalance(account)
					   from money in getLoan(balance)
					   select money;

			Console.WriteLine("recieved {0} loan", loan.Value.Value + " dollar ");
		}
	}
}
