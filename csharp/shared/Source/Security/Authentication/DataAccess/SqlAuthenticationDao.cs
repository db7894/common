using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using SharedAssemblies.Core.Conversions;
using SharedAssemblies.General.Database;
using SharedAssemblies.Security.Authentication.Entities;

namespace SharedAssemblies.Security.Authentication.DataAccess
{
	/// <summary>
	/// Implementation of <see cref="IAuthenticationDao"/> adapting to a sql database
	/// </summary>
	public sealed class SqlAuthenticationDao : IAuthenticationDao
	{
		#region private members

		/// <summary>
		/// Gets or sets the handle to the database utility
		/// </summary>
		private readonly DatabaseUtility _frontEndDatabase;

		/// <summary>
		/// Gets or sets the handle to the database utility
		/// </summary>
		private readonly DatabaseUtility _customersDatabase;

		/// <summary>
		/// Handle to this types logging instance
		/// </summary>
		private static readonly ILog _log =
			LogManager.GetLogger(typeof(SqlAuthenticationDao));

		/// <summary>Exception messages</summary>
		private const string _sqlConnectionException = "Exception occured in the datasource";

		#endregion

		/// <summary>
		/// Initializes a new instance of the SqlAuthenticationDao class
		/// </summary>
		/// <param name="type">Inject the type of database to use</param>
		/// <param name="frontEndConnection">connection string for frontend db</param>
		/// <param name="customersConnection">connection string for customers db</param>
		public SqlAuthenticationDao(ClientProviderType type, string frontEndConnection,
			string customersConnection)
		{
			_frontEndDatabase = new DatabaseUtility(type, frontEndConnection);
			_customersDatabase = new DatabaseUtility(type, customersConnection);
		}

		#region Implementation of IAuthenticationDao

		/// <summary>
		/// Inserts a new session into the database
		/// </summary>
		/// <param name="session">The new session to insert in the database</param>
		/// <returns>The result of the operation</returns>
		/// <throws>DataAccessException</throws>
		public bool InsertSession(SessionEntity session)
		{
			int rowsAffected;

			try
			{
				var parameters = new ParameterSet(_frontEndDatabase.ProviderFactory)
				{
					{ "@vendor_global_id", DbType.String, session.VenderIdentifier },
					{ "@session_type_id", DbType.Int16, session.Type },
					{ "@login_id", DbType.String, session.LoginIdentifier },
					{ "@session_string", DbType.String, session.SessionValue },
					{ "@ip_address", DbType.String, session.IpAddress },
				};

				// NO COUNT is turned off so it returns the rows affected
				rowsAffected = _frontEndDatabase.ExecuteNonQuery(
					"userinterface.usp_insert_login_session",
					CommandType.StoredProcedure, parameters);
			}
			catch (Exception ex) 
			{
				_log.Error(_sqlConnectionException, ex);
				throw;
			}

			return rowsAffected > 0;
		}

		/// <summary>
		/// Deletes the given session if it exists
		/// </summary>
		/// <param name="vendor">The unique identifier for this vendor</param>
		/// <param name="identifier">The unique indentifier for this user</param>
		/// <returns>The result of the operation</returns>
		public bool DeleteSession(string vendor, string identifier)
		{
			int rowsAffected;

			try
			{
				var parameters = new ParameterSet(_frontEndDatabase.ProviderFactory)
				{
					{ "@vendor_id", DbType.String, vendor },
					{ "@login_id", DbType.String, identifier },
				};

				// NO COUNT is off so this actually works as intended
				rowsAffected = _frontEndDatabase.ExecuteNonQuery(
					"userinterface.usp_delete_session", CommandType.StoredProcedure, parameters);
			}
			catch (Exception ex)
			{
				_log.Error(_sqlConnectionException, ex);
				throw;
			}

			return rowsAffected > 0;
		}

		/// <summary>
		/// Gets the session id if it exists
		/// </summary>
		/// <param name="vendor">The unique identifier for this vender</param>
		/// <param name="sessionType">The type of session</param>
		/// <param name="identifier">The unique indentifier for this user</param>
		/// <returns>The session associate with this user</returns>
		public SessionEntity GetSessionById(string vendor, SessionType sessionType,
			string identifier)
		{
			SessionEntity result = null;

			try
			{
				var parameters = new ParameterSet(_frontEndDatabase.ProviderFactory)
				{
					{ "@vendor_global_id", DbType.String, vendor },
					{ "@login_id", DbType.String, identifier },
					{ "@session_type_id", DbType.Int16, sessionType },
				};

				using (IDataReader reader = _frontEndDatabase.ExecuteDataReader(
					"userinterface.usp_select_login_session",
					CommandType.StoredProcedure, parameters))
				{
					if (reader.Read())
					{
						result = GenerateEntityFromDatabaseReader(reader);
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error(_sqlConnectionException, ex);
				throw;
			}

			return result;
		}

	

		/// <summary>
		/// Get All Account Associated with the account passed.
		/// </summary>
		/// <param name="account">account to get linked accounts for</param>
		/// <returns>A collection of accounts liked to this account</returns>
		public IEnumerable<string> GetLinkedAccounts(string account)
		{
			List<string> linkedAccounts = new List<string>();

			try
			{
				var parameters = new ParameterSet(_customersDatabase.ProviderFactory)
				{
					{ "@account_parent", DbType.String, account },
				};

				using (IDataReader reader = _customersDatabase.ExecuteDataReader(
					"dbo.usp_get_child_accounts", CommandType.StoredProcedure, parameters))
				{
					while (reader.Read())
					{
						linkedAccounts.Add(reader["account_child"].ToString());
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error(_sqlConnectionException, ex);
				throw;
			}

			return linkedAccounts;
		}

		#endregion

		#region Private Helper Methods

		/// <summary>
		/// Helper to abstract mapping from the database to entity
		/// </summary>
		/// <param name="reader">The DataReader to extract a ticket from</param>
		/// <returns>A populated esignature entity from the database record</returns>
		private static SessionEntity GenerateEntityFromDatabaseReader(IDataRecord reader)
		{
			bool isValid = (Int32.Parse(reader["login_flag"].ToString()) == 0) ? false : true;

			return isValid ? new SessionEntity
			{
				IpAddress = reader["ip_address"].ToString(),
				LoginIdentifier = reader["login_id"].ToString(),
				Type = (SessionType)Int32.Parse(reader["session_type_id"].ToString()),
				VenderIdentifier = reader["vendor_global_id"].ToString(),
				SessionValue = reader["session_string"].ToString(),
			}
			: null;
		}

		#endregion
	}
}