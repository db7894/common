using System.Collections.Generic;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// Implementation of IValidationContext to validate a single
	/// instance of the specified type.
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	internal sealed class CompiledValidator<TObject> : IValidator<TObject>
	{
		/// <summary>
		/// The current options that control the behavior of the validation.
		/// </summary>
		private ValidationOptions Options;

		/// <summary>
		/// The collection of validations to perform upon the input values.
		/// </summary>
		private List<CompiledValidation<TObject>> Validators;

		/// <summary>
		/// Initialize a new instance of the ValidationContext class
		/// </summary>
		/// <param name="validators">The compiled validators to use</param>
		/// <param name="options">The validation options to use</param>
		public CompiledValidator(List<CompiledValidation<TObject>> validators,
			ValidationOptions options)
		{
			validators.Guard(MessageResources.NotNullValidationContext);
			Validators = validators;
			Options = options;
		}

		/// <summary>
		/// Method used to kick off the full type validation
		/// </summary>
		/// <param name="value">The value to validate</param>
		/// <returns>A collected validation response set</returns>
		ValidationResult IValidator<TObject>.Validate(TObject value)
		{
			return ValidationProcessor.Process(Options, Validators, value);
		}
	}
}
