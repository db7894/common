using System;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Database.Mock;
using SharedAssemblies.General.Testing;

namespace SharedAssemblies.General.Database.UnitTests
{
	/// <summary>
    /// This is a test class for DatabaseUtilityTest and is intended
    /// to contain all DatabaseUtilityTest Unit Tests
    /// </summary>
    [TestClass]
    public class DatabaseUtilityTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
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
        /// A test for ProviderFactory
        /// </summary>
        [TestMethod]
        public void ProviderFactoryTest()
        {
            string connection = "blah blah blah.";
            DbProviderFactory factory = MockClientFactory.Instance;
            DatabaseUtility target = new DatabaseUtility(factory, connection);

            // test that accessor gets same factory we passed in
            Assert.AreEqual(factory, target.ProviderFactory);
        }


        /// <summary>
        /// A test for connection
        /// </summary>
        [TestMethod]
        public void ConnectionTest()
        {
            string connection = "blah blah blah.";
            DbProviderFactory factory = MockClientFactory.Instance;
            DatabaseUtility target = new DatabaseUtility(factory, connection);

            // test that accessor gets same connection string we passed in
            Assert.AreEqual(connection, target.ConnectionString);
        }


        /// <summary>
        /// A test for connection
        /// </summary>
        [TestMethod]
        public void ToString_ReturnsCorrectName_WhenConnectionString()
        {
            string connection = "blah blah blah.";
            DbProviderFactory factory = MockClientFactory.Instance;
            DatabaseUtility target = new DatabaseUtility(factory, connection);

            // test that accessor gets same connection string we passed in
            Assert.AreEqual("SharedAssemblies.General.Database.DatabaseUtility(blah blah blah.)", 
                target.ToString());
        }


        /// <summary>
        /// A test for connection
        /// </summary>
        [TestMethod]
        public void ToString_ReturnsCorrectName_WhenNullConnectionString()
        {
            DbProviderFactory factory = MockClientFactory.Instance;
            DatabaseUtility target = new DatabaseUtility(factory, null);

            // test that accessor gets same connection string we passed in
            Assert.AreEqual("SharedAssemblies.General.Database.DatabaseUtility()",
                target.ToString());
        }


        /// <summary>
        /// A test for ExecuteScalar
        /// </summary>
        [TestMethod]
        public void ExecuteToScalarTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
            string command = "Select * from Customer";
            DatabaseUtility target = new DatabaseUtility(factory, connection);
            object expected = "test";
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ScalarResult = expected;
            factory[connection][command].Enqueue(commandResults);
            object results = target.ExecuteScalar(command, CommandType.Text);

            Assert.AreEqual(expected, results);

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteScalar
        /// </summary>
        [TestMethod]
        public void ExecuteToScalarWithParametersTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
            string command = "Select * from Customer";
            DatabaseUtility target = new DatabaseUtility(factory, connection);
            object expected = "test";
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ScalarResult = expected;
            factory[connection][command].Enqueue(commandResults);
            ParameterSet parms = target.CreateParameterSet();
            object results = target.ExecuteScalar(command, CommandType.Text, parms);

            Assert.AreEqual(expected, results);

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteNonQuery
        /// </summary>
        [TestMethod]
        public void ExecuteNonQueryTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
            string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;" 
				+ "Network Library=dbmssocn";
            string command = "Delete from Customer";
            DatabaseUtility target = new DatabaseUtility(factory, connection);
            int expected = 5;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.RowsAffectedResult = expected;
            factory[connection][command].Enqueue(commandResults);
            int results = target.ExecuteNonQuery(command, CommandType.Text);

            Assert.AreEqual(expected, results);

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteNonQuery
        /// </summary>
        [TestMethod]
        public void ExecuteNonQueryWithParametersTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
            string command = "Delete from Customer";
            DatabaseUtility target = new DatabaseUtility(factory, connection);
            int expected = 5;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.RowsAffectedResult = expected;
            factory[connection][command].Enqueue(commandResults);
            ParameterSet parms = target.CreateParameterSet();
            int results = target.ExecuteNonQuery(command, CommandType.Text);

            Assert.AreEqual(expected, results);

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteDataSet
        /// </summary>
        [TestMethod]
        public void ExecuteForDataSetTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
            string command = "Select * from Customer";
            DatabaseUtility target = new DatabaseUtility(factory, connection);

            string expectedRowObj = "Hello World";
            string columnOneName = "Test Column 1";

            DataTable[] mockData = new[] { new DataTable() };
            mockData[0].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            mockData[0].Rows.Add(new object[] { expectedRowObj });

            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ResultSet = mockData;
            factory[connection][command].Enqueue(commandResults);

            DataSet results = target.ExecuteDataSet(command, CommandType.Text);

            Assert.AreEqual(1, results.Tables.Count);
            Assert.AreEqual(1, results.Tables[0].Rows.Count);
            Assert.AreEqual(1, results.Tables[0].Columns.Count);
            Assert.AreEqual(columnOneName, results.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual(expectedRowObj, results.Tables[0].Rows[0][0]);

            results.Dispose();

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteDataSet
        /// </summary>
        [TestMethod]
        public void ExecuteForDataSetWithParametersTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
            string command = "Select * from Customer";
            DatabaseUtility target = new DatabaseUtility(factory, connection);

            string expectedRowObj = "Hello World";
            string columnOneName = "Test Column 1";

            DataTable[] mockData = new[] { new DataTable() };
            mockData[0].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            mockData[0].Rows.Add(new object[] { expectedRowObj });

            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ResultSet = mockData;
            factory[connection][command].Enqueue(commandResults);

            ParameterSet parms = target.CreateParameterSet();
            DataSet results = target.ExecuteDataSet(command, CommandType.Text, parms);

            Assert.AreEqual(1, results.Tables.Count);
            Assert.AreEqual(1, results.Tables[0].Rows.Count);
            Assert.AreEqual(1, results.Tables[0].Columns.Count);
            Assert.AreEqual(columnOneName, results.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual(expectedRowObj, results.Tables[0].Rows[0][0]);

            results.Dispose();

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteDataReader
        /// </summary>
        [TestMethod]
        public void ExecuteForDataReaderTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
            string command = "Select * from Customer";

            string expectedRowObj = "hello world";
            string columnOneName = "test column 1";

            DataTable[] mockData = new[] { new DataTable() };
            mockData[0].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            mockData[0].Rows.Add(new object[] { expectedRowObj });

            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ResultSet = mockData;
            factory[connection][command].Enqueue(commandResults);

            DatabaseUtility target = new DatabaseUtility(factory, connection);
            IDataReader results = target.ExecuteDataReader(command, CommandType.Text);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.IsClosed);
            Assert.IsTrue(results.Read());
            Assert.AreEqual(results[0], expectedRowObj);
            Assert.AreEqual(results[columnOneName], expectedRowObj);
            Assert.IsFalse(results.Read());

            results.Dispose();

            Assert.IsTrue(results.IsClosed);

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for ExecuteDataReader
        /// </summary>
        [TestMethod]
        public void ExecuteForDataReaderWithParametersTest()
        {
            MockClientFactory factory = MockClientFactory.Instance;
			string connection = "server=SqlDevStl4;database=Customers;Trusted_Connection=True;"
				+ "Network Library=dbmssocn";
            string command = "Select * from Customer";
            DatabaseUtility target = new DatabaseUtility(factory, connection);

            string expectedRowObj = "Hello World";
            string columnOneName = "Test Column 1";

            DataTable[] mockData = new[] { new DataTable() };
            mockData[0].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            mockData[0].Rows.Add(new object[] { expectedRowObj });

            MockCommandResults commandResults = new MockCommandResults();
            commandResults.ResultSet = mockData;
            factory[connection][command].Enqueue(commandResults);

            ParameterSet parms = target.CreateParameterSet();
            IDataReader results = target.ExecuteDataReader(command, CommandType.Text, parms);

            Assert.IsNotNull(results);
            Assert.IsFalse(results.IsClosed);
            Assert.IsTrue(results.Read());
            Assert.AreEqual(results[0], expectedRowObj);
            Assert.AreEqual(results[columnOneName], expectedRowObj);
            Assert.IsFalse(results.Read());

            results.Dispose();

            Assert.IsTrue(results.IsClosed);

            factory.ResetMockResults();
        }


        /// <summary>
        /// A test for DatabaseUtility Constructor
        /// </summary>
        [TestMethod]
        public void DatabaseUtilityConstructorWithTypeTest()
        {
            ClientProviderType type = ClientProviderType.SqlServer;
            string connection = "Someconnection";
            DatabaseUtility target = new DatabaseUtility(type, connection);

            Assert.AreEqual(SqlClientFactory.Instance, target.ProviderFactory);
            Assert.AreEqual(connection, target.ConnectionString);
        }


        /// <summary>
        /// A test for DatabaseUtility Constructor
        /// </summary>
        [TestMethod]
        public void DatabaseUtilityConstructorTest()
        {
            string connection = "Someconnection";
            DatabaseUtility target = new DatabaseUtility(SqlClientFactory.Instance, connection);

            Assert.AreEqual(SqlClientFactory.Instance, target.ProviderFactory);
            Assert.AreEqual(connection, target.ConnectionString);
        }


        /// <summary>
        /// A test for ParameterFactory
        /// </summary>
        [TestMethod]
        public void ParameterFactoryTest()
        {
            string connection = "Someconnection";
            DatabaseUtility target = new DatabaseUtility(SqlClientFactory.Instance, connection);

            Assert.AreEqual(SqlClientFactory.Instance, target.ParameterFactory.ProviderFactory);
        }


        /// <summary>
        /// A test for CreateParameterSet
        /// </summary>
        [TestMethod]
        public void CreateParameterSetTest()
        {
            string connection = "Someconnection";
            DatabaseUtility target = new DatabaseUtility(SqlClientFactory.Instance, connection);
            ParameterSet parameters = target.CreateParameterSet();

            Assert.AreEqual(SqlClientFactory.Instance, parameters.ProviderFactory);
            Assert.AreEqual(target.ProviderFactory, parameters.ProviderFactory);
            Assert.AreEqual(target.ParameterFactory.ProviderFactory, 
				parameters.ParameterFactory.ProviderFactory);
        }


        /// <summary>
        /// A test to make sure they can't pass the wrong type of parameter to the command objects
        /// </summary>
        [TestMethod]
        public void ExecuteNonQuery_ThrowsException_WhenPassedWrongParameterTypes()
        {
            MockClientFactory factory = MockClientFactory.Instance;
			string connection = "server=devsql001.bashwork.dev\\criclust03;database=Orders;"
				+ "Trusted_Connection=True;Network Library=dbmssocn";
            string command = "update someTable";
            DatabaseUtility target = new DatabaseUtility(factory, connection);
            int expected = 5;
            MockCommandResults commandResults = new MockCommandResults();
            commandResults.RowsAffectedResult = expected;
            factory[connection][command].Enqueue(commandResults);

			
            ParameterSet parameters = new ParameterSet(ClientProviderType.OleDb);
            parameters.Add("Parameter1", DbType.Int32, 42);

			// this is what we want, an oledb parameter on a non-oledb command is a cast exception
			AssertEx.Throws<InvalidCastException>(() =>
                target.ExecuteNonQuery(command, CommandType.Text, parameters));
            factory.ResetMockResults();
        }


		/// <summary>
		/// A test to make sure they can't pass the wrong type of parameter to the command objects
		/// </summary>
		[TestMethod]
		public void ExecuteNonQuery_SetsOutputParameter_WhenSelected()
		{
			string connection = "server=devsql001.bashwork.dev\\criclust03;database=Orders;"
				+ "Trusted_Connection=True;Network Library=dbmssocn";
			string command = "update someTable";

			MockClientFactory factory = MockClientFactory.Instance;

			DatabaseUtility target = new DatabaseUtility(factory, connection);

			MockCommandResults commandResults = new MockCommandResults();
			commandResults.RowsAffectedResult = 1;
			commandResults.OutParameters.Add("@retValue", 13);
			commandResults.OutParameters.Add("@outValue", "ERROR");
			factory[connection][command].Enqueue(commandResults);

			ParameterSet parameters = new ParameterSet(factory);
			parameters.Add("Parameter1", DbType.Int32, 42);
			parameters.Add("@retValue", DbType.Int32, ParameterDirection.ReturnValue);
			parameters.Add("@outValue", DbType.String, ParameterDirection.Output);

			target.ExecuteNonQuery(command, CommandType.StoredProcedure, parameters);

			Assert.AreEqual(13, parameters["@retValue"].Value);
			Assert.AreEqual("ERROR", parameters["@outValue"].Value);
		}


		/// <summary>
		/// A test to make sure they can't pass the wrong type of parameter to the command objects
		/// </summary>
		[TestMethod]
		public void ExecuteScalar_SetsOutputParameter_WhenSelected()
		{
			string connection = "server=devsql001.bashwork.dev\\criclust03;database=Orders;"
				+ "Trusted_Connection=True;Network Library=dbmssocn";
			string command = "update someTable";

			MockClientFactory factory = MockClientFactory.Instance;

			DatabaseUtility target = new DatabaseUtility(factory, connection);

			MockCommandResults commandResults = new MockCommandResults();
			commandResults.ScalarResult = 1;
			commandResults.OutParameters.Add("@retValue", 13);
			commandResults.OutParameters.Add("@outValue", "ERROR");
			factory[connection][command].Enqueue(commandResults);

			ParameterSet parameters = new ParameterSet(factory);
			parameters.Add("Parameter1", DbType.Int32, 42);
			parameters.Add("@retValue", DbType.Int32, ParameterDirection.ReturnValue);
			parameters.Add("@outValue", DbType.String, ParameterDirection.Output);

			target.ExecuteScalar(command, CommandType.StoredProcedure, parameters);

			Assert.AreEqual(13, parameters["@retValue"].Value);
			Assert.AreEqual("ERROR", parameters["@outValue"].Value);
		}


		/// <summary>
		/// A test to make sure they can't pass the wrong type of parameter to the command objects
		/// </summary>
		[TestMethod]
		public void ExecuteReader_SetsOutputParameter_WhenSelected()
		{
			string connection = "server=devsql001.bashwork.dev\\criclust03;database=Orders;"
				+ "Trusted_Connection=True;Network Library=dbmssocn";
			string command = "update someTable";

			MockClientFactory factory = MockClientFactory.Instance;

			DatabaseUtility target = new DatabaseUtility(factory, connection);

			MockCommandResults commandResults = new MockCommandResults();
			commandResults.ResultSet = new DataTable[0];
			commandResults.OutParameters.Add("@retValue", 13);
			commandResults.OutParameters.Add("@outValue", "ERROR");
			factory[connection][command].Enqueue(commandResults);

			ParameterSet parameters = new ParameterSet(factory);
			parameters.Add("Parameter1", DbType.Int32, 42);
			parameters.Add("@retValue", DbType.Int32, ParameterDirection.ReturnValue);
			parameters.Add("@outValue", DbType.String, ParameterDirection.Output);

			target.ExecuteDataReader(command, CommandType.StoredProcedure, parameters);

			Assert.AreEqual(13, parameters["@retValue"].Value);
			Assert.AreEqual("ERROR", parameters["@outValue"].Value);
		}
	}
}
