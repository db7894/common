using System;
using System.Data;
using log4net;
using SharedAssemblies.Core.Xml;
using SharedAssemblies.General.Database;
using SharedAssemblies.Security.Encryption;

namespace SharedAssemblies.Security.Nonce
{
	/// <summary>
	/// Implementation of INonceDao backed with a SQL database
	/// </summary>
	public sealed class SqlNonceDao : INonceDao
	{
        #region private members

		/// <summary>
		/// Gets or sets the handle to the database utility
		/// </summary>
        private readonly DatabaseUtility _database;

        /// <summary>
        /// Handle to this types logging instance
        /// </summary>
		private static readonly ILog _log =
			LogManager.GetLogger(typeof(SqlNonceDao));

        /// <summary>Exception message for all other exceptions</summary>
        private const string _genericExceptionMessage = "General exception occurred";

		/// <summary>
		/// The session type that the nonce assembly will use
		/// </summary>
		private const int _sessionTypeId = 3;

		/// <summary>
		/// The vendor identifier to describe what customer is calling this service
		/// </summary>
		private readonly string _vendorId;

        #endregion

		#region Implementation of INonceDao

		/// <summary>
		/// Initializes a new instance of the SqlAuthenticationDao class
		/// </summary>
		/// <param name="type">Inject the type of database to use</param>
		/// <param name="connection">The database connection string to use</param>
		/// <param name="vendorId">The vendor to associate with these requests</param>
		public SqlNonceDao(ClientProviderType type, string connection, string vendorId)
		{
            _database = new DatabaseUtility(type, connection);
			_vendorId = vendorId;
		}

		/// <summary>
		/// Store a value and get a key to retrieve it with
		/// </summary>
		/// <param name="data">The data to store</param>
		/// <returns>The key associated with the data</returns>
		public string Store(string data)
		{
			string result;
			
			try
			{
				var identifier = EncryptionUtility.GenerateRandomStringToken(24);
				var parameters = new ParameterSet(_database.ProviderFactory)
					{
						{ "@vendor_global_id", DbType.String, _vendorId },
						{ "@login_id", DbType.String, identifier },

						// this can be max 255 characters, do we let the database deal with it?
						{ "@session_string", DbType.String, data },
						{ "@session_type_id", DbType.Int16, _sessionTypeId },
					};

				int rowsAffected = _database.ExecuteNonQuery("userinterface.usp_insert_login_session",
					CommandType.StoredProcedure, parameters);
				result = (rowsAffected > 0) ? identifier : null;
			}
			catch (Exception ex)
			{
				_log.Error(_genericExceptionMessage, ex);
				throw;
			}

			return result;
		}

		/// <summary>
		/// Store a value and get a key to retrieve it with
		/// </summary>
		/// <typeparam name="T">The type of data to serialize</typeparam>
		/// <param name="data">The data to store</param>
		/// <returns>The key associated with the data</returns>
		public string Store<T>(T data)
		{
			return Store(XmlUtility.XmlFromType(data));
		}

		/// <summary>
		/// Retrieve data associated with the given key
		/// </summary>
		/// <param name="name">The key to retrieve data for</param>
		/// <returns>The data associated with the key</returns>
		public string Retrieve(string name)
		{
			string result = null;

			try
			{
				var parameters = new ParameterSet(_database.ProviderFactory)
					{
						{ "@vendor_global_id", DbType.String, _vendorId },
						{ "@login_id", DbType.String, name },
						{ "@session_type_id", DbType.Int16, _sessionTypeId },
					};

				// TODO Need to make sure database team logically deletes this data
				using (IDataReader reader = _database.ExecuteDataReader(
					"userinterface.usp_select_login_session", CommandType.StoredProcedure,
					parameters))
				{
					if (reader.Read())
					{
						result = reader["session_string"].ToString();
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error(_genericExceptionMessage, ex);
				throw;
			}

			return result;
		}

		/// <summary>
		/// Retrieve data associated with the given key
		/// </summary>
		/// <typeparam name="T">The type of data to serialize</typeparam>
		/// <param name="name">The key to retrieve data for</param>
		/// <returns>The data associated with the key</returns>
		public T Retrieve<T>(string name)
		{
			string result = Retrieve(name);

			return string.IsNullOrEmpty(result)
				? default(T)
				: XmlUtility.TypeFromXml<T>(result);
		}

		#endregion
	}
}
