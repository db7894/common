using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using SharedAssemblies.General.Database.Factories;


namespace SharedAssemblies.General.Database
{
    /// <summary>
    /// Utility class that does a lot of "one-shot" queries that open the connection,
    /// run the query or update, then close the connection.
    /// </summary>
    public sealed class DatabaseUtility : IDatabaseUtility
    {
		/// <summary>
		/// Property to get/set the connection factory used by this class
		/// </summary>
		public DbProviderFactory ProviderFactory { get; private set; }


		/// <summary>
		/// Property to get/set the parameter factory used by this class
		/// </summary>
		public ParameterFactory ParameterFactory { get; private set; }


		/// <summary>
		/// Property to get/set the connection string used for the factories
		/// </summary>
		public string ConnectionString { get; private set; }


		/// <summary>
        /// Construct a database utility using an enum to specify factory commandType
        /// </summary>
        /// <param name="providerType">Enum specifying the commandType of the factory</param>
        /// <param name="connectionString">Connection string for connections</param>
        public DatabaseUtility(ClientProviderType providerType, string connectionString)
            : this(ClientProviderFactory.Get(providerType), connectionString)
        {
        }


        /// <summary>
        /// Construct a database utility using a reference to an existing 
        /// connection factory
        /// </summary>
        /// <param name="factory">Reference to a connection factory</param>
        /// <param name="connectionString">Connection string for connections</param>
        public DatabaseUtility(DbProviderFactory factory, string connectionString)
        {
            ProviderFactory = factory;
            ConnectionString = connectionString;

            // parameter factory to allow construction of parameters is client neutral way
            ParameterFactory = new ParameterFactory(factory);
        }


        /// <summary>
        /// Returns a parameter set geared to this provider factory
        /// </summary>
        /// <returns>ParameterSet for this factory</returns>
        public ParameterSet CreateParameterSet()
        {
            return ParameterFactory.CreateSet();
        }


        /// <summary>
        /// Execute a commandText and generate a DataSet with the results
        /// </summary>
        /// <returns>DataSet containing the results of the commandText</returns>
        /// <param name="commandText">The database commandText, should correspond to the commandType</param>
        /// <param name="commandType">Type of commandText, if commandText is inline sql, should be Text,
        /// if commandText is stored proc name, should be StoredProcedure, etc.</param>
        /// <returns>Disconnected data set with results</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType)
        {
            return ExecuteDataSet(commandText, commandType, null, new DataSet());
        }


        /// <summary>
        /// Execute a commandText and generate a DataSet with the results
        /// </summary>
        /// <returns>DataSet containing the results of the commandText</returns>
        /// <param name="commandText">The database commandText, should correspond to the commandType</param>
        /// <param name="commandType">Type of commandText, if commandText is inline sql, should be Text,
        /// if commandText is stored proc name, should be StoredProcedure, etc.</param>
        /// <param name="parameters">An optional array of parameters to apply to the commandText</param>
        /// <returns>Disconnected data set with results</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType, ParameterSet parameters)
        {
            return ExecuteDataSet(commandText, commandType, parameters, new DataSet());
        }


        /// <summary>
        /// Execute a commandText and generate a DataSet with the results
        /// </summary>
        /// <returns>DataSet containing the results of the commandText</returns>
        /// <param name="commandText">The database commandText, should correspond to the commandType</param>
        /// <param name="commandType">Type of commandText, if commandText is inline sql, should be Text,
        /// if commandText is stored proc name, should be StoredProcedure, etc.</param>
        /// <param name="dataSetToFill">An existing DataSet to fill/merge results in to.</param>
        /// <returns>Disconnected data set with results</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType, DataSet dataSetToFill)
        {
            return ExecuteDataSet(commandText, commandType, null, dataSetToFill);
        }


        /// <summary>
        /// Execute a commandText and generate a DataSet with the results
        /// </summary>
        /// <returns>DataSet containing the results of the commandText</returns>
        /// <param name="commandText">The database commandText, should correspond to the commandType</param>
        /// <param name="commandType">Type of commandText, if commandText is inline sql, should be Text,
        /// if commandText is stored proc name, should be StoredProcedure, etc.</param>
        /// <param name="parameters">An optional array of parameters to apply to the commandText</param>
        /// <param name="dataSetToFill">An existing DataSet to fill/merge results in to.</param>
        /// <returns>Disconnected data set with results</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType, ParameterSet parameters,
            DataSet dataSetToFill)
        {
            // clean up connection after using
            using (DbConnection connection = OpenConnection())
            {
                // clean up commandText immediately after using
                using (DbCommand command = CreateCommand(connection, commandText, commandType, parameters))
                {
                    return FillDataSet(command, dataSetToFill);
                }
            }
        }


        /// <summary>
        /// Execute a commandText and open a forward-only, read-only DataReader
        /// for the results
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <returns>
        /// An IDataReader that will proces the results.  This reader MUST
        /// be disposed when you are done with it.
        /// </returns>
        public IDataReader ExecuteDataReader(string commandText, CommandType commandType)
        {
            return ExecuteDataReader(commandText, commandType, null);
        }


        /// <summary>
        /// Execute a commandText and open a forward-only, read-only DataReader
        /// for the results
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <param name="parameters">The parameters for the command</param>
        /// <returns>
        /// An IDataReader that will proces the results.  This reader MUST
        /// be disposed when you are done with it.
        /// </returns>
        public IDataReader ExecuteDataReader(string commandText, CommandType commandType,
            ParameterSet parameters)
        {
            // we do NOT dispose this connection, the specialized data reader does that for us
            DbConnection connection = OpenConnection();

            // similarly we don't dispose the commandText, the specialized data reader will for us
            DbCommand command = CreateCommand(connection, commandText, commandType, parameters);

            // wrap the reader in a decorator that will clean up the command and connection on disposal
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }


        /// <summary>
        /// Execute a commandText for a scalar (single) value result.  For example:
        ///     select count(*) from employee;
        /// or:
        ///     select max(salary) from employee;
        /// etc.
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of the command to execute</param>
        /// <returns>A single result from a query</returns>
        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            return ExecuteScalar(commandText, commandType, null);
        }


        /// <summary>
        /// Execute a commandText for a scalar (single) value result.  For example:
        ///     select count(*) from employee;
        /// or:
        ///     select max(salary) from employee;
        /// etc.
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <param name="parameters">The parameters for the command</param>
        /// <returns>A single result from a query</returns>
        public object ExecuteScalar(string commandText, CommandType commandType, ParameterSet parameters)
        {
            // clean up connection after using
            using (DbConnection connection = OpenConnection())
            {
                // clean up commandText immediately after using
                using (DbCommand command = CreateCommand(connection, commandText, commandType, parameters))
                {
                    return command.ExecuteScalar();
                }
            }
        }


        /// <summary>
        /// Execute a commandText that simply inserts/updates/deletes or any other
        /// action that will not generate results.
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <returns>Number of rows affected by the action</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, commandType, null);
        }


        /// <summary>
        /// Execute a commandText that simply inserts/updates/deletes or any other
        /// action that will not generate results.
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <param name="parameters">The parameters for the command</param>
        /// <returns>Number of rows affected by the action</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType, ParameterSet parameters)
        {
                // clean up connection after using
            using (DbConnection connection = OpenConnection())
            {
                // clean up commandText immediately after using
                using (DbCommand command = CreateCommand(connection, commandText, commandType, parameters))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Returns a string representing the database connection string
        /// </summary>
        /// <returns>String value representing the connection</returns>
        public override string ToString()
        {
            return string.Format("{0}({1})", GetType().FullName, ConnectionString ?? string.Empty);
        }


        /// <summary>
        /// Helper method to create and open a commandText given a connection string
        /// This MUST be disposed to prevent resource leaks.  All public methods using 
        /// this are in using blocks except the one that returns a reader, but it
        /// returns a special decorator reader that closes the connection when the
        /// reader is closed.
        /// </summary>
        /// <returns>A DbConnection object representing the database connection</returns>
        private DbConnection OpenConnection()
        {
            // create an implementation neutral connection given our provider factory
            DbConnection connection = ProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();

            return connection;
        }


        /// <summary>
        /// Helper method to create a commandText given a connection and commandText details.
        /// This MUST be disposed to prevent resource leaks (though closing the underlying
        /// connection will also dispose the commandText, I think).
        /// </summary>
        /// <param name="connection">Open connection to database</param>
        /// <param name="commandText">Text of commandText</param>
        /// <param name="commandType">Type of commandText (stored proc, text, etc)</param>
        /// <param name="parameters">parameters for commandText</param>
        /// <returns>A DbCommand object representing the commandText</returns>
        private DbCommand CreateCommand(DbConnection connection, string commandText, CommandType commandType, 
			IEnumerable<DbParameter> parameters)
        {
            // create commandText from the provider factory specific for the chosen implementation
            DbCommand command = ProviderFactory.CreateCommand();
            command.Connection = connection;
            command.CommandText = commandText;
            command.CommandType = commandType;

            // assign the parameters, if any
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }


        /// <summary>
        /// Create and return a data adapter from a commandText object
        /// </summary>
        /// <param name="selectCommand">DbCommand object representing the select commandText to execute</param>
        /// <param name="dataSetToFill">The dataset to fill/merge in to</param>
        /// <returns>DataSet with results</returns>
        private DataSet FillDataSet(DbCommand selectCommand, DataSet dataSetToFill)
        {
            // create the data adapter and fill in the data set from it
            using (var adapter = ProviderFactory.CreateDataAdapter())
            {
                adapter.SelectCommand = selectCommand;

                adapter.Fill(dataSetToFill);
                return dataSetToFill;
            }
        }
    }
}
