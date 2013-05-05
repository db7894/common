using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters;
using System.Collections.Generic;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoCounterCategoryRegistrationTest and is intended
    /// to contain all AutoCounterCategoryRegistrationTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoCounterCategoryRegistrationTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoCounterCategoryRegistration Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCategoryRegistrationConstructorTest()
		{
			var target = new AutoCounterCategoryRegistration(InstanceType.MultiInstance, "XYZ");

			Assert.AreEqual("XYZ", target.UniqueName);
			Assert.IsNull(target.Description);
			Assert.AreEqual(0, target.AutoCounters.Count);
			Assert.AreEqual(InstanceType.MultiInstance, target.InstanceType);
		}

		/// <summary>
		/// A test for GetCounter
		/// </summary>
		[TestMethod]
		public void GetCounterTest()
		{
			var target = new AutoCounterCategoryRegistration(InstanceType.SingleInstance, "XYZ")
				{
					Description = "A",
				};

			var counter = new AutoCounterRegistration(InstanceType.SingleInstance, "C1", CreateFailedAction.CreateStub, null);

			target.AutoCounters.Add("C1", counter);

			Assert.IsNull(target.GetCounter());
		}

		/// <summary>
		/// A test for GetCounter when the underlying counter collection should be non-locking.
		/// </summary>
		[TestMethod]
		public void GetCounterNonLockingTest()
		{
			var target = new AutoCounterCategoryRegistration(InstanceType.SingleInstance, "XYZ")
			{
				Description = "A",
			};

			var counter = new AutoCounterRegistration(InstanceType.SingleInstance, "C1", CreateFailedAction.CreateStub, null);

			target.AutoCounters.Add("C1", counter);

			Assert.IsNull(target.GetCounter());
		}



		/// <summary>
		/// A test for GetCounterInstance
		/// </summary>
		[TestMethod]
		public void GetCounterInstanceTest()
		{
			var target = new AutoCounterCategoryRegistration(InstanceType.MultiInstance, "XYZ")
			{
				Description = "A",
			};

			var counter = new AutoCounterRegistration(InstanceType.MultiInstance, "C1", CreateFailedAction.CreateStub, null);

			target.AutoCounters.Add("C1", counter);

			Assert.IsNull(target.GetCounterInstance("I1"));
		}

		/// <summary>
		/// A test for GetCounterInstance when then underlying counter collection should
		/// be non-locking.
		/// </summary>
		[TestMethod]
		public void GetCounterInstanceNonLockingTest()
		{
			var target = new AutoCounterCategoryRegistration(InstanceType.MultiInstance, "XYZ")
			{
				Description = "A",
			};

			var counter = new AutoCounterRegistration(InstanceType.MultiInstance, "C1", CreateFailedAction.CreateStub, null);

			target.AutoCounters.Add("C1", counter);

			Assert.IsNull(target.GetCounterInstance("I1"));
		}

		/// <summary>
		/// A test for Description
		/// </summary>
		[TestMethod]
		public void DescriptionTest()
		{
			var target = new AutoCounterCategoryRegistration(InstanceType.MultiInstance, "XYZ") { Description = "D1" };

			Assert.AreEqual("D1", target.Description);
		}
	}
}
