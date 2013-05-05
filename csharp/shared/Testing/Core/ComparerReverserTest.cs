using System;
using SharedAssemblies.Core.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Core.UnitTests
{       
	/// <summary>
	/// This is a test class for ComparerReverserTest and is intended
	/// to contain all ComparerReverserTest Unit Tests
	/// </summary>
	[TestClass]
	public class ComparerReverserTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for Compare
		/// </summary>
		[TestMethod]
		public void ComparerReverserReversesStringComparerTest()
		{
			var comparer = new ComparerReverser<string>(StringComparer.CurrentCultureIgnoreCase);

			Assert.AreEqual(-StringComparer.CurrentCultureIgnoreCase.Compare("Apple", "Axe"), comparer.Compare("Apple", "Axe"));
			Assert.AreEqual(-StringComparer.CurrentCultureIgnoreCase.Compare("Apple", "Apple"), comparer.Compare("Apple", "Apple"));
			Assert.AreEqual(-StringComparer.CurrentCultureIgnoreCase.Compare("Axe", "Apple"), comparer.Compare("Axe", "Apple"));
		}

		/// <summary>
		/// A test for Compare with nulls.
		/// </summary>
		[TestMethod]
		public void ComparerReverserReversesNullComparesTest()
		{
			var comparer = new ComparerReverser<string>(StringComparer.CurrentCultureIgnoreCase);

			Assert.AreEqual(1, comparer.Compare(null, "Axe"));
			Assert.AreEqual(-1, comparer.Compare("Apple", null));
			Assert.AreEqual(0, comparer.Compare(null, null));
		}

		/// <summary>
		/// A test for Compare with nulls.
		/// </summary>
		[TestMethod]
		public void ComparerReverserReversesStringComparerNullTest()
		{
			var comparer = new ComparerReverser<string>(StringComparer.CurrentCultureIgnoreCase);

			Assert.AreEqual(-StringComparer.CurrentCultureIgnoreCase.Compare(null, "Axe"), comparer.Compare(null, "Axe"));
			Assert.AreEqual(-StringComparer.CurrentCultureIgnoreCase.Compare("Apple", null), comparer.Compare("Apple", null));
			Assert.AreEqual(-StringComparer.CurrentCultureIgnoreCase.Compare(null, null), comparer.Compare(null, null));
		}
	}
}
