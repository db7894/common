using System;
using System.Collections.Generic;
using System.Linq;
using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input collection contains any of the specified elements.
	/// </summary>
	public static class ContainsAnyElementsValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied collection contains the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> ContainsAnyOf<TObject, TProperty, TPropertyElement>(
			this IPropertyContext<TObject, TProperty> context, params TPropertyElement[] elements)
			where TProperty : IEnumerable<TPropertyElement>
			where TPropertyElement : IComparable<TPropertyElement>
		{
			return ContainsAnyOf(context, elements.AsEnumerable());
		}

		/// <summary>
		/// Validation rule to test if the supplied collection does not contain the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> DoesNotContainAnyOf<TObject, TProperty, TPropertyElement>(
			this IPropertyContext<TObject, TProperty> context, params TPropertyElement[] elements)
			where TProperty : IEnumerable<TPropertyElement>
			where TPropertyElement : IComparable<TPropertyElement>
		{
			return DoesNotContainAnyOf(context, elements.AsEnumerable());
		}

		/// <summary>
		/// Validation rule to test if the supplied collection contains the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> ContainsAnyOf<TObject, TProperty, TPropertyElement>(
			this IPropertyContext<TObject, TProperty> context, IEnumerable<TPropertyElement> elements)
			where TProperty : IEnumerable<TPropertyElement>
			where TPropertyElement : IComparable<TPropertyElement>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => elements.AreAnyIn(property));
			return context;
		}

		/// <summary>
		/// Validation rule to test if the supplied collection does not contain the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> DoesNotContainAnyOf<TObject, TProperty, TPropertyElement>(
			this IPropertyContext<TObject, TProperty> context, IEnumerable<TPropertyElement> elements)
			where TProperty : IEnumerable<TPropertyElement>
			where TPropertyElement : IComparable<TPropertyElement>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => elements.AreAnyIn(property).Not());
			return context;
		}

		/// <summary>
		/// A helper method to check if any of the supplied elements exist in the supplied property
		/// </summary>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="elements">The elements to test for existance in the property</param>
		/// <param name="property">The property to validate</param>
		/// <returns>true if an element exists in property, false otherwise</returns>
		private static bool AreAnyIn<TPropertyElement>(this IEnumerable<TPropertyElement> elements,
			IEnumerable<TPropertyElement> property)
		{
			return elements.NullSafe()
				.Any(element => property.NullSafe().Contains(element));
		}
	}
}
