using SharedAssemblies.Core.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// This is a test class for ReverseComparerTest and is intended
	/// to contain all ReverseComparerTest Unit Tests
	/// </summary>
	[TestClass]
	public class ReverseComparerTest
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
		public void ReverseComparerReversesStringCompareToTest()
		{
			var comparer = new ReverseComparer<string>();

			Assert.AreEqual(- "Apple".CompareTo("Axe"), comparer.Compare("Apple", "Axe"));
			Assert.AreEqual(- "Apple".CompareTo("Apple"), comparer.Compare("Apple", "Apple"));
			Assert.AreEqual(- "Axe".CompareTo("Apple"), comparer.Compare("Axe", "Apple"));
		}

		/// <summary>
		/// A test for Compare
		/// </summary>
		[TestMethod]
		public void ReverseComparerReversesNullComparesTest()
		{
			var comparer = new ReverseComparer<string>();

			Assert.AreEqual(-1, comparer.Compare("Apple", null));
			Assert.AreEqual(0, comparer.Compare(null, null));
			Assert.AreEqual(1, comparer.Compare(null, "Apple"));
		}

		/// <summary>
		/// A test for Compare
		/// </summary>
		[TestMethod]
		public void ReverseComparerReversesStringCompareToNullTest()
		{
			var comparer = new ReverseComparer<string>();

			// only one we can test with CompareTo
			Assert.AreEqual(-"Apple".CompareTo(null), comparer.Compare("Apple", null));
		}
	}
}
