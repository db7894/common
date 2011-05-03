using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// This represents a single validation failure
	/// </summary>
	public sealed class ValidationFailure
	{
		/// <summary>
		/// The property that recieved the validation failure 
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// A message explaining why the validation failed
		/// </summary>
		public string ErrorMessage { get; set; }

		/// <summary>
		/// Override of ToString to supply our custom message
		/// </summary>
		/// <returns>A string representation of this validation failure</returns>
		public override string ToString()
		{
			return string.Format(MessageResources.ValidationErrorMessage, PropertyName, ErrorMessage);
		}
	}
}
