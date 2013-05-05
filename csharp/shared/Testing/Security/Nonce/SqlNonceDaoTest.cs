using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Database;
using SharedAssemblies.General.Database.Mock;
using SharedAssemblies.General.Testing;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Security.Nonce;


namespace SharedAssemblies.Security.UnitTests.Nonce
{
	/// <summary>
	/// Test fixture for SqlNonceDao
	/// </summary>
	[TestClass]
	public class SqlNonceDaoTest
	{
		#region Member Fields

		/// <summary>
		/// Handle to the mocked database instance
		/// </summary>
		private static INonceDao _target;

		/// <summary>
		/// A dummy connection string used to initialize the dao
		/// </summary>
		private const string _connectionString = "server=devsql002.bashwork.dev\\clust008;"
			+ "database=SystemOutageManagement;"
			+ "Trusted_Connection=True;Network Library=dbmssocn";

		/// <summary>
		/// An example vendor identifier to use for identification
		/// </summary>
		private const string _vendorIdentifier = "65B80FBE-F91A-40CE-B3E5-00B3718D7718";

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		#endregion

		#region Test Setup

		/// <summary>
		/// Code run before after each test is executed
		/// </summary>
		[TestCleanup]
		public void MyTestCleanup()
		{
			MockClientFactory.Instance.ResetMockResults();
		}

		/// <summary>
		/// The class SetUp method
		/// </summary>
		[TestInitialize]
		public void MyTestSetup()
		{
			_target = new SqlNonceDao(ClientProviderType.Mock, _connectionString, _vendorIdentifier);
		}

		#endregion

		#region String Methods

		/// <summary>
		/// Tests a value store with a string data type
		/// </summary>
		[TestMethod]
		public void Store_WithGoodData_ReturnsGoodKey()
		{
			string key = Guid.NewGuid().ToString();
			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateKeyResult(key),
				RowsAffectedResult = 1,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_insert_login_session"]
				.Enqueue(database);

			var result = _target.Store("something");

			Assert.IsNotNull(result);
		}

		/// <summary>
		/// Tests a value store with a string data type
		/// </summary>
		[TestMethod]
		public void Store_WithBadData_ReturnsNullKey()
		{
			const string key = null;
			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateKeyResult(key),
				RowsAffectedResult = 0,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_insert_login_session"]
				.Enqueue(database);

			var result = _target.Store("nothing");

			Assert.AreEqual(key, result);
		}

		/// <summary>
		/// Tests a retrieve with a string data type
		/// </summary>
		[TestMethod]
		public void Retrieve_WithGoodKey_ReturnsGoodData()
		{
			const string value = "The result data";
			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateValueResult(value),
				RowsAffectedResult = 1,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_select_login_session"]
				.Enqueue(database);

			var result = _target.Retrieve("something");

			Assert.AreEqual(value, result);
		}

		/// <summary>
		/// Tests a retrieve with a string data type
		/// </summary>
		[TestMethod]
		public void Retrieve_WithBadKey_ReturnsNullData()
		{
			const string value = null;
			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateValueResult(value),
				RowsAffectedResult = 0,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_select_login_session"]
				.Enqueue(database);

			var result = _target.Retrieve("nothing");

			Assert.AreEqual(value, result);
		}

		#endregion

		#region Generic Methods

		/// <summary>
		/// Tests a value store with a generic type
		/// </summary>
		[TestMethod]
		public void StoreGeneric_WithGoodData_ReturnsGoodKey()
		{
			string key = Guid.NewGuid().ToString();
			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateKeyResult(key),
				RowsAffectedResult = 1,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_insert_login_session"]
				.Enqueue(database);

			var result = _target.Store(TestTypeFactory.CreateEntity(true));

			Assert.IsNotNull(result);
		}

		/// <summary>
		/// Tests a value store with a generic type
		/// </summary>
		[TestMethod]
		public void StoreGeneric_WithBadData_ReturnsNullKey()
		{
			const string key = null;
			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateKeyResult(key),
				RowsAffectedResult = 0,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_insert_login_session"]
				.Enqueue(database);

			var result = _target.Store(TestTypeFactory.CreateEntity(false));

			Assert.AreEqual(key, result);
		}

		/// <summary>
		/// Tests a retrieve with a generic data type
		/// </summary>
		[TestMethod]
		public void RetrieveGeneric_WithGoodKey_ReturnsGoodData()
		{
			var expected = TestTypeFactory.CreateEntity(true);

			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateValueResult(expected.ToXml()),
				RowsAffectedResult = 1,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_select_login_session"]
				.Enqueue(database);

			var result = _target.Retrieve<ExampleClass>("something");

			AssertEx.AreEqual(expected, result);
		}

		/// <summary>
		/// Tests a retrieve with a generic data type
		/// </summary>
		[TestMethod]
		public void RetrieveGeneric_WithBadKey_ReturnsNullData()
		{
			const string value = null;
			var database = new MockCommandResults
			{
				ResultSet = TestTypeFactory.CreateValueResult(value),
				RowsAffectedResult = 0,
			};
			MockClientFactory.Instance[_connectionString]["userinterface.usp_select_login_session"]
				.Enqueue(database);

			var result = _target.Retrieve<ExampleClass>("nothing");

			Assert.IsNull(result);
		}

		#endregion
	}
}
