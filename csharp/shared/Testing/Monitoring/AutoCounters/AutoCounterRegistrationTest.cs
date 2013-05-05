using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoCounterRegistrationTest and is intended
    /// to contain all AutoCounterRegistrationTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoCounterRegistrationTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoCounterRegistration Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterRegistrationConstructorTest()
		{
			var target = new AutoCounterRegistration(InstanceType.MultiInstance, "XYZ", CreateFailedAction.CreateStub, null);

			Assert.AreEqual(InstanceType.MultiInstance, target.InstanceType);
			Assert.AreEqual("XYZ", target.UniqueName);
			Assert.IsNull(target.Name);
			Assert.IsNull(target.Category);
			Assert.IsNull(target.Description);
			Assert.IsFalse(target.IsReadOnly);
			Assert.AreEqual(AutoCounterType.Unknown, target.Type);
		}

		/// <summary>
		/// A test for GetCounter
		/// </summary>
		[TestMethod]
		public void GetCounterTest()
		{
			var target = new AutoCounterRegistration(InstanceType.SingleInstance, "XYZ", CreateFailedAction.CreateStub, null)
			             	{
			             		Category = new AutoCounterCategoryRegistration(InstanceType.SingleInstance, "CAT")
			             	};

			var result = target.GetCounter();

			Assert.IsNotNull(result);
		}

		/// <summary>
		/// A test for GetCounterInstance
		/// </summary>
		[TestMethod]
		public void GetCounterInstanceTest()
		{
			var target = new AutoCounterRegistration(InstanceType.MultiInstance, "XYZ", CreateFailedAction.CreateStub, null)
			             	{
			             		Category = new AutoCounterCategoryRegistration(InstanceType.MultiInstance, "CAT")
			             	};

			var result = target.GetCounterInstance("I1");

			Assert.IsNotNull(result);
		}

		/// <summary>
		/// A test for Category
		/// </summary>
		[TestMethod]
		public void CategoryTest()
		{
			var expected = new AutoCounterCategoryRegistration(InstanceType.MultiInstance, "CAT1");
			var target = new AutoCounterRegistration(InstanceType.MultiInstance, "XYZ", CreateFailedAction.CreateStub, null)
						 { Category = expected };

			Assert.AreSame(expected, target.Category);
		}

		/// <summary>
		/// A test for Description
		/// </summary>
		[TestMethod]
		public void DescriptionTest()
		{
			var expected = "D1";
			var target = new AutoCounterRegistration(InstanceType.MultiInstance, "XYZ", CreateFailedAction.CreateStub, null)
							 { Description = expected };

			Assert.AreEqual(expected, target.Description);
		}

		/// <summary>
		/// A test for IsReadOnly
		/// </summary>
		[TestMethod]
		public void IsReadOnlyTest()
		{
			var expected = true;
			var target = new AutoCounterRegistration(InstanceType.MultiInstance, "XYZ", CreateFailedAction.CreateStub, null) 
							{ IsReadOnly = expected };

			Assert.AreEqual(expected, target.IsReadOnly);
		}

		/// <summary>
		/// A test for Name
		/// </summary>
		[TestMethod]
		public void NameTest()
		{
			var expected = "C1";
			var target = new AutoCounterRegistration(InstanceType.MultiInstance, "XYZ", CreateFailedAction.CreateStub, null)
							 { Name = expected };

			Assert.AreEqual(expected, target.Name);
		}

		/// <summary>
		/// A test for Type
		/// </summary>
		[TestMethod]
		public void TypeTest()
		{
			var expected = AutoCounterType.RollingAverageTime;
			var target = new AutoCounterRegistration(InstanceType.MultiInstance, "XYZ", CreateFailedAction.CreateStub, null)
							 { Type = expected };

			Assert.AreEqual(expected, target.Type);
		}
	}
}
