using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{	
	public static class NullBusinessLogic
	{
		private static Account getAccount(string name)
		{
			if (name.Equals("john"))
			{
				return new Account { Name = name, Id = 1 };
			}
			return null;
		}

		private static Balance getBalance(Account account)
		{
			if (account.Id == 1)
			{
				return new Balance { Id = account.Id, Value = 22.35m };
			}
			return null;
		}

		private static Loan getLoan(Balance balance)
		{
			if (balance.Value >= 20.0m)
			{
				return new Loan { Id = balance.Id, Value = 100.0m };
			}
			return null;
		}

		public static void main()
		{
			Console.WriteLine("\n\nNull Business Logic Example\n");
			var name = "john";
			var account = getAccount(name);
			if (account != null)
			{
				var balance = getBalance(account);
				if (balance != null)
				{
					var loan = getLoan(balance);
					if (loan != null)
					{
						Console.WriteLine("recieved {0} loan", (loan != null) ? loan.Value + " dollars " : "no");
					}
				}
			}

		}
	}
}
