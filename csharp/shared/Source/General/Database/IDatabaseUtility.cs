using System.Data;
using System.Data.Common;
using SharedAssemblies.General.Database.Factories;

namespace SharedAssemblies.General.Database
{
    /// <summary>
    /// Interface that DatabaseUtility adheres too.  Useful for interceptor logic.
    /// </summary>
    public interface IDatabaseUtility
    {
		/// <summary>
		/// Property to get/set the connection factory used by this class
		/// </summary>
		DbProviderFactory ProviderFactory { get;}


		/// <summary>
		/// Property to get/set the parameter factory used by this class
		/// </summary>
		ParameterFactory ParameterFactory { get;}


		/// <summary>
		/// Property to get/set the connection string used for the factories
		/// </summary>
		string ConnectionString { get;}

        /// <summary>
        /// Execute a commandText and generate a DataSet with the results
        /// </summary>
        /// <returns>DataSet containing the results of the commandText</returns>
        /// <param name="commandText">The database commandText, should correspond to the commandType</param>
        /// <param name="commandType">Type of commandText, if commandText is inline sql, should be Text,
        /// if commandText is stored proc name, should be StoredProcedure, etc.</param>
        /// <returns>Disconnected data set with results</returns>
        DataSet ExecuteDataSet(string commandText, CommandType commandType);

        /// <summary>
        /// Execute a commandText and generate a DataSet with the results
        /// </summary>
        /// <returns>DataSet containing the results of the commandText</returns>
        /// <param name="commandText">The database commandText, should correspond to the commandType</param>
        /// <param name="commandType">Type of commandText, if commandText is inline sql, should be Text,
        /// if commandText is stored proc name, should be StoredProcedure, etc.</param>
        /// <param name="parameters">An optional array of parameters to apply to the commandText</param>
        /// <returns>Disconnected data set with results</returns>
        DataSet ExecuteDataSet(string commandText, CommandType commandType, ParameterSet parameters);

        /// <summary>
        /// Execute a commandText and generate a DataSet with the results
        /// </summary>
        /// <returns>DataSet containing the results of the commandText</returns>
        /// <param name="commandText">The database commandText, should correspond to the commandType</param>
        /// <param name="commandType">Type of commandText, if commandText is inline sql, should be Text,
        /// if commandText is stored proc name, should be StoredProcedure, etc.</param>
        /// <param name="dataSetToFill">An existing DataSet to fill/merge results in to.</param>
        /// <returns>Disconnected data set with results</returns>
        DataSet ExecuteDataSet(string commandText, CommandType commandType, DataSet dataSetToFill);

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
        DataSet ExecuteDataSet(string commandText, CommandType commandType, ParameterSet parameters,
                                               DataSet dataSetToFill);

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
        IDataReader ExecuteDataReader(string commandText, CommandType commandType);

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
        IDataReader ExecuteDataReader(string commandText, CommandType commandType,
                                                      ParameterSet parameters);

        /// <summary>
        /// Execute a commandText for a scalar (single) value result.  For example:
        ///     select count(*) from employee;
        /// or:
        ///     select max(salary) from employee;
        /// etc.
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <returns>A single result from a query</returns>
        object ExecuteScalar(string commandText, CommandType commandType);

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
        object ExecuteScalar(string commandText, CommandType commandType, ParameterSet parameters);

        /// <summary>
        /// Execute a commandText that simply inserts/updates/deletes or any other
        /// action that will not generate results.
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <returns>Number of rows affected by the action</returns>
        int ExecuteNonQuery(string commandText, CommandType commandType);

        /// <summary>
        /// Execute a commandText that simply inserts/updates/deletes or any other
        /// action that will not generate results.
        /// </summary>
        /// <param name="commandText">The command to execute</param>
        /// <param name="commandType">The type of command</param>
        /// <param name="parameters">The parameters for the command</param>
        /// <returns>Number of rows affected by the action</returns>
        int ExecuteNonQuery(string commandText, CommandType commandType, ParameterSet parameters);

    	/// <summary>
    	/// Returns a parameter set geared to this provider factory
    	/// </summary>
    	/// <returns>ParameterSet for this factory</returns>
    	ParameterSet CreateParameterSet();
    }
}