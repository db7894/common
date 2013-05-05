using Bashwork.General.Validation.Internal;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// Represents a validator for the supplied object
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	public interface IValidator<TObject> : ICleanFluentInterface
	{		
		/// <summary>
		/// Method used to kick off the full type validation
		/// </summary>
		/// <param name="value">The value to validate</param>
		/// <returns>A collected validation response set</returns>
		ValidationResult Validate(TObject value);
	}
}
