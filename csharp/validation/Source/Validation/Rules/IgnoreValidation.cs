using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that can be used to document
	/// the absense of validations on a property.
	/// </summary>
	public static class IgnoreValidation
	{
		/// <summary>
		/// Validation rule that can be used to document that a property is not being validated.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IValidationContext<TObject> IgnoresValidation<TObject>(
			this IValidationContext<TObject> context)
		{
			context.Guard(MessageResources.NotNullValidationContext);
			return context;
		}

		/// <summary>
		/// Validation rule that can be used to document that a property is not being validated.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IgnoresValidation<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			return context;
		}
	}
}
