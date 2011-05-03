namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// Represents how the validation should cascade the
	/// the validation failures.
	/// </summary>
	public enum CascadeMode
	{
		/// <summary>
		/// Continue running all the validations after finding a
		/// validation error (default).
		/// </summary>
		Continue,

		/// <summary>
		/// Stop running the validations as soon as a single validation
		/// error has been found.
		/// </summary>
		StopOnFirstFailure,
	}

	/// <summary>
	/// Represents how the validator should return the results
	/// of its validation.
	/// </summary>
	public enum ReportingMode
	{
		/// <summary>
		/// Return the validation error results upon calling
		/// validate (default).
		/// </summary>
		ReturnValidationResults,

		/// <summary>
		/// If validation errors occurred, throw an exception
		/// containing the validation error results.
		/// </summary>
		ThrowValidationException,
	}

	/// <summary>
	/// Represents how the range comparison should include the
	/// border values.
	/// </summary>
	public enum RangeComparison
	{
		/// <summary>
		/// Include the edge values in the range comparison (default).
		/// </summary>
		Inclusive,

		/// <summary>
		/// Exclude the edge values in the range comparison.
		/// </summary>
		Exclusive,
	}
}
