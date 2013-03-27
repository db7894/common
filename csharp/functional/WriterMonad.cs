using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	public class Writer<T> : IMonad
	{
		public T Value { get; set; }
		public List<string> Log { get; set; }

		public Writer(T value)
		{
			Value = value;
			Log = new List<string>();
		}
	}

	/// <summary>
	/// C# "do" notation
	/// </summary>
	public static class WriterExtensions
	{
		/// <summary>
		/// Unit
		/// </summary>
		public static Writer<T> ToWriter<T>(this T value)
		{
			return new Writer<T>(value);
		}

		/// <summary>
		/// Unit
		/// </summary>
		public static Writer<T> ToWriter<T>(this T value, string message)
		{
			var writer = new Writer<T>(value);
			writer.Log.Add(message);
			return writer;
		}

		/// <summary>
		/// Bind
		/// </summary>
		public static Writer<U> SelectMany<T, U>(this Writer<T> id, Func<T, Writer<U>> k)
		{
			var writer = k(id.Value);
			writer.Log = id.Log.Concat(writer.Log).ToList();
			return writer;
		}

		/// <summary>
		/// Bind + Unit
		/// </summary>
		public static Writer<V> SelectMany<T, U, V>(this Writer<T> id, Func<T, Writer<U>> k, Func<T, U, V> s)
		{
			return id.SelectMany(x =>
				 k(x).SelectMany(y => s(x, y).ToWriter()));
		}
	}

	/// <summary>
	/// A simple example of using the Writer monad
	/// </summary>
	public static class WriterExample
	{
		private static Writer<Account> getAccount(string name)
		{
			var account = new Account { Name = name, Id = 1 };
			return account.ToWriter("added account for " + name);
		}

		private static Writer<Balance> getBalance(Account account)
		{
			var balance = new Balance { Id = account.Id, Value = 22.35m };
			return balance.ToWriter("added balance for " + account.Id);
		}

		private static Writer<Loan> getLoan(Balance balance)
		{
			var loan = new Loan { Id = balance.Id, Value = 100.0m };
			return loan.ToWriter("added available loan for " + balance.Id);
		}

		public static void main()
		{
			Console.WriteLine("\n\nWriter Monad Example\n");
			var name = "john";
			var loan = from account in getAccount(name)
					   from balance in getBalance(account)
					   from money in getLoan(balance)
					   select money;

			Console.WriteLine("user recieved a {0} dollar loan", loan.Value.Value);
			foreach (var message in loan.Log)
			{
				Console.WriteLine(message);
			}
		}
	}
}
