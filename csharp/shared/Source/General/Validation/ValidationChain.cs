using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SharedAssemblies.General.Validation
{
    /// <summary>
    /// An instance of the ValidationChain is passed through each method chained
    /// together and required for fluent design pattern.
    /// </summary>
    /// <typeparam name="T">Type of the objects</typeparam>
    public sealed class ValidationChain<T>
    {
        private List<ValidationError> _validationErrors;

        /// <summary>
        /// The list of target objects currently under validation.
        /// </summary>
        public T[] Targets
        {
            get;
            set;
        }

        /// <summary>
        /// Performance timer.
        /// </summary>
        public Stopwatch ValidationTimer
        {
            get;
            private set;
        }

        /// <summary>
        /// Flag that determines if validation ends when the first validation error
        /// is uncovered.
        /// </summary>
        public bool ShouldExitOnFirstValidationError
        {
            get;
            set;
        }

        /// <summary>
        /// Used to determine which of Targets failed the validation test.
        /// </summary>
        public int ParameterTracking
        {
            get;
            set;
        }

        /// <summary>
        /// Stores one or more validation errors.
        /// </summary>
        public IEnumerable<ValidationError> ValidationErrors
        {
            get
            {
                return _validationErrors;
            }
        }

        /// <summary>
        /// Flag that determines if the ThrowOnError() method has already been called.
        /// </summary>
        public bool HasValidatedAlready
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs a validation chain and is supplied with the initial set of
        /// objects to validate.
        /// </summary>
        /// <param name="targets">Initial set of the objects to validate</param>
        public ValidationChain(params T[] targets)
        {
            Targets = targets;
            ShouldExitOnFirstValidationError = true;
            ParameterTracking = 1;
            ValidationTimer = Stopwatch.StartNew();
            HasValidatedAlready = false;
            _validationErrors = new List<ValidationError>();
        }

        /// <summary>
        /// Add a validation error to our internal list.
        /// </summary>
        /// <param name="validationError">Validation Error message</param>
        public void AddException(ValidationError validationError)
        {
            _validationErrors.Add(validationError);
        }

        /// <summary>
        /// Extension to set validation to run through all validation tests and record all
        /// exceptions.
        /// </summary>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> ReportAll()
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("ReportAll",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            ShouldExitOnFirstValidationError = false;
            return this;
        }

        /// <summary>
        /// Extension to check all objects for nullness.
        /// </summary>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> IsNotNull()
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("IsNotNull",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            if (ShouldExitOnFirstValidationError == true &&
                ValidationErrors.Count() > 0)
            {
                return this;
            }

            for (int i = 0; i < Targets.Length; i++)
            {
                if (Targets[i] == null)
                {
                    AddException(new ValidationError("IsNotNull", 
                        (i + ParameterTracking).ToString() + " parameter failed."));

                    if (ShouldExitOnFirstValidationError == true)
                    {
                        break;
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Extension to check all objects against a list.
        /// </summary>
        /// <param name="whiteList">White list that each object must belong to</param>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> IsIn(IEnumerable<T> whiteList)
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("IsIn",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            if (ShouldExitOnFirstValidationError == true &&
                ValidationErrors.Count() > 0)
            {
                return this;
            }

            if (whiteList != null && whiteList.Count() > 0)
            {
                for (int i = 0; i < Targets.Length; i++)
                {
                    if (Targets[i] == null || !whiteList.Any(p => p.Equals(Targets[i])))
                    {
                        AddException(new ValidationError("IsIn",
                            (i + ParameterTracking).ToString() + " parameter failed."));

                        if (ShouldExitOnFirstValidationError == true)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                AddException(new ValidationError("IsIn",
                    "Failed because whiteList parameter is null."));
            }

            return this;
        }

        /// <summary>
        /// Extension to check all objects against a regular expression.
        /// </summary>
        /// <param name="regex">Regular expression to match against</param>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> Matches(Regex regex)
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("Matches",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            if (ShouldExitOnFirstValidationError == true &&
                ValidationErrors.Count() > 0)
            {
                return this;
            }

            if (regex != null)
            {
                for (int i = 0; i < Targets.Length; i++)
                {
                    if (Targets[i] == null || !regex.IsMatch(Targets[i].ToString()))
                    {
                        AddException(new ValidationError("Matches",
                            (i + ParameterTracking).ToString() + " parameter failed."));

                        if (ShouldExitOnFirstValidationError == true)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                AddException(new ValidationError("Matches",
                    "Failed because regex parameter is null."));
            }

            return this;
        }

        /// <summary>
        /// Extension to check all objects against a regular expression pattern.
        /// </summary>
        /// <param name="pattern">Regular expression pattern</param>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> Matches(string pattern)
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("Matches",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            if (ShouldExitOnFirstValidationError == true &&
                ValidationErrors.Count() > 0)
            {
                return this;
            }

            if (string.IsNullOrEmpty(pattern) == false)
            {
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

                for (int i = 0; i < Targets.Length; i++)
                {
                    if (Targets[i] == null || !regex.IsMatch(Targets[i].ToString()))
                    {
                        AddException(new ValidationError("Matches",
                            (i + ParameterTracking).ToString() + " parameter failed."));

                        if (ShouldExitOnFirstValidationError == true)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                AddException(new ValidationError("Matches",
                    "Failed because pattern parameter is null."));
            }

            return this;
        }

        /// <summary>
        /// Extension to check all objects againts a rule provided by the caller.
        /// </summary>
        /// <param name="ruleBody">Rule provided by the caller</param>
		/// <param name="message">A custom message to supply on error</param>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> Obeys(Predicate<T> ruleBody, string message=null)
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("Obeys",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            if (ShouldExitOnFirstValidationError == true &&
                ValidationErrors.Count() > 0)
            {
                return this;
            }

            if (ruleBody != null)
            {
                for (int i = 0; i < Targets.Length; i++)
                {
                    if (Targets[i] == null || !ruleBody(Targets[i]))
                    {
                        AddException(new ValidationError("Obeys",
							message ?? (i + ParameterTracking).ToString() + " parameter failed."));

                        if (ShouldExitOnFirstValidationError == true)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                AddException(new ValidationError("Obeys",
                    "Failed because ruleBody parameter is null."));
            }

            return this;
        }

        /// <summary>
        /// Extension to set the list of objects that will be validated by subsequent calls.
        /// </summary>
        /// <param name="targets">New set of objects</param>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> That(params T[] targets)
        {
            if (targets == null || targets.Length == 0)
            {
                throw new ArgumentException("Missing parameters to validate!");
            }

            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("That",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            if (ShouldExitOnFirstValidationError == true &&
                ValidationErrors.Count() > 0)
            {
                return this;
            }

            Targets = targets;
            ParameterTracking++;
            return this;
        }

        /// <summary>
        /// Extension to throw a validation exception if any of the previous validation tests have failed.
        /// </summary>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> ThrowOnError()
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("ThrowOnError",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            HasValidatedAlready = true;

            if (ValidationTimer.IsRunning)
            {
                ValidationTimer.Stop();
            }

            if (ValidationErrors.Count() == 0)
            {
                return this;
            }
            else
            {
                if (ValidationErrors.Count() == 1)
                {
                    throw new ValidationException(ValidationErrors.First());
                }
                else
                {
                    throw new ValidationException(new ValidationError("ThrowOnError", 
                        "Multiple ValidationExceptions. See InnerExceptions."),
                        ValidationErrors.AsEnumerable());
                }
            }
        }

        /// <summary>
        /// Extension to throw a validation exception if any of the previous validation tests have failed. Includes
        /// the ability to capture the performance timer.
        /// </summary>
        /// <param name="durationHandler">Stopwatch handler</param>
        /// <returns>Extented ValidationChain</returns>
        public ValidationChain<T> ThrowOnError(Action<Stopwatch> durationHandler)
        {
            if (HasValidatedAlready)
            {
                throw new ValidationException(new ValidationError("ThrowOnError",
                    "Already validated. Verify that there is only one ThrowOnError() method at the end."));
            }

            HasValidatedAlready = true;

            if (ValidationErrors.Count() == 0)
            {
                if (ValidationTimer.IsRunning)
                {
                    ValidationTimer.Stop();
                }

                durationHandler(ValidationTimer);
                return this;
            }
            else
            {
                ValidationException ve = (ValidationErrors.Count() == 1) ?
                    new ValidationException(ValidationErrors.First()) :
                    new ValidationException(new ValidationError("ThrowOnError", 
                        "Multiple ValidationExceptions. See InnerExceptions."),
                        ValidationErrors);

                if (ValidationTimer.IsRunning)
                {
                    ValidationTimer.Stop();
                }

                durationHandler(ValidationTimer);
                throw ve;
            }
        }
    }
}
