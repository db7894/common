using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using SharedAssemblies.General.Database.Mock;
using SharedAssemblies.General.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
	/// <summary>
    /// This is a test class for MockConnectionTest and is intended
    /// to contain all MockConnectionTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockConnectionTest
    {
		/// <summary>
		/// runs after each test has run
		/// </summary>
		[TestCleanup]
		public void MyTestCleanup()
		{
			MockClientFactory.Instance.ResetMockResults();
		}
		
		
		/// <summary>
        /// A test for State
        /// </summary>
        [TestMethod]
        public void StateTest()
        {
			MockConnection target = new MockConnection("server=SqlDevStl4;database=Customers;"
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            Assert.AreEqual(ConnectionState.Closed, target.State);

            target.Open();

            Assert.AreEqual(ConnectionState.Open, target.State);

            target.Dispose();

            Assert.AreEqual(ConnectionState.Closed, target.State);
        }


        /// <summary>
        /// A test for ServerVersion
        /// </summary>
        [TestMethod]
        public void ServerVersionTest()
        {
            MockConnection target = new MockConnection();
            string actual = target.ServerVersion;

            Assert.AreEqual(string.Empty, actual);
        }


        /// <summary>
        /// A test for IsMockInTransaction
        /// </summary>
        [TestMethod]
        public void IsMockInTransactionTest()
        {
            MockConnection target = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            // add connection to connection results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockConnectionResults connectionResults = new MockConnectionResults();
            client.ConnectionResultsMap.Add(target.ConnectionString, connectionResults);

            target.Open();

            Assert.IsFalse(target.IsMockInTransaction);

            DbTransaction trans = target.BeginTransaction();

            Assert.IsTrue(target.IsMockInTransaction);

            trans.Commit();

            Assert.IsFalse(target.IsMockInTransaction);

            client.ResetMockResults();
        }


        /// <summary>
        /// A test for DataSource
        /// </summary>
        [TestMethod]
        public void DataSourceTest()
        {
            string expectedDataSource = "SQLQA001";
            string expectedDatabase = "Orders";

            MockConnectionStringBuilder builder = new MockConnectionStringBuilder();
            builder["Data Source"] = expectedDataSource;
            builder["Database"] = expectedDatabase;

            MockConnection target = new MockConnection(builder.ConnectionString);
            Assert.AreEqual(expectedDataSource, target.DataSource, true);
        }


        /// <summary>
        /// A test for Database
        /// </summary>
        [TestMethod]
        public void DatabaseTest()
        {
            string expectedDataSource = "SQLQA001";
            string expectedDatabase = "Orders";

            MockConnectionStringBuilder builder = new MockConnectionStringBuilder();
            builder["DataSource"] = expectedDataSource;
            builder["Database"] = expectedDatabase;

            MockConnection target = new MockConnection(builder.ConnectionString);

            Assert.AreEqual(expectedDatabase, target.Database, true);
        }


        /// <summary>
        /// A test for CurrentMockTransaction
        /// </summary>
        [TestMethod]
        public void CurrentMockTransactionTest()
        {
			MockConnection target = new MockConnection("server=SqlDevStl4;database=Customers;"
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            // add connection to connection results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockConnectionResults connectionResults = new MockConnectionResults();
            client.ConnectionResultsMap.Add(target.ConnectionString, connectionResults);

            target.Open();

            Assert.IsNull(target.CurrentMockTransaction);

            DbTransaction trans = target.BeginTransaction();

            Assert.IsNotNull(target.CurrentMockTransaction);

            trans.Commit();

            Assert.IsNull(target.CurrentMockTransaction);

            client.ResetMockResults();
        }


        /// <summary>
        /// A test for ConnectionString
        /// </summary>
        [TestMethod]
        public void ConnectionStringTest()
        {
            string expected = "User Id=User001;Password=Password001;Initial Catalog=Server001;" 
				+ "Data Source=Orders";

            MockConnection target = new MockConnection(expected);

            // will accept case variations
            Assert.AreEqual(expected, target.ConnectionString, true);
        }


        /// <summary>
        /// A test for Open
        /// </summary>
        [TestMethod]
        public void OpenTest()
        {
			MockConnection target = new MockConnection("server=SqlDevStl4;database=Customers;"
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            target.Open();

            Assert.AreEqual(ConnectionState.Open, target.State);
        }


        /// <summary>
        /// A test for Dispose
        /// </summary>
        [TestMethod]
        public void DisposeTest()
        {
			MockConnection target = new MockConnection("server=SqlDevStl4;database=Customers;"
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            target.Open();
            target.Dispose();

            Assert.IsTrue(target.IsMockDisposed);
            Assert.AreEqual(ConnectionState.Closed, target.State);
        }


        /// <summary>
        /// A test for CloseMockTransaction
        /// </summary>
        [TestMethod]
        public void CloseMockTransactionTest()
        {
			MockConnection target = new MockConnection("server=SqlDevStl4;database=Customers;"
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            // add connection to connection results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockConnectionResults connectionResults = new MockConnectionResults();
            client.ConnectionResultsMap.Add(target.ConnectionString, connectionResults);

            target.Open();

            target.BeginTransaction();

            Assert.IsTrue(target.IsMockInTransaction);

            target.CloseMockTransaction();

            Assert.IsFalse(target.IsMockInTransaction);
            Assert.IsNull(target.CurrentMockTransaction);

            client.ResetMockResults();
        }


        /// <summary>
        /// A test for Close
        /// </summary>
        [TestMethod]
        public void CloseTest()
        {
			MockConnection target = new MockConnection("server=SqlDevStl4;database=Customers;"
				+ "Trusted_Connection=True;Network Library=dbmssocn");

			target.Open();
            target.Close();
            Assert.AreEqual(ConnectionState.Closed, target.State);

            // verify closed does NOT mean disposed
            Assert.IsFalse(target.IsMockDisposed);
        }


        /// <summary>
        /// A test for ChangeDatabase
        /// </summary>
        [TestMethod]
        public void ChangeDatabaseTest()
        {
            string expectedDataSource = "SQLQA001";
            string originalDatabase = "Orders";
            string expectedDatabase = "Customers";

            MockConnectionStringBuilder builder = new MockConnectionStringBuilder();
            builder["DataSource"] = expectedDataSource;
            builder["Database"] = originalDatabase;

            MockConnection target = new MockConnection(builder.ConnectionString);
            target.ChangeDatabase(expectedDatabase);

            Assert.AreEqual(expectedDatabase, target.Database, true);
        }


        /// <summary>
        /// A test for MockConnection Constructor
        /// </summary>
        [TestMethod]
        public void MockConnectionConstructorTest()
        {
            MockConnection target = new MockConnection();

            Assert.AreEqual(string.Empty, target.ConnectionString);
            Assert.AreEqual(ConnectionState.Closed, target.State);
            Assert.IsFalse(target.IsMockInTransaction);
            Assert.IsFalse(target.IsMockDisposed);
            Assert.IsNull(target.CurrentMockTransaction);
            Assert.AreEqual(string.Empty, target.Database);
            Assert.AreEqual(string.Empty, target.DataSource);
        }


        /// <summary>
        /// A test for MockConnection Constructor
        /// </summary>
        [TestMethod]
        public void MockConnectionWithConnectionStringConstructorTest()
        {
            string expectedDataSource = "SQLQA001";
            string expectedDatabase = "Customers";

            MockConnectionStringBuilder builder = new MockConnectionStringBuilder();
            builder["Data Source"] = expectedDataSource;
            builder["Database"] = expectedDatabase;
            MockConnection target = new MockConnection();
            target.ConnectionString = builder.ConnectionString;

            Assert.AreEqual(builder.ConnectionString, target.ConnectionString, true);
            Assert.AreEqual(ConnectionState.Closed, target.State);
            Assert.IsFalse(target.IsMockInTransaction);
            Assert.IsFalse(target.IsMockDisposed);
            Assert.IsNull(target.CurrentMockTransaction);
            Assert.AreEqual(expectedDatabase, target.Database, true);
            Assert.AreEqual(expectedDataSource, target.DataSource, true);
        }


        /// <summary>
        /// A test for MockConnection Constructor
        /// </summary>
        [TestMethod]
        public void VerifyConsistentConstructionTest()
        {
            string expectedDataSource = "SQLQA001";
            string expectedDatabase = "Customers";

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder["Data Source"] = expectedDataSource;
            builder["Database"] = expectedDatabase;

            SqlConnection target = new SqlConnection
            {
                ConnectionString = builder.ConnectionString
            };

            MockConnection actual = new MockConnection
            {
                ConnectionString = builder.ConnectionString
            };

            Assert.AreEqual(target.ConnectionString, actual.ConnectionString, true);
            Assert.AreEqual(target.State, actual.State);
            Assert.AreEqual(target.Database, actual.Database, true);
            Assert.AreEqual(target.DataSource, actual.DataSource, true);
        }


        /// <summary>
        /// Tests that BeginDbTransaction() throws if connection not open
        /// </summary>
        [TestMethod]
        public void BeginDbTransaction_Throws_IfNotOpen()
        {
            MockConnection con = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");
			AssertEx.Throws(() => con.BeginTransaction());
        }


        /// <summary>
        /// Test BeginDbTransaction() throws if already in a transaction
        /// </summary>
        [TestMethod]
        public void BeginDbTransaction_Throws_IfAlreadyIn()
        {
            MockConnection con = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            // add connection to connection results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockConnectionResults connectionResults = new MockConnectionResults();
            client.ConnectionResultsMap.Add(con.ConnectionString, connectionResults);

            con.Open();

            DbTransaction tx1 = con.BeginTransaction();
			AssertEx.Throws(() => con.BeginTransaction());

            client.ResetMockResults();
        }


        /// <summary>
        /// Test that Open() throws if ShouldThrowOnOpen is true
        /// </summary>
        [TestMethod]
        public void Open_Throws_IfShouldThrowOnOpen()
        {
            MockConnection con = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");

        	MockClientFactory.Instance[con.ConnectionString].ShouldMockConnectionThrowOnOpen = true;

			AssertEx.Throws(() => con.Open());
        }


        /// <summary>
        /// Test Close() rolls back transaction if transaction open
        /// </summary>
        [TestMethod]
        public void Close_RollsbackTransaction_IfOpen()
        {
            MockConnection con = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            // add connection to connection results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockConnectionResults connectionResults = new MockConnectionResults();
            client.ConnectionResultsMap.Add(con.ConnectionString.ToLower(), connectionResults);

            con.Open();

            MockTransaction dbTransaction = con.BeginTransaction() as MockTransaction;

            Assert.IsNotNull(dbTransaction);
            Assert.IsTrue(dbTransaction.IsMockOpen);
            Assert.IsFalse(dbTransaction.IsMockRolledBack);

            con.Close();

            Assert.IsFalse(dbTransaction.IsMockOpen);
            Assert.IsTrue(dbTransaction.IsMockRolledBack);

            client.ResetMockResults();
        }
    }
}
