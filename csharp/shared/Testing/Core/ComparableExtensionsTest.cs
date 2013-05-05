using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// Some tests for the comparable extension methods.
	/// </summary>
	[TestClass]
	public class ComparableExtensionsTest
	{
		/// <summary>
		/// This tests that all the between operations work correctly with all the
		/// specified range values.
		/// </summary>
		[TestMethod]
		public void IsBetween_WithVariousRanges_AllSucceed()
		{	
			Assert.IsTrue(22.IsBetween(0, 30));
			Assert.IsTrue(22.IsBetween(22, 22));
			Assert.IsFalse(22.IsBetween(25, 30));
			Assert.IsTrue(22.IsBetween(0, 30, BetweenComparison.Exclusive));
			Assert.IsFalse(22.IsBetween(0, 22, BetweenComparison.Exclusive));
			Assert.IsTrue(22.IsBetween(0, 22, BetweenComparison.Inclusive));
			Assert.IsTrue(22.IsBetween(22, 22, BetweenComparison.Inclusive));
		}

		/// <summary>
		/// This tests that all the between operations work correctly with all the
		/// specified range values.
		/// </summary>
		[TestMethod]
		public void IsBetween_WithDifferentTypes_AllSucceed()
		{
			Assert.IsTrue('b'.IsBetween('a', 'c'));
			Assert.IsTrue('Z'.IsBetween('Y', 'Z'));
			Assert.IsTrue(22.22.IsBetween(22.00, 23.00));
            
			var date = DateTime.Now;
			Assert.IsTrue(date.IsBetween(date.AddYears(-1), date.AddYears(1)));
			Assert.IsTrue("beta".IsBetween("alpha", "gamma"));
		}

		/// <summary>
		/// This tests that all the between operations work correctly with all the
		/// specified range values.
		/// </summary>
		[TestMethod]
		public void ConstrainTo_WithDifferentTypes_AllSucceed()
		{
			Assert.AreEqual(1.ConstrainTo(10, 20), 10);
			Assert.AreEqual(100.ConstrainTo(10, 20), 20);
			Assert.AreEqual('z'.ConstrainTo('a', 'c'), 'c');
			Assert.AreEqual('G'.ConstrainTo('G', 'R'), 'G');

			var date = DateTime.Now;
			Assert.AreEqual(date.ConstrainTo(date.AddYears(-1), date.AddYears(1)), date);
			
			Assert.AreEqual("gamma".ConstrainTo("alpha", "beta"), "beta");
		}
	}
}
