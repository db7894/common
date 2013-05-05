using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using log4net;
using SharedAssemblies.General.Database;

namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// Implementation of <see cref="IEncryptionKeyDao"/> retrieving keys from a
	/// sql server.
	/// </summary>
    public sealed class SqlEncryptionKeyDao : IEncryptionKeyDao
	{
		#region Private Member Fields

		/// <summary>
        /// Handle to this types logging instance
        /// </summary>
        private static readonly ILog _log =
            LogManager.GetLogger(typeof(SqlEncryptionKeyDao));

        /// <summary>
        /// Gets or sets the handle to the database utility
        /// </summary>
        private readonly DatabaseUtility _database;

        /// <summary>Error messages</summary> 
        private const string _sqlConnectionException = "Exception occured in the datasource";

		/// <summary>
		/// The constant size of the Encryption key database
		/// </summary>
		private const int _encryptionKeyTableSize = 500;

		#endregion

		/// <summary>
        /// Initializes a new instance of the SqlAuthenticationDao class
        /// </summary>
        /// <param name="type">Inject the type of database to use</param>
        /// <param name="connection">The database connection string to use</param>
        public SqlEncryptionKeyDao(ClientProviderType type, string connection)
        {
            _database = new DatabaseUtility(type, connection);
        }

        /// <summary>
        /// Gets the requested encryption keys from the database
        /// </summary>
        /// <param name="keys">The key values to retrieve from the database</param>
        /// <returns>The requested encryption keys</returns>
        public IEnumerable<KeyContainer> GetEncryptionKeysFromDatabase(IEnumerable<int> keys)
        {
			IEnumerable<KeyContainer> result;

            try
            {
				keys = keys.Select(key => key % _encryptionKeyTableSize);
				result = GetEncryptionKeyFromDatabase().Where((_, idx) => keys.Contains(idx));
            }
            catch (Exception ex)
            {
				_log.Error(_sqlConnectionException, ex);
                throw;
            }

			return result;
        }

		/// <summary>
		/// Gets the master encryption key from the database
		/// </summary>
		/// <returns>The current master encryption key</returns>
		public byte[] GetMasterKeyFromDatabase()
		{
			byte[] result = null;

			try
			{
				using (IDataReader reader = _database.ExecuteDataReader(
					"security.usp_get_master_authority", CommandType.StoredProcedure))
				{
					if (reader.Read())
					{
						result = (byte[])reader["master_authority_value"];
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
		/// Gets the encryption keys from the database
		/// </summary>
		/// <returns>An enumerable around the encryption key set</returns>
		private IEnumerable<KeyContainer> GetEncryptionKeyFromDatabase()
		{
			using (IDataReader reader = _database.ExecuteDataReader(
				"security.usp_select_encryption_components",
				CommandType.StoredProcedure))
			{
				while (reader.Read())
				{
					yield return new KeyContainer
					{
						EncryptionKey = (byte[])reader["encrypt_component_value"],
						SigningKey = (byte[])reader["signed_component_value"],
					};
				}
			}
		}
    }
}
