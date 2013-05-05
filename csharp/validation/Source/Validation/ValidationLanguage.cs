using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// A collection of extensions to the fluent interface to make the
	/// DSL a little closer to english.
	/// </summary>
	public static class ValidationLanguage
	{
		/// <summary>
		/// A helper method that is used to retrieve access to a given property of
		/// the supplied complex type.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="expression">The expression that indicates which property to retrieve</param>
		/// <param name="name">A name to override the property name with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> Property<TObject, TProperty>(
			this IValidationContext<TObject> context, Expression<Func<TObject, TProperty>> expression,
			string name = null)
		{
			context.Guard(MessageResources.NotNullValidationContext);
			expression.Guard(MessageResources.NotNullExpression);
			var propertyName = name ?? ExpressionDocumenter.GetPropertyName(expression);
			return new PropertyContext<TObject, TProperty>(context, propertyName, expression);
		}

		/// <summary>
		/// A helper method that is used to retrieve access to property context of the
		/// main complex type (kind of hacky I guess).
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="name">A name to override the property name with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TObject> This<TObject>(
			this IValidationContext<TObject> context, string name = null)
		{
			context.Guard(MessageResources.NotNullValidationContext);
			var propertyName = name ?? typeof(TObject).ToString();
			return Property(context, (x) => x, propertyName);
		}
		
		/// <summary>
		/// A helper method that is used to retrieve access to a given property of
		/// the supplied complex type.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TElement">The property type to validate</typeparam>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="expression">The expression that indicates which property to retrieve</param>
		/// <param name="name">A name to override the property name with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TElement> EachItem<TObject, TElement>(
			this IValidationContext<TObject> context,
			Expression<Func<TObject, IEnumerable<TElement>>> expression, string name = null)
		{
			context.Guard(MessageResources.NotNullValidationContext);
			expression.Guard(MessageResources.NotNullExpression);
			var propertyName = name ?? ExpressionDocumenter.GetPropertyName(expression);
			return new PropertyCollectionContext<TObject, TElement>(context, propertyName, expression);
		}

		/// <summary>
		/// A helper method that sets the current validation options to the
		/// supplied options set.
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="options">The new options to validate with</param> 
		/// <returns>A continued handle to the fluent interface</returns>
		public static IValidationContext<TObject> With<TObject>(
			this IValidationContext<TObject> context, ValidationOptions options = null)
		{
			context.Guard(MessageResources.NotNullValidationContext);
			context.Options = options ?? ValidationOptions.Default;
			return context;
		}
		
		/// <summary>
		/// A helper method to set the validation option to stop validating
		/// as soon as the first validation failure occurs.
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IValidationContext<TObject> StopOnFirstFailure<TObject>(
			this IValidationContext<TObject> context)
		{
			context.Guard(MessageResources.NotNullValidationContext);
			context.Options.CascadeBehavior = CascadeMode.StopOnFirstFailure;
			return context;
		}

		/// <summary>
		/// A helper method to set the validation option to throw an
		/// exception on a validation failure.
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IValidationContext<TObject> ThrowOnFailure<TObject>(
			this IValidationContext<TObject> context)
		{
			context.Guard(MessageResources.NotNullValidationContext);
			context.Options.ReportingBehavior = ReportingMode.ThrowValidationException;
			return context;
		}
	}
}
