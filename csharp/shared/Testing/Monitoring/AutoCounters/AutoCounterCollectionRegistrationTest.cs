using System;
using System.Collections.Generic;

using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoCounterCollectionRegistrationTest and is intended
    /// to contain all AutoCounterCollectionRegistrationTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoCounterCollectionRegistrationTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoCounterCollectionRegistration Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterCollectionRegistrationConstructorTest()
		{
			var target = new AutoCounterCollectionRegistration(InstanceType.MultiInstance, "XYZ", new List<AutoCounterRegistration>(), 
																				CreateFailedAction.CreateStub, null);

			Assert.AreEqual("XYZ", target.UniqueName);
			Assert.AreEqual(InstanceType.MultiInstance, target.InstanceType);
			Assert.AreEqual(null, target.ParentCollection);
		}

		/// <summary>
		/// A test for GetCounterInstance
		/// </summary>
		[TestMethod]
		public void GetCounterTest()
		{
			var target = new AutoCounterCollectionRegistration(InstanceType.SingleInstance, "XYZ",
												new List<AutoCounterRegistration>(), 
												CreateFailedAction.CreateStub, null);

			var result = target.GetCounter();

			Assert.IsNotNull(result);
		}

		/// <summary>
		/// A test for GetCounterInstance
		/// </summary>
		[TestMethod]
		public void GetCounterInstanceTest()
		{
			var target = new AutoCounterCollectionRegistration(InstanceType.MultiInstance, "XYZ", 
																	new List<AutoCounterRegistration>(),
																	CreateFailedAction.CreateStub, null);

			var result = target.GetCounterInstance("I1");

			Assert.IsNotNull(result);
		}

		/// <summary>
		/// A test for ParentCollection
		/// </summary>
		[TestMethod]
		public void ParentCollectionTest()
		{
			var expected = new AutoCounterCollectionRegistration(InstanceType.MultiInstance, "PDQ", 
																	new List<AutoCounterRegistration>(), 
																	CreateFailedAction.CreateStub, null);
			var target = new AutoCounterCollectionRegistration(InstanceType.MultiInstance, "XYZ", 
																	new List<AutoCounterRegistration>(), 
																	CreateFailedAction.CreateStub, null) 
							{ ParentCollection = expected };

			Assert.AreSame(expected, target.ParentCollection);
		}
	}
}
