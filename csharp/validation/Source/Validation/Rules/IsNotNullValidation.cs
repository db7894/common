﻿using System.Linq;
using System.Collections.Generic;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input meets the specified not null / empty constraints.
	/// </summary>
	public static class IsNotNullValidation
	{
		/// <summary>
		/// Validation rule that tests if the supplied object is not null.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IValidationContext<TObject> IsNotNull<TObject>(
			this IValidationContext<TObject> context)
			where TObject : class
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property != null);
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied nullable struct is not null.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IValidationContext<TObject?> IsNotNull<TObject>(
			this IValidationContext<TObject?> context)
			where TObject : struct
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.HasValue);
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied object is not null.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsNotNull<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context)
			where TProperty : class
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property != null);
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied nullable struct is not null.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty?> IsNotNull<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty?> context)
			where TProperty : struct
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.HasValue);
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied collection is not empty.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> IsNotEmpty<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context)
		{
			return IsNotNullOrEmpty<TObject, TProperty>(context);
		}

		/// <summary>
		/// Validation rule that tests if the supplied collection is not null or empty.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> IsNotNullOrEmpty<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => (property != null) && (property.Count() > 0));
			return context;
		}
	}
}
