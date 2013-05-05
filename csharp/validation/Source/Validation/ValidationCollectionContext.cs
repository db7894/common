using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// Implementation of IValidationContext to validate a collection
	/// of instances of the specified type.
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	public sealed class ValidationCollectionContext<TObject> : IValidationContext<TObject>
	{
		/// <summary>
		/// The current options that control the behavior of the validation.
		/// </summary>
		public ValidationOptions Options { get; set; }

		/// <summary>
		/// The unique name of this validator used as a lookup
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The collection of validations to perform upon the input values.
		/// </summary>
		private List<PredicateContext<TObject>> Validations { get; set; }

		/// <summary>
		/// Cache of the initialized type validators
		/// </summary>
		private static readonly ValidatorTypeCache _cache = new ValidatorTypeCache();

		/// <summary>
		/// Initialize a new instance of the ValidationContext class
		/// </summary>
		/// <param name="options">The options to control the behavior of this context</param>
		public ValidationCollectionContext(ValidationOptions options = null)
		{
			Options = options ?? ValidationOptions.Default;
			Validations = new List<PredicateContext<TObject>>();
		}

		/// <summary>
		/// Method used to add another validation rule to the specified context
		/// </summary>
		/// <param name="predicate">The next validation to perform on this object</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public IValidationContext<TObject> Obeys(Expression<Predicate<TObject>> predicate)
		{
			var context = new PredicateContext<TObject> { Predicate = predicate };
			return Obeys(context);
		}

		/// <summary>
		/// Method used to add another validation rule to the specified context
		/// </summary>
		/// <param name="predicate">The next validation to perform on this object</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public IValidationContext<TObject> Obeys(PredicateContext<TObject> predicate)
		{
			predicate.Guard(MessageResources.NotNullPredicate);
			Validations.Add(predicate);
			return this;
		}

		/// <summary>
		/// Method used to compile the underlying expressions and cache
		/// them for proper performance.
		/// </summary>
		/// <returns>The compiled validation context</returns>
		public OperationResult<IValidator<TObject>> Compile()
		{
			var compiled = ExpressionCompiler.Compile(Validations);
			var validator = new CompiledValidator<TObject>(compiled, Options)
				as IValidator<TObject>;
			var success = _cache.Add<TObject>(validator, Name);
			return OperationResult.Create(success, validator);
		}
	}
}
