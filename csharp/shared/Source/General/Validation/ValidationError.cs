using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedAssemblies.General.Validation
{
    /// <summary>
    /// Contains the name of the method that generated the error message.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Name of method that generated error message.
        /// </summary>
        public string MethodName
        {
            get;
            set;
        }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of ValidationError.
        /// </summary>
        /// <param name="methodName">Name of method that generated error message.</param>
        /// <param name="message">Error message.</param>
        public ValidationError(string methodName, string message)
        {
            MethodName = methodName;
            Message = message;
        }
    }
}
