using System;
using System.Linq.Expressions;
using Bashwork.General.Validation.Internal;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// Represents the current context for a complex type validation
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	public interface IValidationContext<TObject> : ICleanFluentInterface
	{
		/// <summary>
		/// Handle to the current validation options
		/// </summary>
		ValidationOptions Options { get; set; }

		/// <summary>
		/// Method used to add another validation rule to the specified context
		/// </summary>
		/// <param name="predicate">The next validation to perform on this object</param>
		/// <returns>A continued handle to the fluent interface</returns>
		IValidationContext<TObject> Obeys(Expression<Predicate<TObject>> predicate);

		/// <summary>
		/// Method used to add another validation rule to the specified context
		/// </summary>
		/// <param name="predicate">The next validation to perform on this object</param>
		/// <returns>A continued handle to the fluent interface</returns>
		IValidationContext<TObject> Obeys(PredicateContext<TObject> predicate);

		/// <summary>
		/// Method used to compile the underlying expressions and cache
		/// them for proper performance.
		/// </summary>
		/// <returns>The compiled validation context</returns>
		OperationResult<IValidator<TObject>> Compile();
	}
}
