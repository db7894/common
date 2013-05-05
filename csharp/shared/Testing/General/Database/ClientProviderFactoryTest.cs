using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Common;
using SharedAssemblies.General.Database.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Database.Mock;

namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// This is a test class for ClientProviderFactoryTest and is intended
    /// to contain all ClientProviderFactoryTest Unit Tests
    /// </summary>
    [TestClass]
    public class ClientProviderFactoryTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test to see if get returns null if bad enum value
        /// </summary>
        [TestMethod]
        public void Get_ReturnsNull_IfBadEnumValue()
        {
            ClientProviderType x = (ClientProviderType)9999;
            DbProviderFactory actual = ClientProviderFactory.Get(x);

            Assert.IsNull(actual);
        }


        /// <summary>
        /// A test for Get
        /// </summary>
        [TestMethod]
        public void GetSqlServerTest()
        {
            DbProviderFactory actual = ClientProviderFactory.Get(ClientProviderType.SqlServer);
            Assert.AreEqual(SqlClientFactory.Instance, actual);
        }

        /// <summary>
        /// A test for Get
        /// </summary>
        [TestMethod]
        public void GetOdbcServerTest()
        {
            DbProviderFactory actual = ClientProviderFactory.Get(ClientProviderType.Odbc);
            Assert.AreEqual(OdbcFactory.Instance, actual);
        }


        /// <summary>
        /// A test for Get
        /// </summary>
        [TestMethod]
        public void GetOleDbServerTest()
        {
            DbProviderFactory actual = ClientProviderFactory.Get(ClientProviderType.OleDb);
            Assert.AreEqual(OleDbFactory.Instance, actual);
        }


        /// <summary>
        /// A test for Get
        /// </summary>
        [TestMethod]
        public void GetMockServerTest()
        {
            DbProviderFactory actual = ClientProviderFactory.Get(ClientProviderType.Mock);
            Assert.AreEqual(MockClientFactory.Instance, actual);
        }
    }
}
