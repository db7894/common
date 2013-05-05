using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;

namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
	/// <summary>
	/// Units tests for the <see cref="NonLockingAutoCountersAttribute"/> object.
	/// </summary>
	[TestClass]
	public class NonLockingAutoCountersAttributeTest
	{
		/// <summary>
		/// Tests default construction to ensure that values are set to
		/// valid defaults.
		/// </summary>
		[TestMethod]
		public void NonLockingAutoCountersAttributeDefaultTest()
		{
			var attr = new NonLockingAutoCountersAttribute();
			Assert.AreEqual(500, attr.UpdateInterval);
			Assert.IsTrue(attr.UseDedicatedThread);
			Assert.AreEqual(ThreadPriority.BelowNormal, attr.DedicatedThreadPriority);
		}

		/// <summary>
		/// Tests the constructor to ensure that input validation is occurring.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void NonLockingAutoCountersAttributeConstructorTest()
		{
			new NonLockingAutoCountersAttribute(-1);
		}	
	}
}
