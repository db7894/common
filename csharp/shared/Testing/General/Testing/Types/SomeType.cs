using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedAssemblies.General.Testing.UnitTests.Types
{
	/// <summary>
	/// A helper type that we can test with. Notice that the class must be
	/// public as we have to use an activator to instantiate a new instance
	/// in the general utilities assembly (internal cannot be used!)
	/// </summary>
	public class SomeType
	{
		/// <summary>
		/// A numeric field
		/// </summary>
		public int Number { get; set; }

		/// <summary>
		/// A name field
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// A double field
		/// </summary>
		public double Interest { get; set; }
	}
}
