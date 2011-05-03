using System;
using System.Linq.Expressions;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation
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
			return Property(context, (x) => x, name);
		}

		/// <summary>
		/// A helper method that sets the current validation options to the
		/// supplied options set.
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="options">The new options to validate with</param> 
		/// <returns>A continued handle to the fluent interface</returns>
		public static IValidationContext<TObject> With<TObject>(
			this IValidationContext<TObject> context, ValidationOptions options)
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

		#region Possible Future Enhancements

		//
		// The following methods will require a bit more complexity (and references)
		// to be added to the library.  The only one I can really get away with is
		// the unless / when operators which I could simply look back in the context
		// chain and wrap the previous predicate with the unless / when.
 		//
		// Any other methods to set values on the rule context if we introduce it
		// should be placed on that instead of in here maybe.
		//
		// Also, I don't know right now how I would allow for sub validators to be used
		// without them throwing.
		//

		/// <summary>
		/// A helper method that is used to retrieve access to a given property of
		/// the supplied complex type.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The property type to validate</typeparam>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="expression">The expression that indicates which property to retrieve</param>
		/// <param name="name">A name to override the property name with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		private static IPropertyContext<TObject, TPropertyElement> PropertyCollection<TObject, TPropertyElement>(
			this IValidationContext<TObject> context, Expression<Func<TObject, TPropertyElement>> expression,
			string name = null)
		{
			// TODO I need to make this treat the collection as a single element
			context.Guard(MessageResources.NotNullValidationContext);
			expression.Guard(MessageResources.NotNullExpression);
			var propertyName = name ?? ExpressionDocumenter.GetPropertyName(expression);
			return new PropertyCollectionContext<TObject, TPropertyElement>(context, propertyName, expression);
		}

		/// <summary>
		/// A helper method to set the error message for the previous validation rule
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="message">The message to return when the validation fails</param>
		/// <returns>A continued handle to the fluent interface</returns>
		private static IPropertyContext<TObject, TProperty> WithMessage<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, string message)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			// TODO, we will need a rule context to be able to do this...
			return context;
		}

		/// <summary>
		/// A helper method to only run the previous rules if the given condition is met
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="predicate">Test to check if we should run the validations</param>
		/// <returns>A continued handle to the fluent interface</returns>
		private static IPropertyContext<TObject, TProperty> When<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, Predicate<TObject> predicate)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			// TODO we will need a rule context to be able to do this...
			return context;
		}

		/// <summary>
		/// A helper method to only run the previous rules if the given condition is met
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="predicate">Test to check if we should run the validations</param>
		/// <returns>A continued handle to the fluent interface</returns>
		private static IPropertyContext<TObject, TProperty> Unless<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, Predicate<TObject> predicate)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			// TODO we will need a rule context to be able to do this...
			return When(context, (input) => !predicate(input));
		}

		#endregion
	}
}
