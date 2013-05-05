using SharedAssemblies.General.Database.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// This is a test class for MockDataAdapterTest and is intended
    /// to contain all MockDataAdapterTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockDataAdapterTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for MockDataAdapter Constructor
        /// </summary>
        [TestMethod]
        public void MockDataAdapterConstructorTest()
        {
            MockDataAdapter target = new MockDataAdapter();
        }
    }
}
