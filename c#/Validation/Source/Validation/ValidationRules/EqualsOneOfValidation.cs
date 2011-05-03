using System;
using System.Collections.Generic;
using System.Linq;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is one of the supplied values.
	/// </summary>
	public static class EqualsOneOfValidation
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
		public static IPropertyContext<TObject, TProperty> EqualsOneOf<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, params TProperty[] comparison)
			where TProperty : IComparable<TProperty>
		{
			return EqualsOneOf(context, comparison.AsEnumerable());
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
		public static IPropertyContext<TObject, TProperty> DoesNotEqualOneOf<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, params TProperty[] comparison)
			where TProperty : IComparable<TProperty>
		{
			return DoesNotEqualOneOf(context, comparison.AsEnumerable());
		}

		/// <summary>
		/// Validation rule that tests if the supplied value matches the requested
		/// comparison value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> EqualsOneOf<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, IEnumerable<TProperty> comparison)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => comparison.Contains(property));
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
		public static IPropertyContext<TObject, TProperty> DoesNotEqualOneOf<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, IEnumerable<TProperty> comparison)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => comparison.Contains(property).Not());
			return context;
		}
	}
}
