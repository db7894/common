using System.Collections.Generic;

namespace Bashwork.General.Validation.Internal
{
	/// <summary>
	/// Helper class that abstracts away the iteration and validation of the
	/// supplied value(s) with the supplied validation rule(s).
	/// </summary>
	internal static class ValidationProcessor
	{
		/// <summary>
		/// Singleton to reduce object creation for the happy path
		/// </summary>
		private static readonly ValidationResult _successful = new ValidationResult();

		/// <summary>
		/// Process a given value against a number of validaton checks and
		/// return the results.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="options">The options to control the behavior of this context</param>
		/// <param name="validations">The collection of validations to perform</param>
		/// <param name="value">The value to process the validations against</param>
		/// <returns>The result of the validation</returns>
		public static ValidationResult Process<TObject>(ValidationOptions options,
			List<CompiledValidation<TObject>> validations, TObject value)
		{
			List<ValidationFailure> exceptions = null;

			for (int i = 0; i < validations.Count; ++i)
			{
				var validation = validations[i];
				if (!validation.Validate(value))
				{
					exceptions = exceptions ?? new List<ValidationFailure>();
					exceptions.Add(validation.Verbage);
					if (options.CascadeBehavior == CascadeMode.StopOnFirstFailure)
					{
						break;
					}
				}
			}

			if (   (exceptions != null) && (exceptions.Count > 0)
				&& (options.ReportingBehavior == ReportingMode.ThrowValidationException))
			{
				throw new ValidationException(exceptions);
			}

			return (exceptions == null)
				? _successful : new ValidationResult(exceptions);
		}

		/// <summary>
		/// Process a collection of values against a number of validaton checks and
		/// return the results.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="options">The options to control the behavior of this context</param>
		/// <param name="validations">The collection of validations to perform</param>
		/// <param name="values">The values to process the validations against</param>
		/// <returns>The result of the validation</returns>
		public static ValidationResult Process<TObject>(ValidationOptions options,
			List<CompiledValidation<TObject>> validations, IEnumerable<TObject> values)
		{
			List<ValidationFailure> exceptions = null;

			for (int i = 0; i < validations.Count; ++i)
			{
				var validation = validations[i];
				foreach (var value in values)
				{
					if (!validation.Validate(value))
					{
						exceptions = exceptions ?? new List<ValidationFailure>();
						exceptions.Add(validation.Verbage);
						if (options.CascadeBehavior == CascadeMode.StopOnFirstFailure)
						{
							break;
						}
					}
				}
			}

			if (   (exceptions != null) && (exceptions.Count > 0)
				&& (options.ReportingBehavior == ReportingMode.ThrowValidationException))
			{
				throw new ValidationException(exceptions);
			}

			return (exceptions == null)
				? _successful : new ValidationResult(exceptions);
		}
	}
}
