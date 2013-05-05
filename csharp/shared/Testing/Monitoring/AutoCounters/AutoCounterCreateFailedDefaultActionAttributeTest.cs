using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoCounterCreateFailedDefaultActionAttributeTest and is intended
    /// to contain all AutoCounterCreateFailedDefaultActionAttributeTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoCounterCreateFailedDefaultActionAttributeTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoCounterCreateFailedDefaultActionAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCreateFailedDefaultActionAttributeConstructorTest()
		{
			var target = new AutoCounterCreateFailedDefaultActionAttribute();

			Assert.AreEqual(CreateFailedAction.ThrowException, target.DefaultCreateFailedAction);
		}

		/// <summary>
		/// A test for AutoCounterCreateFailedDefaultActionAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCreateFailedDefaultActionAttributeConstructorWithArgsTest()
		{
			var target = new AutoCounterCreateFailedDefaultActionAttribute(CreateFailedAction.Default);

			Assert.AreEqual(CreateFailedAction.Default, target.DefaultCreateFailedAction);
		}

		/// <summary>
		/// A test for DefaultCreateFailedAction
		/// </summary>
		[TestMethod]
		public void DefaultCreateFailedActionTest()
		{
			var target = new AutoCounterCreateFailedDefaultActionAttribute { DefaultCreateFailedAction = CreateFailedAction.CreateStub };

			Assert.AreEqual(CreateFailedAction.CreateStub, target.DefaultCreateFailedAction);
		}
	}
}
