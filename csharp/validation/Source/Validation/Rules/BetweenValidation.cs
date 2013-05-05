using System;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is between a set of test values.
	/// </summary>
	public static class BetweenValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied input property is between the
		/// supplied test values.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="minimum">the minimum value to meet</param>
		/// <param name="maximum">The maximum value to meet</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsBetween<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context,
			TProperty minimum, TProperty maximum,
			RangeComparison comparison = RangeComparison.Inclusive)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsValueBetween(property, minimum, maximum, comparison));
			return context;
		}

		/// <summary>
		/// Validation rule to test if the supplied input property is not between the
		/// supplied test values.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="minimum">the minimum value to meet</param>
		/// <param name="maximum">The maximum value to meet</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsNotBetween<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context,
			TProperty minimum, TProperty maximum,
			RangeComparison comparison = RangeComparison.Inclusive)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsValueBetween(property, minimum, maximum, comparison).Not());
			return context;
		}

		/// <summary>
		/// Helper method that checks if the length of a collection
		/// is between the given range
		/// </summary>
		/// <param name="value">The value to test the range for</param>
		/// <param name="minimum">the minimum value to meet</param>
		/// <param name="maximum">The maximum value to meet</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		private static bool IsValueBetween<TProperty>(TProperty value, TProperty minimum,
			TProperty maximum, RangeComparison comparison)
			where TProperty : IComparable<TProperty>
		{
			return (comparison == RangeComparison.Exclusive)
				? (value.CompareTo(minimum) >  0) && (value.CompareTo(maximum) <  0)
				: (value.CompareTo(minimum) >= 0) && (value.CompareTo(maximum) <= 0);
		}
	}
}
