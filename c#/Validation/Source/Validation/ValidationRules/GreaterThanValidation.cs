using System;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is greater than the requested test value.
	/// </summary>
	public static class GreaterThanValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied input property greater than the
		/// supplied test value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="minimum">the minimum value to meet</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsGreaterThan<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, TProperty minimum,
			RangeComparison comparison = RangeComparison.Inclusive)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsGreaterThan(property, minimum, comparison));
			return context;
		}

		/// <summary>
		/// Helper method that checks if the length of a collection
		/// is between the given range
		/// </summary>
		/// <param name="value">The value to test the range for</param>
		/// <param name="minimum">the minimum value to meet</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		private static bool IsGreaterThan<TProperty>(TProperty value, TProperty minimum,
			RangeComparison comparison)
			where TProperty : IComparable<TProperty>
		{
			return (comparison == RangeComparison.Exclusive)
				? (value.CompareTo(minimum) > 0)
				: (value.CompareTo(minimum) >= 0);
		}
	}
}
