using System;
using System.Linq.Expressions;
using SharedAssemblies.General.Validation.Internal;

namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// Represents the current context for a complex type's property validation
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	/// <typeparam name="TProperty">The property type to validate</typeparam>
	public interface IPropertyContext<TObject, TProperty> : ICleanFluentInterface
	{
		/// <summary>
		/// Method used to add a validator that will chained against the
		/// specified property.
		/// </summary>
		/// <param name="validator">The validator method to set for this property</param>
		/// <returns>A continued handle to the property fluent interface</returns>
		IPropertyContext<TObject, TProperty> SetValidator(Func<ValidationResult, TProperty> validator);

		/// <summary>
		/// Method used to add another validation rule to the specified context
		/// </summary>
		/// <param name="predicate">The next validation to perform on this property</param>
		/// <returns>A continued handle to the property fluent interface</returns>
		IPropertyContext<TObject, TProperty> Obeys(Expression<Predicate<TProperty>> predicate);

		/// <summary>
		/// Method used to return back to the main validation context interface
		/// </summary>
		/// <returns>A continued handle to the fluent interface</returns>
		IValidationContext<TObject> And();
	}
}
