using System.Data.Common;
using SharedAssemblies.General.Database.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// This is a test class for MockClientFactoryLazyInstanceTest and is intended
    /// to contain all MockClientFactoryLazyInstanceTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockClientFactoryLazyInstanceTest
    {
        /// <summary>
        /// A test for LazyInstance Constructor
        /// </summary>
        [TestMethod]
        public void MockClientFactory_LazyInstanceConstructorTest()
        {
            DbProviderFactory expected = MockClientFactory.Instance;
            DbProviderFactory actual = MockClientFactory.Instance;

            Assert.AreSame(expected, actual);
        }
    }
}
