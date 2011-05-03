using System;
using System.Collections.Generic;

namespace SharedAssemblies.General.Validation.Tests.Types
{
	/// <summary>
	/// A simple type that we can use for testing
	/// </summary>
	public class ExampleClassType
	{
		public string Prefix { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Age { get; set; }
		public bool Insured { get; set; }
		public DateTime Birthday { get; set; }
		public char Letter { get; set; }
		public TimeSpan Time { get; set; }
		public double Balance { get; set; }
		public int? NullableType { get; set; }
		public List<char> Collection { get; set; }
	}
}
