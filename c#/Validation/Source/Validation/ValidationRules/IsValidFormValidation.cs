using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input string is one of the available valid types (backed by regular expressions)
	/// </summary>
	public static class IsValidFormValidation
	{
		/// <summary>
		/// Validation that checks if the supplied input value
		/// matches the supplied regular expression test.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="validation">The type of form validation to perform</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> IsValid<TObject>(
			this IPropertyContext<TObject, string> context, FormValidationType validation)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => IsValidFormValue(property, validation));
			return context;
		}

		/// <summary>
		/// A helper method to check if the given input meets the supplied form
		/// validation constraint.
		/// </summary>
		/// <param name="value">The value to to validate</param>
		/// <param name="validation">The type of form validation to test for</param>
		/// <returns>true if the validation succeeded, false otherwise</returns>
		private static bool IsValidFormValue(string value, FormValidationType validation)
		{
			return FormValidationTester.ValidateInput(value, validation);
		}
	}
}
