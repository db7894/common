using System;


namespace SharedAssemblies.General.Database.Exceptions
{
    /// <summary>
    /// A parsing exception from the queue
    /// </summary>
    [Serializable]
    public class QueueDataParsingException : ApplicationException
    {
        /// <summary>
        /// The database record in error
        /// </summary>
        private string _recordString = string.Empty;


		/// <summary>
		/// Get/set the record string
		/// </summary>
		public string RecordString
		{
			get { return _recordString; }
			set { _recordString = value; }
		}


        /// <summary>
        /// Constructs a parsing exception from queue data
        /// </summary>
        /// <param name="message">message describing exception</param>
        /// <param name="recordString">the transaction record string that could not be parsed</param>
        public QueueDataParsingException(string message, string recordString)
            : base(message)
        {
            RecordString = recordString;
        }
    }
}
