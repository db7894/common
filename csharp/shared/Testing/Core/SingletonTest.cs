using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Patterns;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// test fixture for singleton
    /// </summary>
    [TestClass]
    public class SingletonTest
    {
        /// <summary>
        /// The test context property for MSTest
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Test that constructor is lazy
        /// </summary>
        [TestMethod]
		[Ignore] // This will _always_ fail if the test below runs first
        public void Constructor_ConstructsLazy_WhenInvoked()
        {
            DateTime preConstruction = DateTime.Now;

            Thread.Sleep(1000);

            // sleep to make sure ticks change
            InstantTime time = Singleton<InstantTime>.Instance;

            // the singleton shouldn't get created until the first call
            // to .Instance, not before.
            Assert.IsTrue(preConstruction < time.Time);
        }


        /// <summary>
        /// Test that instance is always same isntance
        /// </summary>
        [TestMethod]
        public void Instance_ReturnsSameInstance_WheneverUsed()
        {
            // sleep to make sure ticks change
            InstantTime time = Singleton<InstantTime>.Instance;

            // sleep to make sure ticks change
            Thread.Sleep(1000);

            InstantTime secondTime = Singleton<InstantTime>.Instance;


            Assert.AreEqual(time, secondTime);
        }
    }
}