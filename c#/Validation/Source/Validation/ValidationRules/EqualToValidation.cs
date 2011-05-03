using System;
using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is equal to the test value.
	/// </summary>
	public static class EqualToValidation
	{
		/// <summary>
		/// Validation rule that tests if the supplied value matches the requested
		/// comparison value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsEqualTo<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, TProperty comparison)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.CompareTo(comparison) == 0);
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied value does not matche the requested
		/// comparison value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsNotEqualTo<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, TProperty comparison)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.CompareTo(comparison) != 0);
			return context;
		}
	}
}
