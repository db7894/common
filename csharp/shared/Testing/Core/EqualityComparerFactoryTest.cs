using System;
using System.Linq;
using System.Collections.Generic;
using SharedAssemblies.Core.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// This is a test class for EqualityComparerFactoryTest and is intended
    /// to contain all EqualityComparerFactoryTest Unit Tests
    /// </summary>
	[TestClass]
	public class EqualityComparerFactoryTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// Test to make sure null key extractors throw
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateTestForKeyExtractorWithNullArg()
		{
			Func<string, int> extractor = null;

			EqualityComparer.Create(extractor);
		}

		/// <summary>
		/// Test to make sure returns a key extractor
		/// </summary>
		[TestMethod]
		public void CreateTestForKeyExtractor()
		{
			var actual = EqualityComparer.Create<string, int>(s => s.Length);

			Assert.IsInstanceOfType(actual, typeof(KeyEqualityComparer<string, int>));

			// quick test to make sure it's extracting correctly.
			Assert.IsTrue(actual.Equals("12345", "54321"));
			Assert.AreEqual(actual.GetHashCode("12345"), actual.GetHashCode("54321"));
		}

		/// <summary>
		/// A test to make sure null delegates throw
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateTestForGenericWithNullComparerArg()
		{
			Func<string, string, bool> comparer = null;
			Func<string, int> hasher = s => s.GetHashCode();

			EqualityComparer.Create(comparer, hasher);
		}

		/// <summary>
		/// A test to make sure null delegates throw
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateTestForGenericWithNullHasherArg()
		{
			Func<string, string, bool> comparer = (lhs, rhs) => lhs == rhs;
			Func<string, int> hasher = null;

			EqualityComparer.Create(comparer, hasher);
		}

		/// <summary>
		/// Test to make sure returns a key extractor
		/// </summary>
		[TestMethod]
		public void CreateTestForGenericExtractor()
		{
			var actual = EqualityComparer.Create<string>((lhs, rhs) => lhs == rhs, 
				obj => 13);

			Assert.IsInstanceOfType(actual, typeof(GenericEqualityComparer<string>));

			// quick test to make sure it's extracting correctly.
			Assert.IsTrue(actual.Equals("12345", "12345"));
			Assert.AreEqual(13, actual.GetHashCode("12345"));
		}
	}
}
