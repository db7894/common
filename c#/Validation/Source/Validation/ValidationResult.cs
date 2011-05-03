using System.Collections.Generic;
using System.Linq;

namespace Bashwork.Validation
{
	/// <summary>
	/// The following is a collection of the results of a given validation
	/// </summary>
	public sealed class ValidationResult
	{
		private readonly List<ValidationFailure> _failures;

		/// <summary>
		/// Retrieves a readonly collection of the available failures
		/// </summary>
		public IEnumerable<ValidationFailure> Failures
		{
			get { return _failures.AsEnumerable(); }
		}

		/// <summary>
		/// A helper method to determine if the validation was successful
		/// </summary>
		public bool IsSuccessful
		{
			get { return (_failures.Count == 0); }
		}

		/// <summary>
		/// Initializes a new instance of the ValidationResult class
		/// </summary>
		/// <param name="failures">The list of failures to initialize with</param>
		public ValidationResult(IEnumerable<ValidationFailure> failures = null)
		{
			_failures = (failures ?? new List<ValidationFailure>()).ToList();
		}
	}
}
