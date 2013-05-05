using SharedAssemblies.Core.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// This is a test class for CompositeKeyFactoryTest and is intended
    /// to contain all CompositeKeyFactoryTest Unit Tests
    /// </summary>
	[TestClass]
	public class CompositeKeyFactoryTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		[TestMethod]
		public void CreateBinaryTest()
		{
			var key = CompositeKeyFactory.Create("A", 1);

			Assert.IsInstanceOfType(key.Primary, typeof(string));
			Assert.IsInstanceOfType(key.Secondary, typeof(int));
		}

		[TestMethod]
		public void CreateTernaryTest()
		{
			var key = CompositeKeyFactory.Create("A", 1, 3.3);

			Assert.IsInstanceOfType(key.Primary, typeof(string));
			Assert.IsInstanceOfType(key.Secondary, typeof(int));
			Assert.IsInstanceOfType(key.Ternary, typeof(double));
		}
	}
}
