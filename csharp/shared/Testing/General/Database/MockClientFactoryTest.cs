using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Data;
using SharedAssemblies.General.Database.Mock;
using SharedAssemblies.General.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Database.UnitTests
{
	/// <summary>
	/// This is a test class for MockClientFactoryTest and is intended
	/// to contain all MockClientFactoryTest Unit Tests
	/// </summary>
	[TestClass]
	public class MockClientFactoryTest
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
        /// A test for ShouldMockConnectionThrowOnOpen
        /// </summary>
        [TestMethod]
        public void ShouldMockConnectionThrowOnOpenTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;
			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";

            // test bad open if property = true
            bool expectedValue = true;
            target[connection].ShouldMockConnectionThrowOnOpen = expectedValue;

            Assert.AreEqual(expectedValue, target[connection.ToLower()].ShouldMockConnectionThrowOnOpen);

            // test bad open of other element using same connection, but diff command
            target[connection].ShouldMockConnectionThrowOnOpen = expectedValue;

            Assert.AreEqual(expectedValue, target[connection.ToLower()].ShouldMockConnectionThrowOnOpen);

            target.ResetMockResults();

            // test good open if property = false
            expectedValue = false;
            target[connection].ShouldMockConnectionThrowOnOpen = expectedValue;

            Assert.AreEqual(expectedValue, target[connection.ToLower()].ShouldMockConnectionThrowOnOpen);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for ShouldMockCommandThrowOnExecute
        /// </summary>
        [TestMethod]
        public void ShouldMockCommandThrowOnExecuteTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
            string command = "Select * from Customers";

            // test bad open if property = true
            bool expectedValue = true;
            target[connection][command].Enqueue(new MockCommandResults());
            target[connection][command].Peek().ShouldMockCommandThrowOnExecute = expectedValue;

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);
            Assert.AreEqual(expectedValue, 
				target[connection.ToLower()][command].Peek().ShouldMockCommandThrowOnExecute);

            target.ResetMockResults();

            // test good open if property = false
            expectedValue = false;
            target[connection][command].Enqueue(new MockCommandResults());
            target[connection][command].Peek().ShouldMockCommandThrowOnExecute = expectedValue;

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);
            Assert.AreEqual(expectedValue, 
				target[connection.ToLower()][command].Peek().ShouldMockCommandThrowOnExecute);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for ShouldMockCommandThrowOnExecute
        /// </summary>
        [TestMethod]
        public void ShouldMockConnectionThrowOnCommitTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";

            // test bad open if property = true
            bool expectedValue = true;
            target[connection].ShouldMockTransactionThrowOnCommit = expectedValue;

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);
            Assert.AreEqual(expectedValue, target[connection.ToLower()].ShouldMockTransactionThrowOnCommit);

            target.ResetMockResults();

            // test good open if property = false
            expectedValue = false;
            target[connection].ShouldMockTransactionThrowOnCommit = expectedValue;

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);
            Assert.AreEqual(expectedValue, target[connection.ToLower()].ShouldMockTransactionThrowOnCommit);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for ShouldMockCommandThrowOnExecute
        /// </summary>
        [TestMethod]
        public void ShouldMockConnectionThrowOnRollbackTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";

            // test bad open if property = true
            bool expectedValue = true;
            target[connection].ShouldMockTransactionThrowOnRollback = expectedValue;

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);
            Assert.AreEqual(expectedValue, target[connection.ToLower()].ShouldMockTransactionThrowOnRollback);

            target.ResetMockResults();

            // test good open if property = false
            expectedValue = false;
            target[connection].ShouldMockTransactionThrowOnRollback = expectedValue;

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);
            Assert.AreEqual(expectedValue, target[connection.ToLower()].ShouldMockTransactionThrowOnRollback);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for EnqueueResults
        /// </summary>
        [TestMethod]
        public void InsertDbResultsInQueueTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
            string command = "Select * from Customers";

            // test inserting into the queue
            MockCommandResults expectedResult = new MockCommandResults();
            object expectedScalarResult = "blah blah blah";
            expectedResult.ScalarResult = expectedScalarResult;
            target[connection][command].Enqueue(expectedResult);

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);
            Assert.AreEqual(expectedResult, target[connection.ToLower()][command].Peek());

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for EnqueueResults (used multiple times)
        /// </summary>
        [TestMethod]
		[Ignore] // non-deterministic singleton (many threads hitting)
        public void InsertMultipleDbResultsInQueueTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
            string command = "Select id from Customers where name = joe schmo";
            int expectedNumOfQueueElmts = 0;
            int expectedNumOfDictElmts = 0;

            // enter 1st element in queue
            MockCommandResults expectedResult1 = new MockCommandResults();
            expectedResult1.ScalarResult = "blah";
            target[connection][command].Enqueue(expectedResult1);
            ++expectedNumOfQueueElmts;
            ++expectedNumOfDictElmts;

            Assert.AreEqual(expectedNumOfQueueElmts, target[connection.ToLower()][command].Count);
            Assert.AreEqual(expectedNumOfDictElmts, target.ConnectionResultsMap.Count);

            // enter 2nd element in queue...with same conn/cmd pair
            MockCommandResults expectedResult2 = new MockCommandResults();
            expectedResult2.ShouldMockCommandThrowOnExecute = true;
            target[connection][command].Enqueue(expectedResult2);
            ++expectedNumOfQueueElmts;

            Assert.AreEqual(expectedNumOfQueueElmts, target[connection.ToLower()][command].Count);

            Assert.AreEqual(expectedResult1, target[connection.ToLower()][command].Dequeue());
            Assert.AreEqual(expectedResult2, target[connection.ToLower()][command].Peek());

            // enter 3rd element in queue...with new conn/cmd pair
            command = "delete from Customers where name = joe";
            connection = "new connection";
            MockCommandResults expectedResult3 = new MockCommandResults();
            expectedResult3.RowsAffectedResult = 5;
            target[connection][command].Enqueue(expectedResult3);
            ++expectedNumOfDictElmts;

            Assert.AreEqual(expectedNumOfDictElmts, target.ConnectionResultsMap.Count);

            Assert.AreEqual(expectedResult3, target[connection.ToLower()][command].Peek());

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for the MockConnectionResults set property
        /// </summary>
        [TestMethod]
        public void InsertConnectionResultsMapViaSetPropertyTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            // build the ConnectionResultsMap outside of MockClientFactory
            var outsideConnectionResultsMap = new Dictionary<string, MockConnectionResults>();

            // 1st entry for dictionary
            string connection1 = "Test Connection 1";
            bool expectedTempThrowOnOpen1 = true;
            MockConnectionResults tempConnectionResults1 = new MockConnectionResults();
            tempConnectionResults1.ShouldMockConnectionThrowOnOpen = expectedTempThrowOnOpen1;
            outsideConnectionResultsMap.Add(connection1.ToLower(), tempConnectionResults1);

            // 2nd entry for dictionary...brand new connection string
            string connection2 = "Test Connection 2";
            var command = "Select * from Customers";
            object expectedScalar2 = "test scalar from 2nd elmt";
            MockConnectionResults tempConnectionResults2 = new MockConnectionResults();
            tempConnectionResults2[command] = new Queue<MockCommandResults>();
            MockCommandResults tempCommandResults2 = new MockCommandResults();
            tempCommandResults2.ScalarResult = expectedScalar2;
            tempConnectionResults2[command].Enqueue(tempCommandResults2);
            outsideConnectionResultsMap.Add(connection2.ToLower(), tempConnectionResults2);

            // 3rd entry for dictionary...same connection string & command text as 2nd entry
            bool expectedTempThrowOnExecute3 = true;
            MockCommandResults tempCommandResults3 = new MockCommandResults();
            tempCommandResults3.ShouldMockCommandThrowOnExecute = expectedTempThrowOnExecute3;
            outsideConnectionResultsMap[connection2.ToLower()][command].Enqueue(tempCommandResults3);

            // insert outsideConnectionResultsMap into MockClientFactory via sst property
            target.ConnectionResultsMap = outsideConnectionResultsMap;

            // verify results
            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection1.ToLower()]);
            Assert.AreEqual(target.ConnectionResultsMap[connection1.ToLower()], tempConnectionResults1);
            Assert.IsNotNull(target.ConnectionResultsMap[connection2.ToLower()]);
            Assert.AreEqual(target.ConnectionResultsMap[connection2.ToLower()], tempConnectionResults2);
            Assert.AreEqual(expectedTempThrowOnOpen1, 
				target.ConnectionResultsMap[connection1.ToLower()].ShouldMockConnectionThrowOnOpen);
            Assert.AreEqual(expectedScalar2, 
				target.ConnectionResultsMap[connection2.ToLower()][command].Dequeue().ScalarResult);
            Assert.AreEqual(expectedTempThrowOnExecute3, 
				target.ConnectionResultsMap[connection2.ToLower()][command].Dequeue()
					.ShouldMockCommandThrowOnExecute);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for RemoveDbResultsFromQueue
        /// </summary>
        [TestMethod]
        public void RemoveDbResultsFromQueueTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            string connection =
                "server=SqlDevStl4;database=Customers;Trusted_Connection=True;Network Library=dbmssocn";
            string command = "Select id from Customers where name = joe schmo";
            int expectedNumOfQueueElmts = 0;

            // enter 1st element in queue
            MockCommandResults expectedResult1 = new MockCommandResults();
            expectedResult1.ScalarResult = "blah blah blah";
            target[connection][command].Enqueue(expectedResult1);
            ++expectedNumOfQueueElmts;

            Assert.AreEqual(expectedNumOfQueueElmts,
                            target[connection.ToLower()][command].Count);

            // enter 2nd element in queue...with same conn/cmd pair
            MockCommandResults expectedResult2 = new MockCommandResults();
            expectedResult2.ShouldMockCommandThrowOnExecute = true;
            target[connection][command].Enqueue(expectedResult2);
            ++expectedNumOfQueueElmts;

            Assert.AreEqual(expectedNumOfQueueElmts,
                            target[connection.ToLower()][command].Count);

            // invoke DequeueResults
            target[connection][command].Dequeue();
            --expectedNumOfQueueElmts;

            Assert.AreEqual(expectedNumOfQueueElmts,
                            target[connection.ToLower()][command].Count);

            // ensure that the element left in the queue is the 2nd one that was entered
            Assert.AreEqual(expectedResult2,
                target[connection.ToLower()][command].Peek());

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for setting ShouldConnectionThrowOnOpen
        /// </summary>
        [TestMethod]
        public void ThrowOnOpenValueFromQueueTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            string connection =
                "server=SqlDevStl4;database=Customers;Trusted_Connection=True;Network Library=dbmssocn";
            string command = "Select ID from Customers where Name = Joe Schmo";
            bool expectedThrowOnOpen = true;
            bool expectedThrowOnExecute = false;
            bool expectedThrowOnCommit = false;
            bool expectedThrowOnRollback = false;

            // enter element in queue
            target[connection].ShouldMockConnectionThrowOnOpen = expectedThrowOnOpen;
            target[connection][command].Enqueue(new MockCommandResults());

            // invoke SetThrowValuesFromProvider indirectly by executing command
            DatabaseUtility db = new DatabaseUtility(target, connection);
			AssertEx.Throws(() => db.ExecuteNonQuery(command, CommandType.Text));

            Assert.AreEqual(expectedThrowOnOpen, 
				target[connection.ToLower()].ShouldMockConnectionThrowOnOpen);
            Assert.AreEqual(expectedThrowOnExecute, 
				target[connection.ToLower()][command].Peek().ShouldMockCommandThrowOnExecute);
            Assert.AreEqual(expectedThrowOnCommit, 
				target[connection.ToLower()].ShouldMockTransactionThrowOnCommit);
            Assert.AreEqual(expectedThrowOnRollback, 
				target[connection.ToLower()].ShouldMockTransactionThrowOnRollback);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for setting ShouldTransactionThrowOnExecute
        /// </summary>
        [TestMethod]
        public void ThrowOnExecuteValueFromQueueTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;

            string connection =
                "server=SqlDevStl4;database=Customers;Trusted_Connection=True;Network Library=dbmssocn";
            string command = "Select ID from Customers where Name = Joe Schmo";

            // enter element in queue
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ShouldMockCommandThrowOnExecute = true;
            target[connection][command].Enqueue(commandResults);

            // invoke SetThrowValuesFromProvider indirectly by executing command
            DatabaseUtility db = new DatabaseUtility(target, connection);
			AssertEx.Throws(() => db.ExecuteNonQuery(command, CommandType.Text));

            target.ResetMockResults();
        }


        /// <summary>
        /// reset the mock results test
        /// </summary>
        [TestMethod]
        public void ResetMockResultsTest()
        {
            // singleton instance is required
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";

            string command = "test cmd";
            MockClientFactory target = MockClientFactory.Instance;
            int expectedCount = 0;

            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ScalarResult = "test";
            target[connection][command].Enqueue(commandResults);

            // invoke ResetMockResults
            target.ResetMockResults();

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.AreEqual(expectedCount, target.ConnectionResultsMap.Count);
        	Assert.AreEqual(0, target.CommandsCreated.Count);
			Assert.AreEqual(0, target.ParametersCreated.Count);
			Assert.AreEqual(0, target.ConnectionsCreated.Count);
		}


        /// <summary>
        /// A test for ScalarResult
        /// </summary>
        [TestMethod]
        public void MockCommandResultsTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
			
            string command = "Select address from Customers where id = 123456789";

            // test the insertion of a scalar result into ConnectionResultsMap
            string expectedResult = "1 Mockingbird Lane, St. Louis, MO 63132";
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ScalarResult = expectedResult;
            target[connection][command].Enqueue(commandResults);

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);

            MockCommandResults insertedResult =
                target[connection.ToLower()][command].Peek();

            Assert.AreEqual(expectedResult, insertedResult.ScalarResult);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for MockCommandRowsAffectedResult
        /// </summary>
        [TestMethod]
        public void MockCommandRowsAffectedTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
            string command = "delete from Customers where firstname = bob";

            int expectedRows = 7;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.RowsAffectedResult = expectedRows;
            target[connection][command].Enqueue(commandResults);

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);

            MockCommandResults insertedResult =
                target[connection.ToLower()][command].Peek();

            Assert.AreEqual(expectedRows, insertedResult.RowsAffectedResult);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for MockCommandResultSets
        /// </summary>
        [TestMethod]
        public void MockCommandResultSetsTest()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
            string command = "Select * from Customers";

            DataTable[] expectedTable = new DataTable[5];
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ResultSet = expectedTable;
            target[connection][command].Enqueue(commandResults);

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.IsNotNull(target.ConnectionResultsMap[connection.ToLower()]);

            MockCommandResults insertedResult =
                target[connection.ToLower()][command].Peek();

            Assert.AreEqual(expectedTable, insertedResult.ResultSet);

            target.ResetMockResults();
        }


        /// <summary>
        /// A test for Instance
        /// </summary>
        [TestMethod]
        public void InstanceTest()
        {
            // singleton instance should be same every time
            MockClientFactory expected = MockClientFactory.Instance;
            MockClientFactory actual = MockClientFactory.Instance;

            Assert.AreSame(expected, actual);
        }


        /// <summary>
        /// A test for CanCreateDataSourceEnumerator
        /// </summary>
        [TestMethod]
        public void CanCreateDataSourceEnumeratorTest()
        {
            MockClientFactory target = MockClientFactory.Instance;

            // mock always returns false for this property
            Assert.IsFalse(target.CanCreateDataSourceEnumerator);
        }


        /// <summary>
        /// A test for CreatePermission
        /// </summary>
        [TestMethod]
        public void CreatePermissionTest()
        {
            MockClientFactory target = MockClientFactory.Instance;

            // mock always throws on create permission
			AssertEx.Throws<NotImplementedException>(() =>
                target.CreatePermission(PermissionState.None));
        }


        /// <summary>
        /// A test for CreateDataSourceEnumerator
        /// </summary>
        [TestMethod]
        public void CreateDataSourceEnumeratorTest()
        {
            MockClientFactory target = MockClientFactory.Instance;

            // mock always throws on create permission
			AssertEx.Throws<NotImplementedException>(() =>
                target.CreateDataSourceEnumerator());
        }


        /// <summary>
        /// A test for CreateCommandBuilder
        /// </summary>
        [TestMethod]
        public void CreateCommandBuilderTest()
        {
            MockClientFactory target = MockClientFactory.Instance;

            // mock always throws on create permission
			AssertEx.Throws<NotImplementedException>(() =>
                target.CreateCommandBuilder());
        }


        /// <summary>
        /// A test for MockClientFactory constructor
        /// </summary>
        [TestMethod]
        public void MockClientFactoryConstructorTest()
        {
            MockClientFactory target = MockClientFactory.Instance;
            int expectedCount = 0;

            Assert.IsNotNull(target.ConnectionResultsMap);
            Assert.AreEqual(expectedCount, target.ConnectionResultsMap.Count);
        }


        /// <summary>
        /// Connection results creates correctly if it doesn't already exist
        /// </summary>
        [TestMethod]
        public void ConnectionResults_DoesCreate_IfDoesNotExist()
        {
            MockClientFactory.Instance.ConnectionResults.ShouldMockConnectionThrowOnOpen = true;
            MockConnectionResults results = MockClientFactory.GetMockConnectionResults("NotHere");

            Assert.IsNotNull(results);
            Assert.IsTrue(results.ShouldMockConnectionThrowOnOpen);
        }


        /// <summary>
        /// Indexer called with null creates if it doesn't already exist
        /// </summary>
        [TestMethod]
        public void IndexerWithNull_DoesCreate_IfDoesNotExist()
        {
            MockClientFactory.Instance[null].ShouldMockConnectionThrowOnOpen = true;
            MockConnectionResults results = MockClientFactory.GetMockConnectionResults("NotHere");

            Assert.IsNotNull(results);
            Assert.IsTrue(results.ShouldMockConnectionThrowOnOpen);
        }


        /// <summary>
        /// GetMockConnectionResults() takes a default if does not exist
        /// </summary>
        [TestMethod]
        public void GetMockConnectionResults_TakesDefault_IfDoesNotExist()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";

            MockConnectionResults results = new MockConnectionResults();
            target.ConnectionResults = results;

            // check if gives us the default if connection/command does not exist
            Assert.AreSame(results, MockClientFactory.GetMockConnectionResults(connection));
        }

        
        /// <summary>
        /// GetMockConnectionResults() takes a default if doesn't exist and indexer was used
        /// </summary>
        [TestMethod]
        public void GetMockConnectionResults_TakesDefault_IfDoesNotExistAndIndexerUsed()
        {
            // singleton instance is required...test with nonsingleton will fail
            MockClientFactory target = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";

            MockConnectionResults results = new MockConnectionResults();
            target[null] = results;

            // check if gives us the default if connection/command does not exist
            Assert.AreSame(results, MockClientFactory.GetMockConnectionResults(connection));
        }


		/// <summary>
		/// Test that calls to CreateCommand() appropriately add the command created to the history.
		/// </summary>
		[TestMethod]
		public void CreateCommand_AddsCommandToHistory_OnCall()
		{
			const string expectedText = "SELECT * FROM ORDERS";
			var command = MockClientFactory.Instance.CreateCommand();
			command.CommandText = expectedText;

			Assert.AreEqual(1, MockClientFactory.Instance.CommandsCreated.Count);
			Assert.AreEqual(expectedText, MockClientFactory.Instance.CommandsCreated[0].CommandText, true);
		}


		/// <summary>
		/// Test that calls to CreateCommand() appropriately add the command created to the history.
		/// </summary>
		[TestMethod]
		public void CreateConnection_AddsConnectionToHistory_OnCall()
		{
			const string expected = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";

			var connection = MockClientFactory.Instance.CreateConnection();
			connection.ConnectionString = expected;

			Assert.AreEqual(1, MockClientFactory.Instance.ConnectionsCreated.Count);
			Assert.AreEqual(expected, MockClientFactory.Instance.ConnectionsCreated[0].ConnectionString, 
				true);
		}


		/// <summary>
		/// Test that calls to CreateCommand() appropriately add the command created to the history.
		/// </summary>
		[TestMethod]
		public void CreateParameter_AddsParameterToHistory_OnCall()
		{
			var parameter = MockClientFactory.Instance.CreateParameter();
			parameter.DbType = DbType.Int64;
			parameter.ParameterName = "@p1";
			parameter.Value = 32;

			Assert.AreEqual(1, MockClientFactory.Instance.ParametersCreated.Count);
			Assert.AreEqual(DbType.Int64, MockClientFactory.Instance.ParametersCreated[0].DbType);
			Assert.AreEqual("@p1", MockClientFactory.Instance.ParametersCreated[0].ParameterName);
			Assert.AreEqual(32, MockClientFactory.Instance.ParametersCreated[0].Value);
		}


		/// <summary>
		/// Test that calls to CreateCommand() appropriately add the command created to the history.
		/// </summary>
		[TestMethod]
		public void ExecutingCommand_AddsAllToHistory_OnExecute()
		{
			const string expectedConnection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";

			const string expectedCommand = "SELECT * FROM ORDERS";

			DatabaseUtility db = new DatabaseUtility(ClientProviderType.Mock, expectedConnection);

			var parameters = db.CreateParameterSet();
			parameters.Add("@p1");
			parameters.Add("@p2", DbType.Int64);
			parameters.Add("@p3", DbType.String, "Hi");

			db.ExecuteNonQuery(expectedCommand, CommandType.StoredProcedure, parameters);

			// check create counts
			Assert.AreEqual(1, MockClientFactory.Instance.ConnectionsCreated.Count);
			Assert.AreEqual(1, MockClientFactory.Instance.CommandsCreated.Count);
			Assert.AreEqual(3, MockClientFactory.Instance.ParametersCreated.Count);

			// check connection content
			Assert.AreEqual(expectedConnection, 
				MockClientFactory.Instance.ConnectionsCreated[0].ConnectionString, true);

			// check parameter content, should be same
			Assert.AreEqual("@p1", MockClientFactory.Instance.ParametersCreated[0].ParameterName);
			Assert.AreEqual("@p2", MockClientFactory.Instance.ParametersCreated[1].ParameterName);
			Assert.AreEqual(DbType.Int64, MockClientFactory.Instance.ParametersCreated[1].DbType);
			Assert.AreEqual("@p3", MockClientFactory.Instance.ParametersCreated[2].ParameterName);
			Assert.AreEqual(DbType.String, MockClientFactory.Instance.ParametersCreated[2].DbType);
			Assert.AreEqual("Hi", MockClientFactory.Instance.ParametersCreated[2].Value);

			// check command content text and connection and parameters should be same
			Assert.AreEqual(expectedCommand, MockClientFactory.Instance.CommandsCreated[0].CommandText, true);
			Assert.AreSame(MockClientFactory.Instance.ConnectionsCreated[0],
			               MockClientFactory.Instance.CommandsCreated[0].Connection);
			Assert.AreSame(MockClientFactory.Instance.CommandsCreated[0].Parameters[0],
				MockClientFactory.Instance.ParametersCreated[0]);
			Assert.AreSame(MockClientFactory.Instance.CommandsCreated[0].Parameters[1],
				MockClientFactory.Instance.ParametersCreated[1]);
			Assert.AreSame(MockClientFactory.Instance.CommandsCreated[0].Parameters[2],
				MockClientFactory.Instance.ParametersCreated[2]);
		}
	}
}
