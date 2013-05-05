using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// Implementation of IPropertyContext to validate a single instance of the specified type.
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	/// <typeparam name="TProperty">The property type to validate</typeparam>
	public sealed class PropertyContext<TObject, TProperty> : IPropertyContext<TObject, TProperty>
	{
		/// <summary>
		/// The current name of the property to be used in error messages.
		/// </summary>
		private string Name;

		/// <summary>
		/// Handle to the original validation context that we are tied to.
		/// </summary>
		private IValidationContext<TObject> Context;

		/// <summary>
		/// Handle to the expression that selects the requested property
		/// from the original type.
		/// </summary>
		private Expression<Func<TObject, TProperty>> PropertySelector;

		/// <summary>
		/// The collection of validations that are to be applied to
		/// this property.
		/// </summary>
		private List<Expression<Predicate<TProperty>>> Predicates
			= new List<Expression<Predicate<TProperty>>>();

		/// <summary>
		/// Initializes a new instance of the PropertyContext class.
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="name">The name of the property to validate</param>
		/// <param name="selector">The expression used to extract the requested property</param>
		public PropertyContext(IValidationContext<TObject> context,
			string name, Expression<Func<TObject, TProperty>> selector)
		{
			Name = name;
			Context = context;
			PropertySelector = selector;
		}

		/// <summary>
		/// Method used to add another validation rule to the specified context.
		/// </summary>
		/// <param name="predicate">The next validation to perform on this property</param>
		/// <returns>A continued handle to the property fluent interface</returns>
		public IPropertyContext<TObject, TProperty> Obeys(Expression<Predicate<TProperty>> predicate)
		{
			predicate.Guard(MessageResources.NotNullPredicate);
			Predicates.Add(predicate);
			return this;
		}

		/// <summary>
		/// Method used to return back to the main validation context interface.
		/// </summary>
		/// <param name="error">An optional overloaded error message</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public IValidationContext<TObject> And(string error = null)
		{
			foreach (var predicate in Predicates)
			{
				var context = new PredicateContext<TObject>
				{
					PropertyName = Name,
					ErrorMessage = error,
					Predicate = ExpressionUtility.CombineExpression(PropertySelector, predicate),
				};
				Context.Obeys(context);
			}

			return Context;
		}
	}
}
