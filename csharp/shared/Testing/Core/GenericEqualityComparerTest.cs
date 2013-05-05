using System;
using System.Linq;
using System.Collections.Generic;
using SharedAssemblies.Core.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.Core.UnitTests
{    
    /// <summary>
    /// This is a test class for GenericEqualityComparerTest and is intended
    /// to contain all GenericEqualityComparerTest Unit Tests
    /// </summary>
	[TestClass]
	public class GenericEqualityComparerTest
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
			var target = EqualityComparer.Create<string>((s1, s2) => s1[0] == s2[0],
			                                                    s => s.Length);

			Assert.IsTrue(target.Equals("12345", "19999"));
		}

		/// <summary>
		/// Test for equals
		/// </summary>
		[TestMethod]
		public void EqualsTestOnFailure()
		{
			var target = EqualityComparer.Create<string>((s1, s2) => s1[0] == s2[0],
																s => s.Length);

			Assert.IsFalse(target.Equals("12345", "99999"));
		}

		/// <summary>
		/// Test for GetHashCode()
		/// </summary>
		[TestMethod]
		public void GetHashCodeTest()
		{
			var target = EqualityComparer.Create<string>((s1, s2) => s1[0] == s2[0],
																s => s.Length);

			Assert.AreEqual(5, target.GetHashCode("12345"));
		}
	}
}
