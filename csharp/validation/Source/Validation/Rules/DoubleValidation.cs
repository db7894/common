using Bashwork.General.Validation.Internal;
using Bashwork.General.Validation.Resources;

namespace Bashwork.General.Validation.Rules
{
	/// <summary>
	/// A collection of validation methods that tests if the supplied
	/// input meets the specified <see cref="double"/> constraints.
	/// </summary>
	public static class DoubleValidation
	{
		/// <summary>
		/// Validation rule to test if the supplied input property is near the supplied 
		/// test value with the supplied fuzz range.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="value">The value to check if we are near</param>
		/// <param name="fuzz">The fuzz to apply to the supplied value</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, double> IsNear<TObject>(
			this IPropertyContext<TObject, double> context, double value, double fuzz = 1.0)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsDoubleNear(property, value, fuzz));
			return context;
		}

		/// <summary>
		/// Validation rule to test if the supplied input property is not near the supplied 
		/// test value with the supplied fuzz range.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="value">The value to check if we are near</param>
		/// <param name="fuzz">The fuzz to apply to the supplied value</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, double> IsNotNear<TObject>(
			this IPropertyContext<TObject, double> context, double value, double fuzz = 1.0)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsDoubleNear(property, value, fuzz).Not());
			return context;
		}

		/// <summary>
		/// A helper method to perform a fuzzy comparison of two double numbers
		/// </summary>
		/// <param name="property">The property to check if the value is near</param>
		/// <param name="value">The value to check if we are near</param>
		/// <param name="fuzz">The fuzz to apply to the supplied value</param>
		/// <returns>true if the property is near the supplied value, false otherwise</returns>
		private static bool IsDoubleNear(double property, double value, double fuzz)
		{
			return ((property + fuzz) > value)
				&& ((property - fuzz) < value);
		}
	}
}
