
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using log4net;
using SharedAssemblies.DataAccess.Common;
using SharedAssemblies.General.Database;
using SharedAssemblies.Entities.ExampleDomain;
using SharedAssemblies.General.Utilities;

namespace SharedAssemblies.DataAccess.ExampleDomain
{
	/// <summary>
	/// Implementation of the <see cref="IExampleEntityDao"> to work with a SQL database.
	/// </summary>
	public class SqlExampleEntityDao : IExampleEntityDao
	{
		#region Private Fields

		/// <summary>
		/// Handle to the logger for this type
		/// </summary>
		private static readonly ILog _log =
			LogManager.GetLogger(typeof(SqlExampleEntityDao));

		/// <summary>
		/// Gets or sets the handle to the database utility
		/// </summary>
		private DatabaseUtility Database { get; set; }

		// private messages
		private const string SqlExceptionMessage = "Exception connecting to SQL Database";
		private const string GenericExceptionMessage = "General exception occurred";
		private const string SystemStatusUpdateName = "SystemName";

		#endregion

		/// <summary>
		/// Initializes a new instance of the TicketDataAdapter class
		/// </summary>
		/// <param name="type">Inject the type of database to use</param>
		/// <param name="connection">The connection string to use</param>
		public SqlExampleEntityDao(ClientProviderType type, string connection)
		{
			Database = new DatabaseUtility(type, connection);
		}

		#region Examples of a CRUD DAO

		/// <summary>
		/// Retrieve an <see cref="ExampleEntity"> associated with the identifier
		/// </summary>
		/// <param name="identifier">The id associated with the entity</param>
		/// <returns>A hydrated <see cref="ExampleEntity"></returns>       
		public ExampleEntity GetEntityById(string identifier)
		{
			ExampleEntity result = null;

			try
			{
				// first set up parameters
				ParameterSet parameters = new ParameterSet(Database.ProviderFactory)
	                {
	                    { "@identifier", DbType.String, identifier },
	                };

				// then execute your sql/procedure
				using (IDataReader reader = Database.ExecuteDataReader(
					"dbo.usp_get_some_entity", CommandType.StoredProcedure, parameters))
				{
					// collect a single row of data
					if (reader.Read())
					{
						result = ExampleEntityUtility.Hydrate(reader);
					}
				}
			}
			// if there a data-access error, log and wrap it with a common exception
			catch (SqlException ex)
			{
				_log.Error(SqlExceptionMessage, ex);
				throw new DataAccessException(SqlExceptionMessage, ex.InnerException);
			}
			// if there is an some other error, log it and re-throw
			catch (Exception ex)
			{
				_log.Error(GenericExceptionMessage, ex);
				throw;
			}

			return result; 
		}

		/// <summary>
		/// Get all <see cref="ExampleEntity"> associated with a given branch
		/// </summary>
		/// <param name="branchCode">The branch code to retrieve entities from</param>
		/// <returns>A collection of <see cref="ExampleEntity"></returns>       
		public List<ExampleEntity> GetExampleEntityByBranch(string branchCode)
		{
			List<ExampleEntity> result = new List<ExampleEntity>();

			try
			{
				// first set up parameters
				ParameterSet parameters = new ParameterSet(Database.ProviderFactory)
	                {
	                    { "@branch_code", DbType.String, branchCode },
	                };

				// then execute your sql/procedure
				using (IDataReader reader = Database.ExecuteDataReader(
					"dbo.usp_get_some_entity_by_branch_code", CommandType.StoredProcedure, parameters))
				{
					// collect a single row of data
					while (reader.Read())
					{
						result.Add(ExampleEntityUtility.Hydrate(reader));
					}
				}
			}
			// if there a data-access error, log and wrap it with a common exception
			catch (SqlException ex)
			{
				_log.Error(SqlExceptionMessage, ex);
				throw new DataAccessException(SqlExceptionMessage, ex.InnerException);
			}
			// if there is an some other error, log it and re-throw
			catch (Exception ex)
			{
				_log.Error(GenericExceptionMessage, ex);
				throw;
			}

			return result; 
		}

		/// <summary>
		/// Insert the <see cref="ExampleEntity"> into the database
		/// </summary>
		/// <param name="entity">The entity to insert</param>
		/// <returns>A unique id to reference the <see cref="ExampleEntity"></returns>       
		public string Insert(ExampleEntity entity)
		{
			string result = null;

			try
			{
				// first set up parameters
				ParameterSet parameters = ExampleEntityUtility.Parameterize(
					Database.ProviderFactory, entity);

				// then execute your sql/procedure
				using (IDataReader reader = Database.ExecuteDataReader(
					"dbo.usp_get_some_entity", CommandType.StoredProcedure, parameters))
				{
					// collect a single row of data
					if (reader.Read())
					{
						result = reader["identifier"].ToString();
					}
				}
			}
			// if there a data-access error, log and wrap it with a common exception
			catch (SqlException ex)
			{
				_log.Error(SqlExceptionMessage, ex);
				throw new DataAccessException(SqlExceptionMessage, ex.InnerException);
			}
			// if there is an some other error, log it and re-throw
			catch (Exception ex)
			{
				_log.Error(GenericExceptionMessage, ex);
				throw;
			}

			return result; 
		}

		/// <summary>
		/// Update the requested <see cref="ExampleEntity">
		/// </summary>
		/// <param name="entity">The entity to update</param>
		/// <returns>true if the operation succeeded, false otherwise</returns>       
		public bool Update(ExampleEntity entity)
		{
			bool result = false;

			try
			{
				// first set up parameters
				ParameterSet parameters = ExampleEntityUtility.Parameterize(
					Database.ProviderFactory, entity);

				// then execute your sql/procedure
				int rowsAffected = Database.ExecuteNonQuery(
					"dbo.usp_update_some_entity", CommandType.StoredProcedure, parameters);

				result = (rowsAffected > 0);
			}
			// if there a data-access error, log and wrap it with a common exception
			catch (SqlException ex)
			{
				_log.Error(SqlExceptionMessage, ex);
				throw new DataAccessException(SqlExceptionMessage, ex.InnerException);
			}
			// if there is an some other error, log it and re-throw
			catch (Exception ex)
			{
				_log.Error(GenericExceptionMessage, ex);
				throw;
			}

			return result; 
		}

		/// <summary>
		/// Remove the requested <see cref="ExampleEntity">
		/// </summary>
		/// <param name="entity">The entity to delete</param>
		/// <returns>true if the operation succeeded, false otherwise</returns>       
		public bool Delete(ExampleEntity entity)
		{
			bool result = false;

			try
			{
				// first set up parameters
				ParameterSet parameters = new ParameterSet(Database.ProviderFactory)
	                {
	                    { "@identifier", DbType.String, entity.Identifier },
	                };

				// then execute your sql/procedure
				int rowsAffected = Database.ExecuteNonQuery(
					"dbo.usp_delete_some_entity", CommandType.StoredProcedure, parameters);

				result = (rowsAffected > 0);
			}
			// if there a data-access error, log and wrap it with a common exception
			catch (SqlException ex)
			{
				_log.Error(SqlExceptionMessage, ex);
				throw new DataAccessException(SqlExceptionMessage, ex.InnerException);
			}
			// if there is an some other error, log it and re-throw
			catch (Exception ex)
			{
				_log.Error(GenericExceptionMessage, ex);
				throw;
			}

			return result; 
		}

		#endregion

		#region Examples of Other Database Utility Methods

		/// <summary>
		/// Check the database for the current status of a system variable
		/// </summary>
		/// <param name="system">The system to get the status of</param>
		/// <returns>true if the flush can proceed, false otherwise</returns>
		public bool GetSystemStatus(string system)
		{
			bool result = false;

			try
			{
				// first set up parameters
				ParameterSet parameters = new ParameterSet(Database.ProviderFactory)
	                {
	                    { "@system_name", DbType.String, system },
	                };

				// then execute your sql/procedure
				using (IDataReader reader = Database.ExecuteDataReader(
					"dbo.usp_get_system_status", CommandType.StoredProcedure, parameters))
				{
					if (reader.Read()) // retrieve only one row
					{
						result = reader["system_status"].ToType<bool>();
					}
				}
			}
			// if there a data-access error, log and wrap it with a common exception
			catch (SqlException ex)
			{
				_log.Error(SqlExceptionMessage, ex);
				throw new DataAccessException(SqlExceptionMessage, ex.InnerException);
			}
			// if there is an some other error, log it and re-throw
			catch (Exception ex)
			{
				_log.Error(GenericExceptionMessage, ex);
				throw;
			}

			return result;
		}

		/// <summary>
		/// Set the current status in the database of a system variable
		/// </summary>
		/// <param name="system">The system to get the status of</param>
		/// <param name="status">The value to set the flag to</param>
		/// <returns>The result of the operation</returns>       
		public bool SetSystemStatus(string system, bool status)
		{
			bool result = false;

			try
			{
				// first set up parameters
				ParameterSet parameters = new ParameterSet(Database.ProviderFactory)
					{
						{ "@system_name", DbType.String, system },
						{ "@system_status", DbType.Boolean, status },
						{ "@updated_by", DbType.String, SystemStatusUpdateName },
					};

				// then execute your sql/procedure
				int rowsAffected = Database.ExecuteNonQuery("dbo.usp_set_system_status",
					CommandType.StoredProcedure, parameters);

				result = (rowsAffected > 0);
			}
			// if there a data-access error, log and wrap it with a common exception
			catch (SqlException ex)
			{
				_log.Error(SqlExceptionMessage, ex);
				throw new DataAccessException(SqlExceptionMessage, ex.InnerException);
			}
			// if there is an some other error, log it and re-throw
			catch (Exception ex)
			{
				_log.Error(GenericExceptionMessage, ex);
				throw;
			}

			return result;
		}

		#endregion
	}
}

