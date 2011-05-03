using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that tests if the supplied
	/// input meets the specified <see cref="char"/> constraints.
	/// </summary>
	public static class CharacterValidation
	{
		/// <summary>
		/// Validation rule that tests if the supplied character is within the allowable
		/// ASCII printable character range.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, char> IsPrintable<TObject>(
			this IPropertyContext<TObject, char> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => (property > ' ') && (property < '~'));
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied character is uppercase
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, char> IsUppercase<TObject>(
			this IPropertyContext<TObject, char> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => char.IsUpper(property));
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied character is lowercase
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, char> IsLowercase<TObject>(
			this IPropertyContext<TObject, char> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => char.IsLower(property));
			return context;
		}

		/// <summary>
		/// Validation rule that tests if the supplied character is a digit from 0 to 9.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, char> IsDigit<TObject>(
			this IPropertyContext<TObject, char> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => char.IsDigit(property));
			return context;
		}
	}
}
