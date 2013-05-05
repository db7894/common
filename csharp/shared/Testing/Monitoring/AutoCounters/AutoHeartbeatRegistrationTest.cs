using SharedAssemblies.Monitoring.AutoCounters;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoHeartbeatRegistrationTest and is intended
    /// to contain all AutoHeartbeatRegistrationTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoHeartbeatRegistrationTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoHeartbeatRegistration Constructor
		/// </summary>
		[TestMethod]
		public void AutoHeartbeatRegistrationConstructorTest()
		{
			var wrapped = new AutoCounterRegistration(InstanceType.SingleInstance, "XYZ", CreateFailedAction.CreateStub, null);
			var target = new AutoHeartbeatRegistration(wrapped, 1500);

			Assert.AreSame(wrapped, target.Counter);
			Assert.AreEqual(1500.0, target.Timer.Interval);
		}

		/// <summary>
		/// A test for Dispose
		/// </summary>
		[TestMethod]
		public void DisposeTest()
		{
			var wrapped = new AutoCounterRegistration(InstanceType.SingleInstance, "XYZ", CreateFailedAction.CreateStub, null);
			var target = new AutoHeartbeatRegistration(wrapped, 1500);

			target.Start();
			target.Dispose();

			Assert.IsFalse(target.Timer.Enabled);
		}

		/// <summary>
		/// A test for Start
		/// </summary>
		[TestMethod]
		public void StartTest()
		{
			var wrapped = new AutoCounterRegistration(InstanceType.SingleInstance, "XYZ", CreateFailedAction.CreateStub, null);
			var target = new AutoHeartbeatRegistration(wrapped, 1500);

			target.Start();

			Assert.IsTrue(target.Timer.Enabled);
		}

		/// <summary>
		/// A test for Stop
		/// </summary>
		[TestMethod]
		public void StopTest()
		{
			var wrapped = new AutoCounterRegistration(InstanceType.SingleInstance, "XYZ", CreateFailedAction.CreateStub, null);
			var target = new AutoHeartbeatRegistration(wrapped, 1500);

			target.Start();
			target.Stop();

			Assert.IsFalse(target.Timer.Enabled);
		}
	}
}
