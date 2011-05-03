using System;
using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.ValidationRules
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
			context.Obeys(property => property.CompareTo(DateTime.Now) > 0);
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
			context.Obeys(property => property.CompareTo(DateTime.Now) < 0);
			return context;
		}
	}
}
