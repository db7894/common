using System.Data;
using System.Data.Common;

using SharedAssemblies.General.Database.Exceptions;

namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// Test class for a fake transaction that mimics the System.Data model.
    /// </summary>
    public class MockTransaction : DbTransaction
    {
        /// <summary>Reference to underlying connection.</summary>
        private readonly DbConnection _dbConnection;

        /// <summary>The default isolation level for the mock.</summary>
        private readonly IsolationLevel _isolationLevel;


		/// <summary>
		/// Test property to see if fake was committed
		/// </summary>
		public bool IsMockCommitted { get; private set; }


		/// <summary>
		/// Test property to see if fake was rolled back
		/// </summary>
		public bool IsMockRolledBack { get; private set; }


		/// <summary>
		/// Test property to see if the fake is still open
		/// </summary>
		public bool IsMockOpen
		{
			get { return !IsMockCommitted && !IsMockRolledBack; }
		}


		/// <summary>
		/// Specifies the <see cref="T:System.Data.IsolationLevel" /> for this transaction.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Data.IsolationLevel" /> for this transaction.
		/// </returns>
		public override IsolationLevel IsolationLevel
		{
			get { return _isolationLevel; }
		}


		/// <summary>
		/// Specifies the <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.
		/// </returns>
		protected override DbConnection DbConnection
		{
			get { return _dbConnection; }
		}


		/// <summary>
        /// Constructs a fake transaction tied to a fake connection
        /// </summary>
        /// <param name="connection">The connection the transaction is over.</param>
        /// <param name="level">The isolation level of the transaction.</param>
        public MockTransaction(DbConnection connection, IsolationLevel level)
        {
            _dbConnection = connection;
            _isolationLevel = level;
            IsMockCommitted = false;
            IsMockRolledBack = false;
        }


        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit()
        {
        	var foundConnection = MockClientFactory.GetMockConnectionResults(_dbConnection.ConnectionString);

			if (foundConnection != null && foundConnection.ShouldMockTransactionThrowOnCommit)
			{
				throw new DataAccessException("Test requested fake throw exception on Commit()");
			}

        	((MockConnection)Connection).CloseMockTransaction();
            IsMockCommitted = true;
        }


        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public override void Rollback()
        {
			var foundConnection = MockClientFactory.GetMockConnectionResults(_dbConnection.ConnectionString);

			if (foundConnection != null && foundConnection.ShouldMockTransactionThrowOnRollback)
            {
                throw new DataAccessException("Test requested fake throw exception on Rollback()");
            }

            ((MockConnection)Connection).CloseMockTransaction();
            IsMockRolledBack = true;
        }
    }
}
