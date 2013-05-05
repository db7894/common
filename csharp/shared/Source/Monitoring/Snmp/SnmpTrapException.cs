using System;

namespace SharedAssemblies.Monitoring.Snmp
{
    /// <summary>
    /// Exception that occurred when trying to raise an SNMP Trap
    /// </summary>
	/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
	[Serializable]
	[Obsolete("Use $/SharedAssemblies/ThirdParty/SharpSnmp instead", true)]
    public class SnmpTrapException : ApplicationException
    {
        /// <summary>
        /// Basic construction with no additional message or inner exception
        /// </summary>
        public SnmpTrapException()
        {            
        }

        /// <summary>
        /// Constructor that allows you to specify a message
        /// </summary>
        /// <param name="message">Message for the exception</param>
        public SnmpTrapException(string message) :
            base(message)
        {            
        }

        /// <summary>
        /// Constructor that allows you to specify a message and an inner exception
        /// that caused this exception to be raised.
        /// </summary>
        /// <param name="message">Message for the exception</param>
        /// <param name="innerException">Inner exception that caused this exception</param>
        public SnmpTrapException(string message, Exception innerException) :
            base(message, innerException)
        {            
        }
    }
}
