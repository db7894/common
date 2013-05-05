using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using SharedAssemblies.General.Database.Exceptions;
using SharedAssemblies.General.Database.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Testing;

namespace SharedAssemblies.General.Database.UnitTests
{
	/// <summary>
    /// This is a test class for MockCommandTest and is intended
    /// to contain all MockCommandTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockCommandTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for UpdatedRowSource
        /// </summary>
        [TestMethod]
        public void UpdatedRowSourceTest()
        {
            MockCommand target = new MockCommand(); // TODO: Initialize to an appropriate value
            target.Connection = new MockConnection();
            target.Connection.Open();

            UpdateRowSource expected = UpdateRowSource.FirstReturnedRecord;

            target.UpdatedRowSource = expected;

            Assert.AreEqual(expected, target.UpdatedRowSource);
        }


        /// <summary>
        /// A test for IsMockPrepared
        /// </summary>
        [TestMethod]
        public void IsMockPreparedTest()
        {
            MockCommand target = new MockCommand();
            target.Connection = new MockConnection();
            target.Connection.Open();

            Assert.IsFalse(target.IsMockPrepared);

            target.Prepare();

            Assert.IsTrue(target.IsMockPrepared);
        }


        /// <summary>
        /// A test for IsMockDisposed
        /// </summary>
        [TestMethod]
        public void IsMockDisposedTest()
        {
            MockCommand target = new MockCommand();
            target.Connection = new MockConnection();
            target.Connection.Open();

            Assert.IsFalse(target.IsMockDisposed);

            target.Dispose();

            Assert.IsTrue(target.IsMockDisposed);
        }


        /// <summary>
        /// A test for DesignTimeVisible
        /// </summary>
        [TestMethod]
        public void DesignTimeVisibleTest()
        {
            MockCommand target = new MockCommand();

            Assert.IsTrue(target.DesignTimeVisible);

            target.DesignTimeVisible = false;

            Assert.IsFalse(target.DesignTimeVisible);
        }


        /// <summary>
        /// A test for CommandType
        /// </summary>
        [TestMethod]
        public void CommandTypeTest()
        {
            MockCommand target = new MockCommand();

            Assert.AreEqual(CommandType.Text, target.CommandType);

            target.CommandType = CommandType.StoredProcedure;

            Assert.AreEqual(CommandType.StoredProcedure, target.CommandType);
        }


        /// <summary>
        /// A test for CommandTimeout
        /// </summary>
        [TestMethod]
        public void CommandTimeoutTest()
        {
            MockCommand target = new MockCommand();

            Assert.AreEqual(30, target.CommandTimeout);

            target.CommandTimeout = 60;

            Assert.AreEqual(60, target.CommandTimeout);
        }


        /// <summary>
        /// A test for CommandText
        /// </summary>
        [TestMethod]
        public void CommandTextTest()
        {
            string expected = "blah";

            MockCommand target = new MockCommand();

            Assert.AreEqual(string.Empty, target.CommandText);

            target.CommandText = expected;

            Assert.AreEqual(expected, target.CommandText);
        }


        /// <summary>
        /// A test for ExecuteScalar
        /// </summary>
        [TestMethod]
        public void ExecuteScalarTest()
        {
            MockCommand target = new MockCommand();
            target.Connection = new MockConnection();
            target.Connection.Open();

            double expected = 3.1415927;

            MockClientFactory client = MockClientFactory.Instance;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ScalarResult = expected;
            client[target.Connection.ConnectionString][target.CommandText].Enqueue(commandResults);

            object actual = target.ExecuteScalar();

            Assert.AreEqual(expected, actual);

            client.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteNonQuery
        /// </summary>
        [TestMethod]
        public void ExecuteNonQueryTest()
        {
            MockCommand target = new MockCommand();
            target.Connection = new MockConnection();
            target.Connection.Open();

            int expected = 7;

            MockClientFactory client = MockClientFactory.Instance;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.RowsAffectedResult = expected;
            client[target.Connection.ConnectionString][target.CommandText].Enqueue(commandResults);

            object actual = target.ExecuteNonQuery();

            Assert.AreEqual(expected, actual);

            client.ResetMockResults();
        }


        /// <summary>
        /// Test execute reader method
        /// </summary>
        [TestMethod]
        public void ExecuteReader()
        {
            MockCommand target = new MockCommand();
            target.Connection = new MockConnection();
            target.Connection.Open();

            string expected = "Hello World";
            string columnOneName = "Test Column 1";

            DataTable[] mockData = new[] { new DataTable() };
            mockData[0].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            mockData[0].Rows.Add(new object[] { expected });

            MockClientFactory client = MockClientFactory.Instance;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ResultSet = mockData;
            client[target.Connection.ConnectionString][target.CommandText].Enqueue(commandResults);

            DbDataReader results = target.ExecuteReader();

            Assert.IsTrue(results.Read());
            Assert.AreEqual(results[0], expected);
            Assert.AreEqual(results[columnOneName], expected);
            Assert.IsFalse(results.Read());

            client.ResetMockResults();
        }


        /// <summary>
        /// A test for Dispose
        /// </summary>
        [TestMethod]
        public void DisposeTest()
        {
            MockCommand target = new MockCommand();

            Assert.IsFalse(target.IsMockDisposed);

            target.Dispose();

            Assert.IsTrue(target.IsMockDisposed);
        }


        /// <summary>
        /// A test for Cancel
        /// </summary>
        [TestMethod]
        public void CancelTest()
        {
            MockCommand target = new MockCommand();
			AssertEx.Throws<DataAccessException>(() => target.Cancel());

            target.Connection = new MockConnection();
			AssertEx.Throws<DataAccessException>(() => target.Cancel());

            // this should not cancel this time
            target.Connection.Open();
            target.Cancel();
        }


        /// <summary>
        /// A test for MockCommand Constructor
        /// </summary>
        [TestMethod]
        public void MockCommandConstructorTest()
        {
            MockCommand target = new MockCommand();

            Assert.IsNull(target.Connection);
        }


        /// <summary>
        /// A test for MockCommand Constructor
        /// </summary>
        [TestMethod]
        public void MockCommandWithConnectionConstructorTest()
        {
            DbConnection connection = new MockConnection();
            MockCommand target = new MockCommand(connection);

            Assert.AreSame(connection, target.Connection);
        }


        /// <summary>
        /// Test consistent construction
        /// </summary>
        [TestMethod]
        public void ConsistentConstructionTest()
        {
            MockCommand target = new MockCommand();
            SqlCommand actual = new SqlCommand();

            Assert.AreEqual(actual.CommandText, target.CommandText);
            Assert.AreEqual(actual.CommandTimeout, target.CommandTimeout);
            Assert.AreEqual(actual.CommandType, target.CommandType);
            Assert.AreEqual(actual.DesignTimeVisible, target.DesignTimeVisible);
            Assert.AreEqual(actual.UpdatedRowSource, target.UpdatedRowSource);
        }


        /// <summary>
        /// Unit test for method
        /// </summary>
        [TestMethod]
        public void CreateDbParameter_ReturnsMockParameter_OnCall()
        {
            MockCommand target = new MockCommand();
            DbParameter actual = target.CreateParameter();

            Assert.IsInstanceOfType(actual, typeof(MockParameter));
        }


        /// <summary>
        /// Test ExecuteDbDataReader throws exception on execute if specified
        /// </summary>
        [TestMethod]
        public void ExecuteDbDataReader_ThrowsException_OnExecute()
        {
            MockConnection connection = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");

            connection.Open();
            MockCommand target = new MockCommand(connection);
            target.CommandText = "Select blah from blah";

            // add to results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ShouldMockCommandThrowOnExecute = true;
            client[target.Connection.ConnectionString][target.CommandText].Enqueue(commandResults);

			AssertEx.Throws(() => target.ExecuteReader());
        }


        /// <summary>
        /// Test ExecuteScalar() throws exception if specified
        /// </summary>
        [TestMethod]
        public void ExecuteScalar_ThrowsException_OnExecute()
        {
            MockConnection connection = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");
            connection.Open();
            MockCommand target = new MockCommand(connection);
            target.CommandText = "Select blah from blah";

            // add to results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ShouldMockCommandThrowOnExecute = true;
            client[target.Connection.ConnectionString][target.CommandText].Enqueue(commandResults);

			AssertEx.Throws(() => target.ExecuteScalar());
        }


        /// <summary>
        /// Test ExecuteNonQuery throws exception on execute if specified
        /// </summary>
        [TestMethod]
        public void ExecuteNonQuery_ThrowsException_OnExecute()
        {
            MockConnection connection = new MockConnection("server=SqlDevStl4;database=Customers;" 
				+ "Trusted_Connection=True;Network Library=dbmssocn");
            connection.Open();
            MockCommand target = new MockCommand(connection);
            target.CommandText = "Select blah from blah";

            // add to results map in MockClientFactory
            MockClientFactory client = MockClientFactory.Instance;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ShouldMockCommandThrowOnExecute = true;
            client[target.Connection.ConnectionString][target.CommandText].Enqueue(commandResults);

			AssertEx.Throws(() => target.ExecuteNonQuery());
        }


        /// <summary>
        /// Test ParameterCollection initializes to empty when constructed
        /// </summary>
        [TestMethod]
        public void DbParameterCollection_InitializesToEmpty_OnConstruction()
        {
            MockCommand target = new MockCommand();

            Assert.IsNotNull(target.Parameters);
            Assert.AreEqual(0, target.Parameters.Count);
        }


        /// <summary>
        /// Test ParameterCollection add works correctly
        /// </summary>
        [TestMethod]
        public void DbParameterCollection_HasTwoCount_AfterAddTwoParameters()
        {
            MockCommand target = new MockCommand();

            target.Parameters.Add(target.CreateParameter());
            target.Parameters.Add(target.CreateParameter());

            Assert.AreEqual(2, target.Parameters.Count);
        }


		/// <summary>
		/// Checks whether the OutParameters property initializes to null.
		/// </summary>
		[TestMethod]
		public void OutParameters_DefaultToEmpty_OnConstruction()
		{
			var results = new MockCommandResults();

			Assert.IsNotNull(results.OutParameters);
			Assert.AreEqual(0, results.OutParameters.Count);
		}


		/// <summary>
		/// Checks whether the OutParameters property initializes to null.
		/// </summary>
		[TestMethod]
		public void OutParameters_Adds_OnInitialization()
		{
			var results = new MockCommandResults { OutParameters = { { "@retVal", 1 } } };

			Assert.IsNotNull(results.OutParameters);
			Assert.AreEqual(1, results.OutParameters.Count);
		}
	}
}
