using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that tests if the supplied
	/// input meets the specified <see cref="int"/> constraints.
	/// </summary>
	public static class IntegerValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied input value is an even value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, int> IsEven<TObject>(
			this IPropertyContext<TObject, int> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsMultipleOf(property, 2));
			return context;
		}

		/// <summary>
		/// Validation rule to test if the supplied input value is an odd value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, int> IsOdd<TObject>(
			this IPropertyContext<TObject, int> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsMultipleOf(property, 2).Not());
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied input value is a multiple of
		/// the supplied value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="multiple">A multiple to check the input value for</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, int> IsMultipleOf<TObject>(
			this IPropertyContext<TObject, int> context, int multiple)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsMultipleOf(property, multiple));
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied input value is not a multiple of
		/// the supplied value.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="multiple">A multiple to check the input value for</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, int> IsNotMultipleOf<TObject>(
			this IPropertyContext<TObject, int> context, int multiple)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsMultipleOf(property, multiple).Not());
			return context;
		}

		/// <summary>
		/// A helper method that checks if the supplied value is a multiple
		/// of the supplied multiple.
		/// </summary>
		/// <param name="value">The value to check if is a multiple</param>
		/// <param name="multiple">The multiple to check with</param>
		/// <returns>true if value is a multiple, false otherwise</returns>
		private static bool IsMultipleOf(int value, int multiple)
		{
			return (value % multiple) == 0;
		}
	}
}
