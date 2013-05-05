using System.Data;
using System.Data.Common;

using SharedAssemblies.General.Database.Exceptions;

namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// A fake database command that mimics the typical System.Data model
    /// </summary>
    public sealed class MockCommand : DbCommand
    {
        /// <summary>
        /// the parameters for this command
        /// </summary>
        private readonly MockParameterCollection _parameters = new MockParameterCollection();


        /// <summary>
        /// Gets or sets the text command to run against the data source.
        /// </summary>
        /// <returns>
        /// The text command to execute. The default value is an empty string ("").
        /// </returns>
        public override string CommandText { get; set; }


        /// <summary>
        /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
        /// </summary>
        /// <returns>
        /// The time in seconds to wait for the command to execute.
        /// </returns>
        public override int CommandTimeout { get; set; }


        /// <summary>
        /// Indicates or specifies how the <see cref="P:System.Data.Common.DbCommand.CommandText" /> property is interpreted.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Data.CommandType" /> values. The default is Text.
        /// </returns>
        public override CommandType CommandType { get; set; }


        /// <summary>
        /// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow" /> when used by the Update method of a <see cref="T:System.Data.Common.DbDataAdapter" />.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Data.UpdateRowSource" /> values. The default is Both unless the command is automatically generated. Then the default is None.
        /// </returns>
        public override UpdateRowSource UpdatedRowSource { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the command object should be visible in a customized interface control.
        /// </summary>
        /// <returns>
        /// true, if the command object should be visible in a control; otherwise false. The default is true.
        /// </returns>
        public override bool DesignTimeVisible { get; set; }


        /// <summary>
        /// Mock property to test if command was prepared or not
        /// </summary>
        public bool IsMockPrepared { get; private set; }


        /// <summary>
        /// Mock property to test if command was disposed or not
        /// </summary>
        public bool IsMockDisposed { get; private set; }


        /// <summary>
        /// Gets or sets the <see cref="T:System.Data.Common.DbConnection" /> used by this <see cref="T:System.Data.Common.DbCommand" />.
        /// </summary>
        /// <returns>
        /// The connection to the data source.
        /// </returns>
        protected override DbConnection DbConnection { get; set; }


        /// <summary>
        /// Gets the collection of DbParameter objects.
        /// </summary>
        /// <returns>
        /// The parameters of the SQL statement or stored procedure.
        /// </returns>
        protected override DbParameterCollection DbParameterCollection
        {
            get { return _parameters; }
        }


        /// <summary>
        /// Gets or sets the <see cref="P:System.Data.Common.DbCommand.DbTransaction" /> within which this <see cref="T:System.Data.Common.DbCommand" /> object executes.
        /// </summary>
        /// <returns>
        /// The transaction within which a Command object of a .NET Framework data provider executes. The default value is a null reference (Nothing in Visual Basic).
        /// </returns>
        protected override DbTransaction DbTransaction { get; set; }


        /// <summary>
        /// Create a new MockCommand object tied to no connection
        /// </summary>
        public MockCommand()
            : this(null)
        {
        }


        /// <summary>
        /// Construct a fake command for unit testing
        /// </summary>
        /// <param name="connection">The connection to attach to</param>
        public MockCommand(DbConnection connection)
        {
            Connection = connection;
            CommandText = string.Empty;
            CommandTimeout = 30;
            CommandType = CommandType.Text;
            UpdatedRowSource = UpdateRowSource.Both;
            DesignTimeVisible = true;
        }


        /// <summary>
        /// Creates a prepared (or compiled) version of the command on the data source.
        /// </summary>
        public override void Prepare()
        {
            ThrowIfInvalid("Prepare()");

            IsMockPrepared = true;
        }



        /// <summary>
        /// Attempts to cancels the execution of a DbCommand
        /// </summary>
        public override void Cancel()
        {
            ThrowIfInvalid("Cancel()");
        }


        /// <summary>
        /// Executes an SQL statement against the Connection object of a .NET 
        /// Framework data provider, and returns the number of rows affected.
        /// </summary>
        /// <returns>
        /// The number of rows affected (-1 by default).
        /// </returns>
        public override int ExecuteNonQuery()
        {
            // check to see if results exist for this connection/command pair in MockClientFactory
            int returnResult = MockCommandResults.DefaultRowsAffectedResult;

            ThrowIfInvalid("ExecuteNonQuery");

            var found = MockClientFactory.GetMockCommandResults(DbConnection.ConnectionString, 
				CommandText);

            if (found != null)
            {
                // remove first result from the queue
                MockCommandResults resultFromQueue = found.Dequeue();

                returnResult = resultFromQueue.RowsAffectedResult;
                if (resultFromQueue.ShouldMockCommandThrowOnExecute)
                {
                    throw new DataAccessException("Test requested mock throw exception on ExecuteNonQuery.");
                }

				// set any possible output parameter values from the mock
				SetOutputParameters(resultFromQueue);
            }

            return returnResult;
        }


        /// <summary>
        /// Executes the query, and returns the first column of the first row in the 
        /// resultset returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <returns>
        /// The first column of the first row in the resultset.
        /// </returns>
        public override object ExecuteScalar()
        {
            object returnResult = null;

            ThrowIfInvalid("ExecuteScalar()");

            var found = MockClientFactory.GetMockCommandResults(DbConnection.ConnectionString, 
				CommandText);

            if (found != null)
            {
                // remove first result from the queue
                MockCommandResults resultFromQueue = found.Dequeue();

                returnResult = resultFromQueue.ScalarResult;
                if (resultFromQueue.ShouldMockCommandThrowOnExecute)
                {
                    throw new DataAccessException("Test requested mock throw exception on ExecuteScalar.");
                }

				// set any possible output parameter values from the mock
				SetOutputParameters(resultFromQueue);
			}

            return returnResult;
        }


        /// <summary>
        /// Creates a new instance of a DbParameter object.
        /// </summary>
        /// <returns>
        /// A DbParameter object.
        /// </returns>
        protected override DbParameter CreateDbParameter()
        {
            return new MockParameter();
        }


        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <returns>
        /// A mock DbDataReader.
        /// </returns>
        /// <param name="behavior">
        /// An instance of CommandBehavior.
        /// </param>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            // check to see if results exist for this connection/command pair in MockClientFactory
            DataTable[] returnResult = null;

            ThrowIfInvalid("ExecuteReader()");

            var found = MockClientFactory.GetMockCommandResults(DbConnection.ConnectionString, 
				CommandText);

            if (found != null)
            {
                // remove first result from the queue
                MockCommandResults resultFromQueue = found.Dequeue();
                returnResult = resultFromQueue.ResultSet;

                if (resultFromQueue.ShouldMockCommandThrowOnExecute)
                {
                    throw new DataAccessException(
						"Test requested mock throw exception on ExecuteDataReader.");
                }

				// set any possible output parameter values from the mock
				SetOutputParameters(resultFromQueue);
			}

            return new MockDataReader(this)
            {
                MockData = returnResult
            };
        }


        /// <summary>
        /// Overload to set mock property when disposed
        /// </summary>
        /// <param name="disposing">True if dispose is being called by finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsMockDisposed = true;
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Mock property to test whether the command is currently valid
        /// </summary>
        /// <param name="operationName">The name of the operation to check.</param>
        private void ThrowIfInvalid(string operationName)
        {
            // can't perform actions if fake does not have a connection reference
            if (Connection == null)
            {
				var message = string.Format("Cannot perform {0} when no connection specified.",
					operationName);
                throw new DataAccessException(message);
            }

            // can't perform actions if fake connection is not open
            if (Connection.State != ConnectionState.Open)
            {
				var message = string.Format("Cannot perform {0} when connection is not open.",
                    operationName);
                throw new DataAccessException(message);
            }
        }


		/// <summary>
		/// Checks all the output parameters in the mock results against this command's parameter results
		/// and sets the values indicated if they exist.
		/// </summary>
		/// <param name="mockResults">The mock command results to use to inject output parameter values.</param>
		private void SetOutputParameters(MockCommandResults mockResults)
		{
			if (mockResults.OutParameters != null)
			{
				foreach (var outParameter in mockResults.OutParameters)
				{
					if (DbParameterCollection.Contains(outParameter.Key))
					{
						DbParameterCollection[outParameter.Key].Value = outParameter.Value;
					}
				}
			}			
		}
    }
}
