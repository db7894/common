using System.Collections.Generic;
using System.Linq;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input collection has the specified length.
	/// </summary>
	public static class LengthValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied collection has the specified length
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="minimum">the minimum length of the collection (default uint.MinValue)</param>
		/// <param name="maximum">The maximum length of the collection (default uint.MaxValue)</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> HasLength<TObject, TProperty, TPropertyElement>(
			this IPropertyContext<TObject, TProperty> context,
			uint minimum = uint.MinValue, uint maximum = uint.MaxValue,
			RangeComparison comparison = RangeComparison.Inclusive)
			where TProperty : class, IEnumerable<TPropertyElement>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsLengthBetween(property, minimum, maximum, comparison));
			return context;
		}

		/// <summary>
		/// Helper method that checks if the length of a collection
		/// is between the given range
		/// </summary>
		/// <typeparam name="TPropertyElement">The type of element in the collection</typeparam>
		/// <param name="collection">The collection to validate the length of</param>
		/// <param name="minimum">the minimum length of the collection (default uint.MinValue)</param>
		/// <param name="maximum">The maximum length of the collection (default uint.MaxValue)</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>true if the length is in range, false otherwise</returns>
		private static bool IsLengthBetween<TPropertyElement>(
			IEnumerable<TPropertyElement> collection,
			uint minimum, uint maximum, RangeComparison comparison)
		{
			var length = collection.NullSafe().Count();

			return (comparison == RangeComparison.Exclusive)
				? (length > minimum) && (length < maximum)
				: (length >= minimum) && (length <= maximum);
		}
	}
}
