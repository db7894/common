
namespace Bashwork.Validation
{
	/// <summary>
	/// A collection of extra delegate types used in this library.
	/// </summary>
	public static class ValidationDelegateTypes
	{
		/// <summary>
		/// Represents a validation delegate that returns the result of a given
		/// validation.
		/// </summary>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="input">The input value to perform a validation test on</param>
		/// <returns>The result of the validation test</returns>
		public delegate ValidationFailure ValidationPredicate<in TProperty>(TProperty input);
	}
}
