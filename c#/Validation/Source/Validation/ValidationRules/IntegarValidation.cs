using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that tests if the supplied
	/// input meets the specified <see cref="int"/> constraints.
	/// </summary>
	public static class IntegarValidation
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
			context.Obeys(property => (property % 2) == 0);
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
			context.Obeys(property => (property % 2) != 0);
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
			context.Obeys(property => (property % multiple) == 0);
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
			context.Obeys(property => (property % multiple) != 0);
			return context;
		}
	}
}
