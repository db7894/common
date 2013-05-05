using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

using SharedAssemblies.General.Database.Exceptions;

namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// Mocks a database connection for unit testing purposes
    /// </summary>
    public sealed class MockConnection : DbConnection
    {
        /// <summary>
        /// connection string specifying connection details
        /// </summary>
        private readonly MockConnectionStringBuilder _connectionStringBuilder 
			= new MockConnectionStringBuilder();

        /// <summary>
        /// The version of the server
        /// </summary>
        private readonly string _serverVersion = string.Empty;

        /// <summary>
        /// The state of the connection
        /// </summary>
        private ConnectionState _state = ConnectionState.Closed;


        /// <summary>
        /// Gets or sets the string used to open the connection.
        /// </summary>
        /// <returns>
        /// The connection string used to establish the initial connection. The exact contents of the 
        /// connection string depend on the specific data source for this connection. The default value is an empty string.
        /// </returns>
        public override string ConnectionString
        {
            get
            {
                var connectionString = _connectionStringBuilder.ConnectionString;
                return (connectionString != null) 
                    ? connectionString.ToLower() 
                    : string.Empty;
            }

            set 
            { 
                _connectionStringBuilder.ConnectionString = (value != null) 
                    ? value.ToLower() 
                    : string.Empty; 
            }
        }


        /// <summary>
        /// Gets the name of the current database after a connection is opened, or the database name specified in the connection string before the connection is opened.
        /// </summary>
        /// <returns>
        /// The name of the current database or the name of the database to be used after a connection is opened. The default value is an empty string.
        /// </returns>
        public override string Database
        {
            get
            {
                return _connectionStringBuilder.ContainsKey("Database")
                    ? _connectionStringBuilder["Database"].ToString().ToLower()
                    : string.Empty;
            }
        }


        /// <summary>
        /// Gets a string that describes the state of the connection.
        /// </summary>
        /// <returns>
        /// The state of the connection. The format of the string returned depends on the specific type of connection you are using.
        /// </returns>
        public override ConnectionState State
        {
            get { return _state; }
        }


        /// <summary>
        /// Gets the name of the database server to which to connect.
        /// </summary>
        /// <returns>
        /// The name of the database server to which to connect. The default value is an empty string.
        /// </returns>
        public override string DataSource
        {
            get
            {
                return _connectionStringBuilder.ContainsKey("Data Source")
                           ? _connectionStringBuilder["Data Source"].ToString().ToLower()
                           : string.Empty;
            }
        }


        /// <summary>
        /// Gets a string that represents the version of the server to which the object is connected.
        /// </summary>
        /// <returns>
        /// The version of the database. The format of the string returned depends on the specific type of connection you are using.
        /// </returns>
        public override string ServerVersion
        {
            get { return _serverVersion; }
        }


        /// <summary>
        /// Property to get/set the current mock transaction in progress
        /// </summary>
        public MockTransaction CurrentMockTransaction { get; set; }


        /// <summary>
        /// Test property to see if currently in a transaction
        /// </summary>
        public bool IsMockInTransaction { get; private set; }


        /// <summary>
        /// Test property to see if currently in a transaction
        /// </summary>
        public bool IsMockDisposed { get; private set; }


        /// <summary>
        /// Default constructor with no connection string
        /// </summary>
        public MockConnection()
            : this(string.Empty)
        {
        }


        /// <summary>
        /// Constructor with a specified connection string
        /// </summary>
        /// <param name="connectionString">Connection string for location of data source</param>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5005:NoEmptyCatchBlocks", Justification = "If mock is ill-formatted don't care.")]
		public MockConnection(string connectionString)
        {
            try
            {
                _connectionStringBuilder.ConnectionString = connectionString;
            }

            catch (Exception)
            {
                // suppress exception on connection string since it's just a mock...
            }
        }


        /// <summary>
        /// Closes the connection to the database. This is the preferred method of closing any open connection.
        /// </summary>
        public override void Close()
        {
            // if connection is closed and the transaction is still open roll it back
            if (CurrentMockTransaction != null && CurrentMockTransaction.IsMockOpen)
            {
                CurrentMockTransaction.Rollback();
            }
            CurrentMockTransaction = null;

            _state = ConnectionState.Closed;
        }


        /// <summary>
        /// Changes the current database for an open connection.
        /// </summary>
        /// <param name="databaseName">
        /// Specifies the name of the database for the connection to use.
        /// </param>
        public override void ChangeDatabase(string databaseName)
        {
            _connectionStringBuilder["Database"] = databaseName;
        }


        /// <summary>
        ///                     Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString" />.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public override void Open()
        {
            // should this connection throw an exception? check dictionary in MockClientFactory
            var foundConnection = MockClientFactory.GetMockConnectionResults(ConnectionString);

            if (foundConnection != null && foundConnection.ShouldMockConnectionThrowOnOpen)
            {
                throw new DataAccessException("Test user requested exception throw in Open()");
            }

            _state = ConnectionState.Open;
        }


        /// <summary>
        /// Close the fake transaction if currently open
        /// </summary>
        public void CloseMockTransaction()
        {
            CurrentMockTransaction = null;
            IsMockInTransaction = false;
        }


        /// <summary>
        /// Override to close the connection state when closed
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _state = ConnectionState.Closed;
                IsMockDisposed = true;
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        /// <param name="isolationLevel">
        /// Specifies the isolation level for the transaction.
        /// </param>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            // if one already open throw exception
            if (CurrentMockTransaction != null)
            {
                throw new DataAccessException("Cannot begin transaction when transaction already open.");
            }

            if (State != ConnectionState.Open)
            {
                throw new DataAccessException("Cannot begin transaction on closed connection.");
            }

            // get corresponding MockConnectionResults from MockClientFactory
            MockConnectionResults foundConnection;
            foundConnection = MockClientFactory.GetMockConnectionResults(ConnectionString);

            // create dummy transaction and return
            IsMockInTransaction = true;
        	return CurrentMockTransaction = new MockTransaction(this, isolationLevel);
        }


        /// <summary>
        /// Creates and returns a <see cref="T:System.Data.Common.DbCommand" /> object associated with the current connection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbCommand" /> object.
        /// </returns>
        protected override DbCommand CreateDbCommand()
        {
            // pass on mock properties
            return new MockCommand(this);
        }
    }
}
