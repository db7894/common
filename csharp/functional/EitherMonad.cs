using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	public class Either<T> : IMonad
	{
		public T Value { get; set; }
		public Exception Error { get; set; }
		public bool HasValue { get; private set; }

		public Either(Exception error)
		{
			Value = default(T);
			Error = error;
			HasValue = false;
		}

		public Either(T value)
		{
			Value = value;
			HasValue = true;
		}
	}

	/// <summary>
	/// C# "do" notation
	/// </summary>
	public static class EitherExtensions
	{
		/// <summary>
		/// Unit
		/// </summary>
		public static Either<T> ToEither<T>(this T value)
		{
			return new Either<T>(value);
		}

		/// <summary>
		/// Bind
		/// </summary>
		public static Either<U> SelectMany<T, U>(this Either<T> id, Func<T, Either<U>> k)
		{
			return (id.HasValue) ? k(id.Value) : new Either<U>(id.Error);
		}

		/// <summary>
		/// Bind + Unit
		/// </summary>
		public static Either<V> SelectMany<T, U, V>(this Either<T> id, Func<T, Either<U>> k, Func<T, U, V> s)
		{
			return id.SelectMany(x =>
				 k(x).SelectMany(y => s(x, y).ToEither()));
		}
	}

	/// <summary>
	/// A simple example of using the either monad
	/// </summary>
	public static class EitherExample
	{
		private static Either<Account> getAccount(string name)
		{
			if (name.Equals("john"))
			{
				var account = new Account { Name = name, Id = 1 };
				return account.ToEither();
			}
			return new Either<Account>(new ArgumentException("user does not exist"));
		}

		private static Either<Balance> getBalance(Account account)
		{
			if (account.Id == 1)
			{
				var balance = new Balance { Id = account.Id, Value = 22.35m };
				return balance.ToEither();
			}
			return new Either<Balance>(new ArgumentException("user has no balances"));
		}

		private static Either<Loan> getLoan(Balance balance)
		{
			if (balance.Value >= 20.0m)
			{
				var loan = new Loan { Id = balance.Id, Value = 100.0m };
				return loan.ToEither();
			}
			return new Either<Loan>(new ArgumentException("user denied a loan"));
		}

		public static void main()
		{
			Console.WriteLine("\n\nEither Monad Example\n");
			var name = "john";
			var loan = from account in getAccount(name)
					   from balance in getBalance(account)
					   from money in getLoan(balance)
					   select money;

			Console.WriteLine("{0}", (loan.HasValue)
				? "user recieved " + loan.Value.Value + " dollar loan"
				: loan.Error.ToString());
		}
	}
}
