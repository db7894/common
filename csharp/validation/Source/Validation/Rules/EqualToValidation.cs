using System;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is equal to the test value.
	/// </summary>
	public static class EqualToValidation
	{
		/// <summary>
		/// Validation rule that tests if the supplied value matches the requested
		/// comparison value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsEqualTo<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, TProperty comparison)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.IsEqualTo(comparison));
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied value does not matche the requested
		/// comparison value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="comparison">The type of range comparison to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, TProperty> IsNotEqualTo<TObject, TProperty>(
			this IPropertyContext<TObject, TProperty> context, TProperty comparison)
			where TProperty : IComparable<TProperty>
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.IsNotEqualTo(comparison));
			return context;
		}

		#region Private Helper Methods

		/// <summary>
		/// Helper method to check if the two comparable values are equal
		/// </summary>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="left">The left value to compare</param>
		/// <param name="right">The right value to compare</param>
		/// <returns>true if equal, false otherwise</returns>
		private static bool IsEqualTo<TProperty>(this TProperty left, TProperty right)
			where TProperty : IComparable<TProperty>
		{
			return left.CompareTo(right) == 0;
		}

		/// <summary>
		/// Helper method to check if the two comparable values are not equal
		/// </summary>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="left">The left value to compare</param>
		/// <param name="right">The right value to compare</param>
		/// <returns>true if not equal, false otherwise</returns>
		private static bool IsNotEqualTo<TProperty>(this TProperty left, TProperty right)
			where TProperty : IComparable<TProperty>
		{
			return left.CompareTo(right) != 0;
		}

		#endregion
	}
}
