using System;
using SharedAssemblies.Core.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Core.UnitTests
{    
    /// <summary>
    /// This is a test class for KeyEqualityComparerTest and is intended
    /// to contain all KeyEqualityComparerTest Unit Tests
    /// </summary>
	[TestClass]
	public class KeyEqualityComparerTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Test for equals
		/// </summary>
		[TestMethod]
		public void EqualsTestOnSuccess()
		{
			var target = EqualityComparer.Create<string, int>(s => s.Length);

			Assert.IsTrue(target.Equals("12345", "ABCDE"));
		}

		/// <summary>
		/// Test for equals
		/// </summary>
		[TestMethod]
		public void EqualsTestOnFailure()
		{
			var target = EqualityComparer.Create<string, int>(s => s.Length);

			Assert.IsFalse(target.Equals("12345", "123456789"));
		}

		/// <summary>
		/// Test for GetHashCode()
		/// </summary>
		[TestMethod]
		public void GetHashCodeTest()
		{
			var target = EqualityComparer.Create<string, int>(s => s.Length);

			Assert.AreEqual(5.GetHashCode(), target.GetHashCode("12345"));
		}
	}
}
