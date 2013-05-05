using SharedAssemblies.Monitoring.AutoCounters;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoHeartbeatAttributeTest and is intended
    /// to contain all AutoHeartbeatAttributeTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoHeartbeatAttributeTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoHeartbeatAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoHeartbeatAttributeConstructorTest()
		{
			var target = new AutoHeartbeatAttribute();

			Assert.AreEqual(":", target.AbbreviatedName);
			Assert.AreEqual(AutoCounterType.ElapsedTime, target.AutoCounterType);
			Assert.AreEqual("", target.Category);
			Assert.AreEqual("", target.Description);
			Assert.IsFalse(target.IsReadOnly);
			Assert.AreEqual("", target.Name);
			Assert.AreEqual(":", target.UniqueName);
			Assert.AreEqual("sec", target.Units);
			Assert.AreEqual(5000, target.HeartbeatIntervalInMs);
		}

		/// <summary>
		/// A test for AutoHeartbeatAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoHeartbeatAttributeConstructorWithArgsTest()
		{
			var target = new AutoHeartbeatAttribute("CAT1", "HB1", 4000);

			Assert.AreEqual("CAT1:HB1", target.AbbreviatedName);
			Assert.AreEqual(AutoCounterType.ElapsedTime, target.AutoCounterType);
			Assert.AreEqual("CAT1", target.Category);
			Assert.AreEqual("HB1", target.Description);
			Assert.IsFalse(target.IsReadOnly);
			Assert.AreEqual("HB1", target.Name);
			Assert.AreEqual("CAT1:HB1", target.UniqueName);
			Assert.AreEqual("sec", target.Units);
			Assert.AreEqual(4000, target.HeartbeatIntervalInMs);
		}

		/// <summary>
		/// A test for HeartbeatIntervalInMs
		/// </summary>
		[TestMethod]
		public void HeartbeatIntervalInMsTest()
		{
			var expected = 1250;
			var target = new AutoHeartbeatAttribute { HeartbeatIntervalInMs = expected };

			Assert.AreEqual(expected, target.HeartbeatIntervalInMs);
		}
	}
}
