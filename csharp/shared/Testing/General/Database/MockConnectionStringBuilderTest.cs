using SharedAssemblies.General.Database.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// This is a test class for MockConnectionStringBuilderTest and is intended
    /// to contain all MockConnectionStringBuilderTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockConnectionStringBuilderTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for MockConnectionStringBuilder Constructor
        /// </summary>
        [TestMethod]
        public void MockConnectionStringBuilderConstructorTest()
        {
            MockConnectionStringBuilder target = new MockConnectionStringBuilder();

            target["User Id"] = "User001";
            target["Password"] = "Password001";
            target["Initial Catalog"] = "Server001";
            target["Data Source"] = "Orders";

            Assert.AreEqual(
				"User Id=User001;Password=Password001;Initial Catalog=Server001;Data Source=Orders",
                target.ConnectionString);
        }
    }
}
