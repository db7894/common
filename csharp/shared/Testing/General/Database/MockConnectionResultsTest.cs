using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Database.Mock;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// Tests for the mock connection class
    /// </summary>
    [TestClass]
    public class MockConnectionResultsTest
    {
        /// <summary>
        /// Property used internally by ms test
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup]
        public void MyTestCleanup()
        {
            MockClientFactory.Instance.ResetMockResults();
        }


        /// <summary>
        /// GetMockCommandResults returns null if none in queue
        /// </summary>
        [TestMethod] public void CommandResults_Collapse_IfNoneEnqueued()
        {
            Queue<MockCommandResults> results = MockClientFactory.GetMockCommandResults("NotHere", 
                "NotThere");    

            Assert.IsNull(results);
        }


        /// <summary>
        /// Command results create if do not exist
        /// </summary>
        [TestMethod]
        public void CommandResults_DoesCreate_IfDoesNotExist()
        {
            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(new MockCommandResults());
            Queue<MockCommandResults> results = MockClientFactory.GetMockCommandResults("NotHere",
                "NotThere");

            Assert.IsNotNull(results);
        }


        /// <summary>
        /// CommandResults create if does not exist using index
        /// </summary>
        [TestMethod]
        public void CommandResults_DoesCreate_IfDoesNotExistUsingIndexer()
        {
            MockClientFactory.Instance[null][null].Enqueue(new MockCommandResults());
            Queue<MockCommandResults> results = MockClientFactory.GetMockCommandResults("NotHere",
                "NotThere");

            Assert.IsNotNull(results);
        }


        /// <summary>
        /// GetMockCommandResults() takes a adefault if does not exist
        /// </summary>
        [TestMethod]
        public void GetMockCommandResults_TakesDefault_IfDoesNotExist()
        {
            // singleton instance is required...test with nonsingleton will fail
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
            string command = "select * from customer";

            MockCommandResults results = new MockCommandResults { RowsAffectedResult = 42 };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);

            Queue<MockCommandResults> actual = MockClientFactory.GetMockCommandResults(connection, command);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(results, actual.Peek());
        }

        
        /// <summary>
        /// GetMockCommandResults() takes a default if does not exist and using an indexer
        /// </summary>
        [TestMethod]
        public void GetMockCommandResults_TakesDefault_IfDoesNotExistUsingIndexer()
        {
            // singleton instance is required...test with nonsingleton will fail
			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
			string command = "select * from customer";

            MockCommandResults results = new MockCommandResults { RowsAffectedResult = 42 };

            MockClientFactory.Instance[null][null].Enqueue(results);

            Queue<MockCommandResults> actual = MockClientFactory.GetMockCommandResults(connection, command);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.AreSame(results, actual.Peek());
        }
    }
}
