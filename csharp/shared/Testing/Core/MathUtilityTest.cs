using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Math;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// Test fixture for int extensions
	/// </summary>
	[TestClass]
	public class MathUtilityTest
	{
		/// <summary>
		/// Test method for the fibonacci sequence.
		/// </summary>
		[TestMethod]
		public void Fibonacci_ReturnsSequence_WhenCalled()
		{
			var expected = new[] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
			var actual = MathUtility.GetFibonacciSequence().Take(10).ToArray();

			CollectionAssert.AreEquivalent(expected, actual);
		}
	}
}