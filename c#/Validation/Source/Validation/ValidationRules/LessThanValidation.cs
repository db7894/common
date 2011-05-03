using System;
using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is less than the requested test value.
	/// </summary>
	public static class LessThanValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied input property less than the
		/// supplied test value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="maximum">The maximum value to meet</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsLessThan<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context,
			TProperty maximum, RangeComparison comparison = RangeComparison.Inclusive)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsLessThan(property, maximum, comparison));
			return context;
		}

		/// <summary>
		/// Helper method that checks if the length of a collection
		/// is between the given range
		/// </summary>
		/// <param name="value">The value to test the range for</param>
		/// <param name="maximum">The maximum value to meet</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		private static bool IsLessThan<TProperty>(TProperty value, TProperty maximum,
			RangeComparison comparison)
			where TProperty : IComparable<TProperty>
		{
			return (comparison == RangeComparison.Exclusive)
				? (value.CompareTo(maximum) < 0)
				: (value.CompareTo(maximum) <= 0);
		}
	}
}
