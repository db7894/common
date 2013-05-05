using System.Data;
using System.Collections.Generic;
using SharedAssemblies.General.Database.Mock;
using SharedAssemblies.General.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Database;
using SharedAssemblies.Security.Authentication.DataAccess;
using SharedAssemblies.Security.Authentication.Entities;

namespace SharedAssemblies.Security.UnitTests.Authentication
{
    /// <summary>
    /// This is a test class for SqlAuthenticationDaoTest and is intended
    /// to contain all SqlAuthenticationDaoTest Unit Tests
    /// </summary>
    [TestClass]
    public class SqlAuthenticationDaoTest
    {
        /// <summary>
        /// front-end conenction string
        /// </summary>
        private const string _frontEndConnection =
			@"server=DEVSQL001.SCOTTRADE.DEV\\CLUST009;database=Bashworkr_Admin;"
			+ @"Trusted_Connection=True;Network Library=dbmssocn";

        /// <summary>
        /// customers connection string
        /// </summary>
        private const string _customersConnection =
			@"server=DEVSQL001.SCOTTRADE.DEV\\CLUST009;database=Bashworkr_Admin;"
			+ @"Trusted_Connection=True;Network Library=dbmssocn";

        /// <summary>
        /// A test for InsertSession
        /// </summary>
        [TestMethod]
        public void InsertSession_DBThrowsOnExecute_ThrowsException()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults
			{
				ShouldMockCommandThrowOnExecute = true,
			};

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
			AssertEx.Throws(() => target.InsertSession(session));
        }

        /// <summary>
        /// A test for InsertSession
        /// </summary>
        [TestMethod]
        public void InsertSession_ZeroRowsAffected_ReturnsFalse()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults
            {
                RowsAffectedResult = 0
            };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
            Assert.IsFalse(target.InsertSession(session));
        }

		/// <summary>
        /// A test for InsertSession
        /// </summary>
        [TestMethod]
        public void InsertSession_OneRowAffected_ReturnsTrue()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults
            {
                RowsAffectedResult = 1
            };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
            Assert.IsTrue(target.InsertSession(session));
        }

        /// <summary>
        /// A test for DeleteSession
        /// </summary>
        [TestMethod]
        public void DeleteSession_DBThrowsOnExecute_ThrowsDataException()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults
            {
                ShouldMockCommandThrowOnExecute = true
            };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
            AssertEx.Throws(() =>
				target.DeleteSession(session.VenderIdentifier, session.LoginIdentifier));
        }

        /// <summary>
        /// A test for DeleteSession
        /// </summary>
        [TestMethod]
        public void DeleteSession_ZeroRowsAffected_ReturnsFalse()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults
            {
                RowsAffectedResult = 0
            };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
            Assert.IsFalse(target.DeleteSession(session.VenderIdentifier, session.LoginIdentifier));
        }

        /// <summary>
        /// A test for DeleteSession
        /// </summary>
        [TestMethod]
        public void DeleteSession_OneRowAffected_ReturnsTrue()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults
            {
                RowsAffectedResult = 1
            };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
            Assert.IsTrue(target.DeleteSession(session.VenderIdentifier, session.LoginIdentifier));
        }

        /// <summary>
        /// A test for GetSessionById
        /// </summary>
        [TestMethod]
        public void GetSessionById_DBThrowsOnExecute_ThrowsException()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults
            {
                ShouldMockCommandThrowOnExecute = true
            };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
			AssertEx.Throws(() =>
				target.GetSessionById(session.VenderIdentifier, session.Type, session.LoginIdentifier));
        }

        /// <summary>
        /// A test for GetSessionById
        /// </summary>
        [TestMethod]
        public void GetSessionById_DBReturnsRecord_ReturnsSessionEntity()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type,
				_frontEndConnection, _customersConnection);
            SessionEntity session = new SessionEntity
            {
                VenderIdentifier = "111",
                Type = SessionType.Session,
                LoginIdentifier = "111",
                SessionValue = "1234",
                IpAddress = "1.1.1.1"
            };

            MockCommandResults results = new MockCommandResults();
            results.ResultSet = new[] 
            {
				new DataTable 
                {
					Columns = 
                    {
						new DataColumn("ip_address", typeof(string)),
                        new DataColumn("login_id", typeof(string)),
                        new DataColumn("session_type_id", typeof(int)),
                        new DataColumn("vendor_global_id", typeof(string)),
                        new DataColumn("session_string", typeof(string)),
						new DataColumn("login_flag", typeof(string)),
					},
					Rows = 
                    {
						new object[] { "first", "second", 1, "forth", "fifth", 1 }	
                    }
                }
			};

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
            SessionEntity actualSession = target.GetSessionById(session.VenderIdentifier,
				session.Type, session.LoginIdentifier);

            Assert.IsNotNull(actualSession);
            Assert.AreEqual("first", actualSession.IpAddress);
            Assert.AreEqual("second", actualSession.LoginIdentifier);
            Assert.AreEqual(SessionType.Session, actualSession.Type);
            Assert.AreEqual("forth", actualSession.VenderIdentifier);
            Assert.AreEqual("fifth", actualSession.SessionValue);
        }

        /// <summary>
        /// A test for GetLinkedAccounts
        /// </summary>
        [TestMethod]
        public void GetLinkedAccounts_DBThrowsOnExecute_ThrowsException()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type, _frontEndConnection, 
				_customersConnection);
 
            MockCommandResults results = new MockCommandResults
            {
                ShouldMockCommandThrowOnExecute = true
            };

            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);
			AssertEx.Throws(() => target.GetLinkedAccounts("11111285"));
        }

        /// <summary>
        /// A test for GetLinkedAccounts
        /// </summary>
        [TestMethod]
        public void GetLinkedAccounts_DBReturnsRecord_ReturnsLinkedAccountsList()
        {
            ClientProviderType type = ClientProviderType.Mock;
            SqlAuthenticationDao target = new SqlAuthenticationDao(type, _frontEndConnection, 
				_customersConnection);


            MockCommandResults results = new MockCommandResults();
            results.ResultSet = new[]
                                    {
                                        new DataTable
                                            {
                                                Columns =
                                                    {
                                                        new DataColumn("account_child", typeof(string)),
                                                    },
                                                Rows =
                                                    {
                                                        new object[] { "first" },
                                                        new object[] { "second" },
                                                        new object[] { "third" }
                                                    }
                                            },
                                    };
            MockClientFactory.Instance.ConnectionResults.CommandResults.Enqueue(results);

			var expected = new List<string> { "first", "second", "third" };
            var actual = new List<string>(target.GetLinkedAccounts("test"));
			AssertEx.AreEqual(expected, actual);
        }
    }
}
