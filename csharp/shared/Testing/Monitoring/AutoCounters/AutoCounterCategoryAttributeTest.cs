using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters;

namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoCounterCategoryAttributeTest and is intended
    /// to contain all AutoCounterCategoryAttributeTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoCounterCategoryAttributeTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoCounterCategoryAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCategoryAttributeConstructorTest()
		{
			var target = new AutoCounterCategoryAttribute();

			Assert.AreEqual("", target.Name);
			Assert.AreEqual(InstanceType.SingleInstance, target.InstanceType);
			Assert.AreEqual("", target.Description);
		}

		/// <summary>
		/// A test for AutoCounterCategoryAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCategoryAttributeConstructorWithArgsTest()
		{
			var target = new AutoCounterCategoryAttribute("XYZ");

			Assert.AreEqual("XYZ", target.Name);
			Assert.AreEqual(InstanceType.SingleInstance, target.InstanceType);
			Assert.AreEqual("XYZ", target.Description);
		}

		/// <summary>
		/// A test for Description
		/// </summary>
		[TestMethod]
		public void DescriptionTest()
		{
			var target = new AutoCounterCategoryAttribute { Description = "XYZ" };

			Assert.AreEqual("XYZ", target.Description);
		}

		/// <summary>
		/// A test for InstanceType
		/// </summary>
		[TestMethod]
		public void InstanceTypeTest()
		{
			var target = new AutoCounterCategoryAttribute { InstanceType = InstanceType.MultiInstance };

			Assert.AreEqual(InstanceType.MultiInstance, target.InstanceType);
		}

		/// <summary>
		/// A test for Name
		/// </summary>
		[TestMethod]
		public void NameTest()
		{
			var target = new AutoCounterCategoryAttribute { Name = "XYZ" };

			Assert.AreEqual("XYZ", target.Name);
		}
	}
}
