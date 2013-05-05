using System.Linq;
using System.Collections.Generic;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input collection is a unique collection.
	/// </summary>
	public static class UniqueCollectionValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied collection contains values that are
		/// all unique.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> IsUnique<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => AreAllElementsUnique(property));
			return context;
		}

		/// <summary>
		/// Validation rule to test if the supplied collection contains values that are
		/// not all unique.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, IEnumerable<TProperty>> IsNotUnique<TObject, TProperty>(
			this IPropertyContext<TObject, IEnumerable<TProperty>> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => AreAllElementsUnique(property).Not());
			return context;
		}
		
		/// <summary>
		/// Helper method to check if a collection has all unique values
		/// </summary>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="property">The property to validate</param>
		/// <returns>true if the collection is unique, false otherwise</returns>
		private static bool AreAllElementsUnique<TPropertyElement>(IEnumerable<TPropertyElement> property)
		{
			return property.NullSafe().Distinct().Count()
				== property.NullSafe().Count();
		}
	}
}
