
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that tests if the supplied
	/// input meets the specified <see cref="bool"/> constraints.
	/// </summary>
	public static class BoolValidation
	{
		/// <summary>
		/// Validation that tests if the supplied boolean is true.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, bool> IsTrue<TObject>(
			this IPropertyContext<TObject, bool> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property == true);
			return context;
		}

		/// <summary>
		/// Validation that tests if the supplied boolean is false.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, bool> IsFalse<TObject>(
			this IPropertyContext<TObject, bool> context)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property == false);
			return context;
		}
	}
}
