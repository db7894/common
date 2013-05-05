using System;
using System.Data.Common;


namespace SharedAssemblies.General.Database.Exceptions
{
	/// <summary>
	/// Domain level exception to be used in data access layer code
	/// </summary>
	[Serializable]
	public class DataAccessException : DbException
	{
		/// <summary>
		/// Initialzes a new instance of the DataAccessException
		/// </summary>
		/// <param name="message">The message to attach to the exception</param>
		public DataAccessException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initialzes a new instance of the DataAccessException
		/// </summary>
		/// <param name="message">The message to attach to the exception</param>
		/// <param name="inner">The inner exception to propigate up</param>
		public DataAccessException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
