using System.Data;
using SharedAssemblies.General.Database.Exceptions;
using SharedAssemblies.General.Database.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Testing;

namespace SharedAssemblies.General.Database.UnitTests
{
	/// <summary>
    /// This is a test class for MockTransactionTest and is intended
    /// to contain all MockTransactionTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockTransactionTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


		/// <summary>
		/// runs after each test has run
		/// </summary>
		[TestCleanup]
		public void Cleanup()
		{
			MockClientFactory.Instance.ResetMockResults();
		}


        /// <summary>
        /// A test for IsolationLevel
        /// </summary>
        [TestMethod]
        public void IsolationLevelTest()
        {
            MockTransaction target = new MockTransaction(new MockConnection(), IsolationLevel.Serializable);

            Assert.AreEqual(IsolationLevel.Serializable, target.IsolationLevel);
        }


        /// <summary>
        /// A test for IsMockRolledBack
        /// </summary>
        [TestMethod]
        public void IsMockRolledBackTest()
        {
            MockTransaction target = new MockTransaction(new MockConnection(), IsolationLevel.Serializable);

            Assert.IsFalse(target.IsMockRolledBack);
            target.Rollback();
            Assert.IsTrue(target.IsMockRolledBack);
        }


        /// <summary>
        /// A test for IsMockOpen
        /// </summary>
        [TestMethod]
        public void IsMockOpenTest()
        {
            MockTransaction target = new MockTransaction(new MockConnection(), IsolationLevel.Serializable);

            Assert.IsTrue(target.IsMockOpen);
            target.Rollback();
            Assert.IsFalse(target.IsMockOpen);
        }


        /// <summary>
        /// A test for IsMockCommitted
        /// </summary>
        [TestMethod]
        public void IsMockCommittedTest()
        {
            MockTransaction target = new MockTransaction(new MockConnection(), IsolationLevel.Serializable);

            Assert.IsFalse(target.IsMockCommitted);
            target.Commit();
            Assert.IsTrue(target.IsMockCommitted);
        }


        /// <summary>
        /// A test for Rollback
        /// </summary>
        [TestMethod]
        public void RollbackTest()
        {
            MockConnection con = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            // add connection to connection results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockConnectionResults connectionResults = new MockConnectionResults();
            client.ConnectionResultsMap.Add(con.ConnectionString, connectionResults);

            con.Open();
            MockTransaction target = con.BeginTransaction() as MockTransaction;

        	MockClientFactory.Instance[con.ConnectionString].ShouldMockTransactionThrowOnRollback = true;
			AssertEx.Throws<DataAccessException>(() => target.Rollback());

			MockClientFactory.Instance[con.ConnectionString].ShouldMockTransactionThrowOnRollback = false;
			target.Rollback();

            Assert.IsTrue(target.IsMockRolledBack);
            Assert.IsFalse(target.IsMockCommitted);
            Assert.IsFalse(target.IsMockOpen);

            client.ResetMockResults();
        }


        /// <summary>
        /// A test for Commit
        /// </summary>
        [TestMethod]
        public void CommitTest()
        {
            MockConnection con = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");
            con.Open();
            MockTransaction target = con.BeginTransaction() as MockTransaction;

			MockClientFactory.Instance[con.ConnectionString].ShouldMockTransactionThrowOnCommit = true;
			AssertEx.Throws<DataAccessException>(() => target.Commit());

			MockClientFactory.Instance[con.ConnectionString].ShouldMockTransactionThrowOnCommit = false;
			target.Commit();

            Assert.IsFalse(target.IsMockRolledBack);
            Assert.IsTrue(target.IsMockCommitted);
            Assert.IsFalse(target.IsMockOpen);
        }


        /// <summary>
        /// A test for MockTransaction Constructor
        /// </summary>
        [TestMethod]
        public void MockTransactionConstructorTest()
        {
            MockTransaction target = new MockTransaction(new MockConnection(), IsolationLevel.Serializable);

            Assert.IsFalse(target.IsMockRolledBack);
            Assert.IsFalse(target.IsMockCommitted);
            Assert.IsTrue(target.IsMockOpen);
            Assert.AreEqual(IsolationLevel.Serializable, target.IsolationLevel);
        }
    }
}
