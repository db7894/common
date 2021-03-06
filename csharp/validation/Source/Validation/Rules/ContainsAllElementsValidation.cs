﻿using System;
using System.Linq;
using System.Collections.Generic;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input collection contains all of the specified elements.
	/// </summary>
	public static class ContainsAllElementsValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied collection contains the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> ContainsAllOf<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context, params TProperty[] elements)
			where TProperty : IComparable<TProperty>
		{
			return ContainsAllOf(context, elements.AsEnumerable());
		}

		/// <summary>
		/// Validation rule to test if the supplied collection does not contain the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> DoesNotContainAllOf<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context, params TProperty[] elements)
			where TProperty : IComparable<TProperty>
		{
			return DoesNotContainAllOf(context, elements.AsEnumerable());
		}

		/// <summary>
		/// Validation rule to test if the supplied collection contains the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> ContainsAllOf<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context, IEnumerable<TProperty> elements)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => elements.AreAllIn(property));
			return context;
		}

		/// <summary>
		/// Validation rule to test if the supplied collection does not contain the specified value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="elements">A collection of elements to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> DoesNotContainAllOf<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context, IEnumerable<TProperty> elements)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => elements.AreAllIn(property).Not());
			return context;
		}

		/// <summary>
		/// A helper method to check if any of the supplied elements exist in the supplied property
		/// </summary>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="elements">The elements to test for existance in the property</param>
		/// <param name="property">The property to validate</param>
		/// <returns>true if an element exists in property, false otherwise</returns>
		private static bool AreAllIn<TPropertyElement>(this IEnumerable<TPropertyElement> elements,
			IEnumerable<TPropertyElement> property)
			where TPropertyElement : IComparable<TPropertyElement>
		{
			return elements.NullSafe()
				.All(element => property.NullSafe().Contains(element));
		}
	}
}
