using System.Linq;
using System.Collections.Generic;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// The following is a collection of the results of a given validation
	/// </summary>
	public sealed class ValidationResult
	{
		private readonly bool _isSuccessful;
		private readonly IEnumerable<ValidationFailure> _failures;
		private static readonly IEnumerable<ValidationFailure> _emptyFailures
			= Enumerable.Empty<ValidationFailure>().ToList();

		/// <summary>
		/// Retrieves a readonly collection of the available failures
		/// </summary>
		public IEnumerable<ValidationFailure> Failures
		{
			get { return _failures.AsEnumerable(); }
		}

		/// <summary>
		/// A flag that indicates if the validation was successful
		/// </summary>
		public bool IsSuccessful { get { return _isSuccessful; } }

		/// <summary>
		/// Initializes a new instance of the ValidationResult class
		/// </summary>
		/// <param name="failures">The list of failures to initialize with</param>
		public ValidationResult(IEnumerable<ValidationFailure> failures = null)
		{

			_failures = failures ?? _emptyFailures;
			_isSuccessful = (failures == null) || (_failures.Count() == 0);
		}
	}
}
