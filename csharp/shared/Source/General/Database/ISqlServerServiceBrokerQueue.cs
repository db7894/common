namespace SharedAssemblies.General.Database
{
	/// <summary>
	/// Interfact for exchanging data with Bashwork SQL Server Service Broker queues.
	/// </summary>
	/// <typeparam name="T">Type into which queue records will be deserialized</typeparam>
	public interface ISqlServerServiceBrokerQueue<T>
	{
		/// <summary>
		/// Get next record from queue.
		/// </summary>
		/// <returns>next record; null if no record available</returns>
		T GetNextRecord();

		/// <summary>
		/// Insert a record into queue
		/// </summary>
		/// <param name="record">The record to insert.</param>
		void InsertRecord(T record);

		/// <summary>
		/// Commit get operation.  Users of this class must commit or rollback 
		/// transaction after any queue receive that returns no data (null).
		/// </summary>
		/// <returns>'true' if successful</returns>
		bool CommitTransaction();

		/// <summary>
		/// Undo get operation.  Users of this class must commit or rollback 
		/// transaction after any queue receive that returns no data (null).
		/// </summary>
		/// <returns>'true' if successful</returns>
		bool RollbackTransaction();
	}
}