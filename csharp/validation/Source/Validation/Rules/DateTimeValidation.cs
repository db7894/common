using System;
using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input meets the specified <see cref="DateTime"/> constraints.
	/// </summary>
	public static class DateTimeValidation
	{
		/// <summary>
		/// Validation rule that tests if the supplied date occurs in the future.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, DateTime> IsFuture<TObject>(
			this IPropertyContext<TObject, DateTime> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.IsInFuture());
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied date occurs in the past.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, DateTime> IsPast<TObject>(
			this IPropertyContext<TObject, DateTime> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.IsInPast());
			return context;
		}

		#region Private Helper Methods

		/// <summary>
		/// Helper method to check if the time occurs in the future
		/// </summary>
		/// <param name="value">The value to compare</param>
		/// <returns>true if in future, false otherwise</returns>
		private static bool IsInFuture(this DateTime value)
		{
			return value.CompareTo(DateTime.Now) > 0;
		}

		/// <summary>
		/// Helper method to check if the time occurs in the past
		/// </summary>
		/// <param name="value">The value to compare</param>
		/// <returns>true if in past, false otherwise</returns>
		private static bool IsInPast(this DateTime value)
		{
			return value.CompareTo(DateTime.Now) < 0;
		}

		#endregion
	}
}
