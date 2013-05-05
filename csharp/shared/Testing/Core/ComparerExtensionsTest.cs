using SharedAssemblies.Core.Comparers;
using SharedAssemblies.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;


namespace SharedAssemblies.Core.UnitTests
{    
    /// <summary>
    /// This is a test class for ComparerExtensionsTest and is intended
    /// to contain all ComparerExtensionsTest Unit Tests
    /// </summary>
	[TestClass]
	public class ComparerExtensionsTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// Test that Reverse() extension method returns a reverse comparer.
		/// </summary>
		[TestMethod]
		public void ReverseReturnsRightInstanceTest()
		{
			Assert.IsInstanceOfType(Comparer<string>.Default.Reverse(), typeof(ComparerReverser<string>));
		}

		/// <summary>
		/// Test that Reverse() extension method returns a reverse comparer.
		/// </summary>
		[TestMethod]
		public void ReverseReverses()
		{
			var list = new List<string> {"A", "C", "B", "F", "D", "E"};

			var orderedList = list.OrderBy(l => l, Comparer<string>.Default).ToList();
			var reverseList = list.OrderBy(l => l, Comparer<string>.Default.Reverse()).ToList();

			Assert.AreEqual(orderedList[0], reverseList[5]);
			Assert.AreEqual(orderedList[1], reverseList[4]);
			Assert.AreEqual(orderedList[2], reverseList[3]);
			Assert.AreEqual(orderedList[3], reverseList[2]);
			Assert.AreEqual(orderedList[4], reverseList[1]);
			Assert.AreEqual(orderedList[5], reverseList[0]);
		}

		/// <summary>
		/// Test that Reverse() extension method returns a reverse comparer.
		/// </summary>
		[TestMethod]
		public void ReverseOfReverseRevertsToOriginal()
		{
			var list = new List<string> { "A", "C", "B", "F", "D", "E" };

			var orderedList = list.OrderBy(l => l, Comparer<string>.Default).ToList();
			var reverseList = list.OrderBy(l => l, Comparer<string>.Default.Reverse().Reverse()).ToList();

			Assert.AreEqual(orderedList[0], reverseList[0]);
			Assert.AreEqual(orderedList[1], reverseList[1]);
			Assert.AreEqual(orderedList[2], reverseList[2]);
			Assert.AreEqual(orderedList[3], reverseList[3]);
			Assert.AreEqual(orderedList[4], reverseList[4]);
			Assert.AreEqual(orderedList[5], reverseList[5]);
		}
	}
}
