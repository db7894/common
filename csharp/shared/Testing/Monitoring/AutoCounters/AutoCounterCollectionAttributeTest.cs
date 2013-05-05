using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{    
    /// <summary>
    /// This is a test class for AutoCounterCollectionAttributeTest and is intended
    /// to contain all AutoCounterCollectionAttributeTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoCounterCollectionAttributeTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoCounterCollectionAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCollectionAttributeDefaultConstructorTest()
		{
			var target = new AutoCounterCollectionAttribute();

			Assert.AreEqual(0, target.AutoCounters.Length);
			Assert.AreEqual(InstanceType.SingleInstance, target.InstanceType);
			Assert.AreEqual("", target.Name);
			Assert.IsNull(target.ParentCollection);
		}

		/// <summary>
		/// A test for AutoCounterCollectionAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCollectionAttributeConstructorWithArgsTest()
		{
			var target = new AutoCounterCollectionAttribute("ACC1", "C1", "C2", "C3");

			Assert.AreEqual(3, target.AutoCounters.Length);
			Assert.AreEqual("C1", target.AutoCounters[0]);
			Assert.AreEqual("C2", target.AutoCounters[1]);
			Assert.AreEqual("C3", target.AutoCounters[2]);
			Assert.AreEqual(InstanceType.SingleInstance, target.InstanceType);
			Assert.AreEqual("ACC1", target.Name);
			Assert.IsNull(target.ParentCollection);
		}

		/// <summary>
		/// A test for AutoCounters
		/// </summary>
		[TestMethod]
		public void AutoCountersTest()
		{
			var expected = new [] { "C1", "C2", "C3" };
			var target = new AutoCounterCollectionAttribute("ACC1") { AutoCounters = expected };

			Assert.AreSame(expected, target.AutoCounters);
		}

		/// <summary>
		/// A test for InstanceType
		/// </summary>
		[TestMethod]
		public void InstanceTypeTest()
		{
			var expected = InstanceType.MultiInstance;
			var target = new AutoCounterCollectionAttribute("ACC1") { InstanceType = expected };

			Assert.AreEqual(expected, target.InstanceType);
		}

		/// <summary>
		/// A test for Name
		/// </summary>
		[TestMethod]
		public void NameTest()
		{
			var expected = "ZZZ";
			var target = new AutoCounterCollectionAttribute("ACC1") { Name = expected };

			Assert.AreEqual(expected, target.Name);
		}

		/// <summary>
		/// A test for ParentCollection
		/// </summary>
		[TestMethod]
		public void ParentCollectionTest()
		{
			var expected = "P1";
			var target = new AutoCounterCollectionAttribute("ACC1") { ParentCollection = expected };

			Assert.AreEqual(expected, target.ParentCollection);
		}
	}
}
