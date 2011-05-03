using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SharedAssemblies.General.Validation.Internal;

namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// Helper class that abstracts away the iteration and validation of the
	/// supplied value(s) with the supplied validation rule(s).
	/// </summary>
	public static class ValidationRunner
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="options">The options to control the behavior of this context</param>
		/// <param name="validations">The collection of validations to perform</param>
		/// <param name="value">The value to process the validations against</param>
		/// <returns></returns>
		public static ValidationResult Process<TObject>(ValidationOptions options,
			IEnumerable<Expression<Predicate<TObject>>> validations, TObject value)
		{
			var exceptions = new List<ValidationFailure>();

			foreach (var validation in validations)
			{
				var result = PerformSingleValidation(options, validation, value);
				if (result != null)
				{
					exceptions.Add(result);
					if (options.CascadeBehavior == CascadeMode.StopOnFirstFailure)
					{
						break;
					}
				}
			}

			if (options.ReportingBehavior == ReportingMode.ThrowValidationException)
			{
				throw new ValidationException(exceptions);
			}
			return new ValidationResult(exceptions);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="options">The options to control the behavior of this context</param>
		/// <param name="validations">The collection of validations to perform</param>
		/// <param name="value">The value to process the validations against</param>
		/// <returns></returns>
		public static ValidationResult Process<TObject>(ValidationOptions options,
			IEnumerable<Expression<Predicate<TObject>>> validations, IEnumerable<TObject> value)
		{
			var exceptions = new List<ValidationFailure>();

			foreach (var validation in validations)
			{
				var result = PerformSingleValidation(options, validation, value);
				if (result != null)
				{
					exceptions.AddRange(result);
				}
			}

			if (options.ReportingBehavior == ReportingMode.ThrowValidationException)
			{
				throw new ValidationException(exceptions);
			}
			return new ValidationResult(exceptions);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="options">The options to control the behavior of this context</param>
		/// <param name="validation">The single validation to perform</param>
		/// <param name="value">The value to validate against the given validation rule</param>
		/// <returns></returns>
		private static ValidationFailure PerformSingleValidation<TObject>(
			ValidationOptions options,
			Expression<Predicate<TObject>> validation, TObject value)

		{
			ValidationFailure result = null;

			if (!validation.Compile()(value))
			{
				result = GenerateValidationFailure(validation, options.ReportErrors);
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="options">The options to control the behavior of this context</param>
		/// <param name="validation">The single validation to perform</param>
		/// <param name="values">The values to validate against the given validation rule</param>
		/// <returns></returns>
		private static IEnumerable<ValidationFailure> PerformSingleValidation<TObject>(
			ValidationOptions options,
			Expression<Predicate<TObject>> validation, IEnumerable<TObject> values)
		{
			var results = new List<ValidationFailure>();
			var expression = validation.Compile();

			foreach (var value in values)
			{
				if (expression(value))
				{
					results.Add(GenerateValidationFailure(validation, options.ReportErrors));

					if (options.CascadeBehavior == CascadeMode.StopOnFirstFailure)
					{
						break;
					}
				}
			}

			return results;
		}

		/// <summary>
		/// Helper method to generate a populated <see cref="ValidationFailure"/>
		/// given a failed validation.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="validation">The validation to create an error message for</param>
		/// <param name="verbose">Flag that indicates if we should generate the error messages</param>
		/// <returns>A populated <see cref="ValidationFailure"/> for the given expression</returns>
		private static ValidationFailure GenerateValidationFailure<TObject>(
			Expression<Predicate<TObject>> validation, bool verbose)
		{
			return (!verbose) ? null : new ValidationFailure
			{
				// TODO overload for the two
				PropertyName = ExpressionDocumenter.GetPropertyName(validation),
				ErrorMessage = ExpressionDocumenter.GetValidationName(validation),
			};
		}
	}
}
