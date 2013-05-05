using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using log4net;
using SharedAssemblies.General.Database.Exceptions;

namespace SharedAssemblies.General.Database
{
	/// <summary>
	/// This class provides access to SQL Server Service Broker queues.  It's features 
	/// include the means to perform transactional gets from the queue (via commit 
	/// and rollback).  This class also handles the deserialization of the XML records
	/// from the queue (deserializes into objects of generic type T)
	/// </summary>
	/// <typeparam name="T">Type into which queue records will be deserialized</typeparam>
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.ReadabilityAnalyzer",
		"ST2002:MethodCannotExceedMaxLines",
		Justification = "Legacy Code - Delaying refactoring to avoid breaking existing callers.")]

	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5007:NoCatchSystemException",
		Justification = "Legacy Code - Delaying refactoring to avoid breaking existing callers.")]

	public class SqlServerServiceBrokerQueue<T> : ISqlServerServiceBrokerQueue<T>
	{
		/// <summary>The sql connection</summary>
		private SqlConnection _sqlConnection = null;

		/// <summary>Transaction for removing items from queue safely</summary>
		private SqlTransaction _sqlTransaction = null;

		/// <summary>Sql command to retrieve items.</summary>
		private SqlCommand _sqlCommand = null;

		/// <summary>Connection string for database connection.</summary>
		private readonly string _dbConnectionString;

		/// <summary>Stored proc name for receiving from queue.</summary>
		private readonly string _receiveStoredProcName;

		/// <summary>Stored proc name for inserting into queue.</summary>
		private readonly string _insertStoredProcName;

		/// <summary>The type of the queue.</summary>
		private readonly string _queueType = string.Empty;

		/// <summary>The logger instance to log to.</summary>
		private readonly ILog _logger = null;

		/// <summary>When last record was received.</summary>
		private string _lastRecordReceived = null;


		/// <summary>
		/// Get/set the queue receive timeout if desired
		/// </summary>
		public int QueueReadTimeoutMs { get; set; }

		/// <summary>
		/// Get/set the column name of the result set containing a queue record
		/// when returned by the receive stored proc - if needed (if differs from default)
		/// </summary>
		public string NextQueueRecordColumnName { get; set; }

		/// <summary>
		/// Get/set the name of the parameter of the stored procedure used for 
		/// inserting/reinserting data to the queue - if needed (if differs from default)
		/// </summary>
		public string InsertStoredProcParamName { get; set; }

		/// <summary>
		/// Basic constructor which does not require a logger object
		/// </summary>
		/// <param name="dbConnectionString">database connection string for queue</param>
		/// <param name="receiveStoredProcName">stored proc for receiving from queue</param>
		/// <param name="insertStoredProcName">stored proc for inserting/reinserting to queue</param>
		/// <param name="queueType">type of the queue - any descriptive word(s)</param>
		public SqlServerServiceBrokerQueue(string dbConnectionString, string receiveStoredProcName,
		                                   string insertStoredProcName, string queueType)
			: this(dbConnectionString, receiveStoredProcName, insertStoredProcName, queueType, null)
		{
		}

		/// <summary>
		/// Constructor which allows user to provide a logging object
		/// </summary>
		/// <param name="dbConnectionString">database connection string for queue</param>
		/// <param name="receiveStoredProcName">stored proc for receiving from queue</param>
		/// <param name="insertStoredProcName">stored proc for inserting/reinserting to queue</param>
		/// <param name="queueType">type of the queue - any descriptive word(s)</param>
		/// <param name="logger">logging object to be used by this class</param>
		public SqlServerServiceBrokerQueue(string dbConnectionString, string receiveStoredProcName,
		                                   string insertStoredProcName, string queueType, ILog logger)
		{
			_dbConnectionString = dbConnectionString;
			_receiveStoredProcName = receiveStoredProcName;
			_insertStoredProcName = insertStoredProcName;
			_queueType = queueType;
			_logger = logger;

			QueueReadTimeoutMs = 30000; // default to 30 seconds
			NextQueueRecordColumnName = "message_body";
			InsertStoredProcParamName = "@message";
		}

		/// <summary>
		/// Get next record from queue.
		/// </summary>
		/// <returns>next record; null if no record available</returns>
		public T GetNextRecord()
		{
			T nextQueueRecord = default(T);
			string xmlStringRecord = GetNextRecordString();

			if (null == xmlStringRecord)
			{
				CommitTransaction(); // commit the transaction if not data was received
			}
			else
			{
				_lastRecordReceived = xmlStringRecord; // store last transaction for potential reinsert

				nextQueueRecord = ParseRecordString(xmlStringRecord);
			}

			return nextQueueRecord;
		}

		/// <summary>
		/// Insert a record into queue
		/// </summary>
		/// <param name="record">The record to insert.</param>
		public void InsertRecord(T record)
		{
			string xmlRecord = string.Empty;

			using (StringWriter stringWriter = new StringWriter())
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

				xmlSerializer.Serialize(stringWriter, record);
				xmlRecord = stringWriter.ToString();
			}

			InsertRecordToDatabase(xmlRecord);
		}

		/// <summary>
		/// Commit get operation.  Users of this class must commit or rollback 
		/// transaction after any queue receive that returns no data (null).
		/// </summary>
		/// <returns>'true' if successful</returns>
		public bool CommitTransaction()
		{
			bool returnVal = true;

			try
			{
				_sqlTransaction.Commit();
			}
			catch (SqlException se)
			{
				if (null != _logger)
				{
					StringBuilder builder = new StringBuilder();
					builder.Append("SQL Exception committing query transaction from DB queue: ");

					AppendSqlExceptionErrorInfoToString(builder, se);

					_logger.Error(builder.ToString());
				}
				returnVal = false;
			}
			catch (Exception e)
			{
				if (null != _logger)
				{
					_logger.ErrorFormat(
						"Exception caught while committing query transaction from DB queue: {0} -- {1}",
						e.Message, e.StackTrace);
				}
				returnVal = false;
			}
			finally
			{
				CleanupSqlResources();
				_sqlConnection = null; // reset so we can verify no open transaction upon next queue read
				_lastRecordReceived = null; // reset so we never risk reinserting a record that was
				//    successfully processed.
			}

			return returnVal;
		} // End CommitTransaction()

		/// <summary>
		/// Undo get operation.  Users of this class must commit or rollback 
		/// transaction after any queue receive that returns no data (null).
		/// </summary>
		/// <returns>'true' if successful</returns>
		public bool RollbackTransaction()
		{
			bool returnVal = true;

			try
			{
				// not doing actual SQL rollback since 5 in a row disables queue
				//    (instead reinsert and commit, in that order, so transaction
				//    remains open until we're done with record)
				try
				{
					ReinsertRecord();
				}
				catch (Exception)
				{
					_sqlTransaction.Rollback();
				}
				_sqlTransaction.Commit();
			}
			catch (SqlException se)
			{
				if (null != _logger)
				{
					StringBuilder builder = new StringBuilder();
					builder.Append("SQL Exception rolling back query transaction from DB queue: ");

					AppendSqlExceptionErrorInfoToString(builder, se);

					_logger.Error(builder.ToString());
				}
				returnVal = false;
			}
			catch (Exception e)
			{
				if (null != _logger)
				{
					_logger.ErrorFormat(
						"Exception caught while rolling back query transaction from DB queue: {0} -- {1}",
						e.Message, e.StackTrace);
				}
				returnVal = false;
			}
			finally
			{
				CleanupSqlResources();
				_sqlConnection = null; // reset so we can verify no open transaction upon next queue read
				_lastRecordReceived = null; // reset to prevent chance of reinserting same record twice
			}

			return returnVal;
		} // End RollbackTransaction()

		/// <summary>
		/// Get next XML record string from the DB queue
		/// </summary>
		/// <returns>XML string - null if queue empty</returns>
		private string GetNextRecordString()
		{
			string xmlString;

			try
			{
				xmlString = ExecuteQueueReceive();
			}
			catch (SqlException se)
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("SQL Exception querying DB queue for next {0} record: ", _queueType);

				AppendSqlExceptionErrorInfoToString(builder, se);

				if (null != _logger)
				{
					_logger.Error(builder.ToString());
				}
				throw new Exception("SQL Exception: " + se.Message);
			}
			catch (IndexOutOfRangeException iore)
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat(
					"Index out of range exception querying DB queue for next {0} record "
					+ "probably due to bad column name: {1}",
					_queueType, iore.Message);

				if (null != _logger)
				{
					_logger.Error(builder.ToString());
				}
				throw new Exception(iore.Message);
			}
			catch (Exception e)
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("Exception querying DB queue for next {0} record: {1} -- {2}",
				                     _queueType, e.Message, e.StackTrace);

				if (null != _logger)
				{
					_logger.Error(builder.ToString());
				}
				throw;
			}

			return xmlString;
		} // End GetNextRecordString()

		/// <summary>
		/// Do the detail work of getting the next record from the database queue
		/// </summary>
		/// <returns>XML string - null if queue empty</returns>
		private string ExecuteQueueReceive()
		{
			string xmlString = null; // default return value
			string connectionString = _dbConnectionString;
			string messageBodyDbColumnName = NextQueueRecordColumnName;

			if (null == _sqlConnection)
			{
				_sqlConnection = new SqlConnection(connectionString);
			}
			else
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat(
					"Cannot get next {0} record from DB queue because DAO object " +
					"already has open SQL transaction.",
					_queueType);

				throw new Exception(builder.ToString());
			}
			_sqlConnection.Open();

			_sqlCommand = new SqlCommand(_receiveStoredProcName, _sqlConnection);

			// set the command timeout seconds to be
			// longer than the stored proc timeout
			_sqlCommand.CommandTimeout = QueueReadTimeoutMs / 1000 + 30;

			_sqlTransaction = _sqlConnection.BeginTransaction();
			_sqlCommand.Transaction = _sqlTransaction;
			_sqlCommand.CommandType = CommandType.StoredProcedure;

			// The timeout parameter needs to be low (at least less than 1 minute)
			//    so we don't leave open a long running transaction which could
			//    prevent truncation of SQL Server transaction logs
			SqlParameter myParam = _sqlCommand.Parameters.Add("@timeout", SqlDbType.Int);
			myParam.Value = QueueReadTimeoutMs;

			SqlDataReader sqlDataReader = _sqlCommand.ExecuteReader();

			if (sqlDataReader.Read())
			{
				int messageBodyDbColumnIndex = sqlDataReader.GetOrdinal(messageBodyDbColumnName);
				if (!sqlDataReader.IsDBNull(messageBodyDbColumnIndex))
				{
					xmlString = sqlDataReader.GetString(messageBodyDbColumnIndex);
				}
			}
			sqlDataReader.Close();

			return xmlString;
		}

		/// <summary>
		/// Deserialize XML string into object of type T
		/// </summary>
		/// <param name="recordString">XML string to be deserialized</param>
		/// <returns>result of deserialization - object of type T</returns>
		private T ParseRecordString(string recordString)
		{
			T newRecord = default(T);

			try
			{
				using (StringReader stringReader = new StringReader(recordString))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(T));

					newRecord = (T) serializer.Deserialize(stringReader);
				}
			}
			catch (InvalidOperationException ioe)
			{
				if (null != _logger)
				{
					_logger.ErrorFormat(
						"Exception thrown deserializing XML from {0} queue: {1} / {2} -- XML string: {3}",
						_queueType, ioe.Message, ioe.InnerException.Message, recordString);
				}
				throw new QueueDataParsingException(ioe.Message, recordString);
			}
			catch (Exception e)
			{
				if (null != _logger)
				{
					_logger.ErrorFormat("Exception getting next {0} from DB queue: {1} -- {2}",
					                    _queueType, e.Message, e.StackTrace);
				}
				throw new QueueDataParsingException(e.Message, recordString);
			}

			return newRecord;
		} // End ParseRecordString()

		/// <summary>
		/// Reinsert a record back into queue
		/// </summary>
		private void ReinsertRecord()
		{
			if (null != _lastRecordReceived)
			{
				InsertRecordToDatabase(_lastRecordReceived);
			}
			else
			{
				_logger.WarnFormat("Invalid attempt to rollback {0} queue transaction "
				                   + "that is not open.", _queueType);
			}
		} // End ReinsertRecord()

		/// <summary>
		/// Insert a record into database queue
		/// </summary>
		/// <param name="xmlRecord">The xml record to insert into database.</param>
		private void InsertRecordToDatabase(string xmlRecord)
		{
			try
			{
				using (SqlConnection sqlConn = new SqlConnection(_dbConnectionString))
				{
					sqlConn.Open();

					using (SqlCommand sqlCmd = new SqlCommand(_insertStoredProcName, sqlConn))
					{
						sqlCmd.CommandType = CommandType.StoredProcedure;

						SqlParameter myParam = sqlCmd.Parameters.Add(InsertStoredProcParamName, 
							SqlDbType.Xml);

						myParam.Value = xmlRecord;

						int numRowsAffected = sqlCmd.ExecuteNonQuery();

						if (0 == numRowsAffected)
						{
							if (null != _logger)
							{
								_logger.WarnFormat(
									"Zero DB rows affected while reinserting {0} record into DB queue.  "
									+ "Record={1}",
									_queueType, xmlRecord ?? string.Empty);
							}
						}
					} // end using (SqlCommand...)
				} // end using (SqlConnection...)
			}
			catch (SqlException se)
			{
				if (null != _logger)
				{
					StringBuilder builder = new StringBuilder();
					builder.AppendFormat("SQL Exception reinserting {0} record into DB (record={1}) -- ",
					                     _queueType, xmlRecord ?? string.Empty);

					AppendSqlExceptionErrorInfoToString(builder, se);

					_logger.ErrorFormat(builder.ToString());
				}
				throw new Exception("SQL Exception: " + se.Message);
			}
			catch (Exception e)
			{
				if (null != _logger)
				{
					_logger.ErrorFormat("Exception reinserting {0} record into DB (record={1}): {2} -- {3}",
					                    _queueType, xmlRecord ?? string.Empty, e.Message, e.StackTrace);
				}
				throw;
			}
		}

		/// <summary>
		/// Append the details of a SqlException to a string (actually StringBuilder)
		/// </summary>
		/// <param name="builder">StringBuilder object to which the error info will be appended</param>
		/// <param name="se">the SqlException object containing the error info</param>
		private static void AppendSqlExceptionErrorInfoToString(StringBuilder builder, SqlException se)
		{
			foreach (SqlError error in se.Errors)
			{
				builder.AppendFormat("Message: {0} | ", error.Message);
				builder.AppendFormat("ErrorCode: {0} | ", error.Number);
				builder.AppendFormat("LineNumber: {0} | ", error.LineNumber);
				builder.AppendFormat("Server: {0} | ", error.Server);
				builder.AppendFormat("Procedure: {0} ***", error.Procedure);
			}
		}

		/// <summary>
		/// Cleanup unmanaged DB resources 
		/// </summary>
		private void CleanupSqlResources()
		{
			try
			{
				_sqlCommand.Dispose();
			}
			catch (Exception e)
			{
				if (null != _logger)
				{
					_logger.ErrorFormat("Exception while disposing of SqlCommand object: {0} -- {1}",
					                    e.Message, e.StackTrace);
				}
			}

			try
			{
				_sqlConnection.Close();
			}
			catch (Exception e)
			{
				if (null != _logger)
				{
					_logger.ErrorFormat("Exception while closing SqlConnection object: {0} -- {1}",
					                    e.Message, e.StackTrace);
				}
			}
		}
	}
}