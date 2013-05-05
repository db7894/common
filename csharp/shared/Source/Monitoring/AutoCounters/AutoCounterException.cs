using System;

namespace SharedAssemblies.Monitoring.AutoCounters
{
    /// <summary>
    /// Exception wrapper for counter exceptions
    /// </summary>
    public class AutoCounterException : ApplicationException
    {
        /// <summary>
        /// Create a new exception when an error occurs in installing a counter
        /// </summary>
        /// <param name="message">Text message</param>
        /// <param name="inner">Cause error</param>
        public AutoCounterException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Create a new exception when an error occurs in installing a counter
        /// </summary>
        /// <param name="message">Text message</param>
        public AutoCounterException(string message)
            : base(message)
        {
        }
    }
}