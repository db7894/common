using System.Text.RegularExpressions;
using Bashwork.Validation.Internal;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value matches the supplied regular expression test.
	/// </summary>
	public static class RegularExpressionValidation
	{
		/// <summary>
		/// Validation that checks if the supplied input value
		/// matches the supplied regular expression test.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="regexp">The regular expression to validate with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> Matches<TObject>(
			this IPropertyContext<TObject, string> context, string regexp)
		{
			regexp.Guard(MessageResources.NotNullRegex);
			return Matches(context, new Regex(regexp));
		}

		/// <summary>
		/// Validation that checks if the supplied input value
		/// matches the supplied regular expression test.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="regexp">The regular expression to validate with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> Matches<TObject>(
			this IPropertyContext<TObject, string> context, Regex regexp)
		{
			regexp.Guard(MessageResources.NotNullRegex);
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => regexp.IsMatch(property));
			return context;
		}

		/// <summary>
		/// Validation that checks if the supplied input value
		/// does not match the supplied regular expression test.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="regexp">The regular expression to validate with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> DoesNotMatch<TObject>(
			this IPropertyContext<TObject, string> context, string regexp)
		{
			regexp.Guard(MessageResources.NotNullRegex);
			return DoesNotMatch(context, new Regex(regexp));
		}

		/// <summary>
		/// Validation that checks if the supplied input value
		/// does not match the supplied regular expression test.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="regexp">The regular expression to validate with</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> DoesNotMatch<TObject>(
			this IPropertyContext<TObject, string> context, Regex regexp)
		{
			regexp.Guard(MessageResources.NotNullRegex);
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => regexp.IsMatch(property).Not());
			return context;
		}
	}
}
