using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;

namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// A collection of tests to check the correctness of the SA.C.Containers
	/// </summary>
	[TestClass]
	public class ContainerTest
	{
		/// <summary>
		/// This tests that the KeyValuePair factory generates the same
		/// value type as manually creating a KeyValuePair.
		/// </summary>
		[TestMethod]
		public void TestThat_KeyValuePairFactory_ProducesCorrectType()
		{
			var actual = KeyValuePairFactory.Create("Name", 2);
			var expected = new KeyValuePair<string, int>("Name", 2);
			Assert.AreEqual(expected, actual);
		}
	}
}
