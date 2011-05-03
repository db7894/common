using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation
{
	/// <summary>
	/// Implementation of IValidationContext to validate a collection
	/// of instances of the specified type.
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	public class ValidationCollectionContext<TObject> : IValidationContext<TObject>
	{
		/// <summary>
		/// The current options that control the behavior of the validation.
		/// </summary>
		public ValidationOptions Options { get; set; }

		/// <summary>
		/// The collection of values that we are to validate.
		/// </summary>
		public IEnumerable<TObject> Value { get; private set; }

		/// <summary>
		/// The collection of validations to perform upon the input values.
		/// </summary>
		private List<Expression<Predicate<TObject>>> Validations { get; set; }

		/// <summary>
		/// Initialize a new instance of the ValidationContext class
		/// </summary>
		/// <param name="input">The input collection to validate</param>
		/// <param name="options">The options to control the behavior of this context</param>
		public ValidationCollectionContext(IEnumerable<TObject> input, ValidationOptions options = null)
		{
			Value = input;
			Options = options ?? ValidationOptions.Default;
			Validations = new List<Expression<Predicate<TObject>>>();
		}

		/// <summary>
		/// Method used to add another validation rule to the specified context
		/// </summary>
		/// <param name="predicate">The next validation to perform on this object</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public IValidationContext<TObject> Obeys(Expression<Predicate<TObject>> predicate)
		{
			predicate.Guard(MessageResources.NotNullPredicate);
			Validations.Add(predicate);
			return this;
		}

		/// <summary>
		/// Method used to kick off the full type validation
		/// </summary>
		/// <returns>A collected validation response set</returns>
		public ValidationResult Validate()
		{
			return ValidationRunner.Process(Options, Validations, Value);
		}
	}
}
