using System;
using System.Collections.Generic;
using System.Linq;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input collection contains the specified element.
	/// </summary>
	public static class ContainsElementValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied collection contains the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> Contains<TObject, TProperty, TPropertyElement>(
			this IPropertyContext<TObject, TProperty> context, TPropertyElement element)
			where TProperty : IEnumerable<TPropertyElement>
			where TPropertyElement : IComparable<TPropertyElement>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => element.IsIn(property));
			return context;
		}

		/// <summary>
		/// Validation rule to test if the supplied collection does not contain the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> DoesNotContain<TObject, TProperty, TPropertyElement>(
			this IPropertyContext<TObject, TProperty> context, TPropertyElement element)
			where TProperty : IEnumerable<TPropertyElement>
			where TPropertyElement : IComparable<TPropertyElement>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => element.IsIn(property).Not());
			return context;
		}

		/// <summary>
		/// A helper method to check if any of the supplied elements exist in the supplied property
		/// </summary>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="element">The element to test for existance in the property</param>
		/// <param name="property">The property to validate</param>
		/// <returns>true if an element exists in property, false otherwise</returns>
		private static bool IsIn<TPropertyElement>(this TPropertyElement element,
			IEnumerable<TPropertyElement> property)
		{
			return property.NullSafe().Contains(element);
		}
	}
}
