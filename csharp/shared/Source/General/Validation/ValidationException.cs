using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedAssemblies.General.Validation
{
    /// <summary>
    /// The exception that is thrown when one or more objects provided by a method 
    /// are invalid.
    /// </summary>
    [Serializable()]
    public class ValidationException : Exception
    {
        private ValidationError[] _validationErrors = null;

        /// <summary>
        /// Contains all exceptions if multiple validation tests have failed.
        /// </summary>
        public IEnumerable<ValidationError> ValicationErrors
        {
            get
            {
                if (_validationErrors != null)
                {
                    for (int i = 0; i < _validationErrors.Length; ++i)
                    {
                        yield return _validationErrors[i];
                    }
                }
            }
        }

        /// <summary>
        /// Holds the ValidationError.
        /// </summary>
        public ValidationError Error
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of ValidationException.
        /// </summary>
        public ValidationException() : base() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class with a specified error message.
        /// </summary>
        /// <param name="error">Validation Error</param>
        public ValidationException(ValidationError error)
            : base(error.Message) 
        {
            Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="error">Validation Error</param>
        /// <param name="inner">Inner Validation Error</param>
        public ValidationException(ValidationError error, ValidationError inner)
            : base(error.Message, new ValidationException(inner)) 
        {
            Error = error;
            _validationErrors = new ValidationError[] { inner };
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class with a specified error message 
        /// and a reference to the multiple inner exceptions that is the cause of this exception.
        /// </summary>
        /// <param name="error">Validation Error</param>
        /// <param name="innerExceptions">Inner Validation Errors: one or more</param>
        public ValidationException(ValidationError error, IEnumerable<ValidationError> innerExceptions)
            : base(error.Message, new ValidationException(innerExceptions.FirstOrDefault()))
        {
            Error = error;

            foreach (ValidationError ve in innerExceptions)
            {
                if (ve == null)
                {
                    throw new ArgumentNullException("innerExceptions");
                }
            }

            _validationErrors = innerExceptions.ToArray();
        }
    }
}
