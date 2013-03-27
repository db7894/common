using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	public class Account
	{
		public string Name { get; set; }
		public int Id { get; set; }
	}

	public class Balance
	{
		public decimal Value { get; set; }
		public int Id { get; set; }
	}

	public class Loan
	{
		public decimal Value { get; set; }
		public int Id { get; set; }
	}
}
