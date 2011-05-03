using System;
using System.Linq.Expressions;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation
{
	// TODO I need to figure out what I need to do to make this work

	/// <summary>
	/// Implementation of IPropertyContext to validate a collection of the specified type.
	/// </summary>
	/// <typeparam name="TObject">The top level complex type to validate</typeparam>
	/// <typeparam name="TProperty">The property type to validate</typeparam>
	internal class PropertyCollectionContext<TObject, TProperty> : IPropertyContext<TObject, TProperty>
	{
		/// <summary>
		/// The current name of the property to be used in error messages.
		/// </summary>
		private string Name { get; set; } // TODO currently unused

		/// <summary>
		/// Handle to the original validation context that we are tied to.
		/// </summary>
		private IValidationContext<TObject> Context { get; set; }

		/// <summary>
		/// Handle to the expression that selects the requested property from the
		/// original type.
		/// </summary>
		private Expression<Func<TObject, TProperty>> PropertySelector { get; set; }

		/// <summary>
		/// Initializes a new instance of the PropertyContext class.
		/// </summary>
		/// <param name="context">Handle to the current validation context</param>
		/// <param name="name">The name of the property to validate</param>
		/// <param name="selector">The expression used to extract the requested property</param>
		public PropertyCollectionContext(IValidationContext<TObject> context,
			string name, Expression<Func<TObject, TProperty>> selector)
		{
			Context = context;
			Name = name;
			PropertySelector = selector;
		}

		/// <summary>
		/// Method used to add a validator that will chained against the
		/// specified property.
		/// </summary>
		/// <param name="validator">The validator method to set for this property</param>
		/// <returns>A continued handle to the property fluent interface</returns>
		public IPropertyContext<TObject, TProperty> SetValidator(
			Func<ValidationResult, TProperty> validator)
		{
			validator.Guard(MessageResources.NotNullExpression);
			// TODO now how do I link this...
			return this;
		}

		/// <summary>
		/// Method used to add another validation rule to the specified context.
		/// </summary>
		/// <param name="predicate">The next validation to perform on this property</param>
		/// <returns>A continued handle to the property fluent interface</returns>
		public IPropertyContext<TObject, TProperty> Obeys(Expression<Predicate<TProperty>> predicate)
		{
			predicate.Guard(MessageResources.NotNullPredicate);
			Context.Obeys(ExpressionUtility.CombineExpression(PropertySelector, predicate));
			return this;
		}

		/// <summary>
		/// Method used to return back to the main validation context interface.
		/// </summary>
		/// <returns>A continued handle to the fluent interface</returns>
		public IValidationContext<TObject> And()
		{
			// TODO should this submit all the rules to the validation context,
			// so then we can do the when clause
			return Context;
		}
	}
}
