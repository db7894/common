using System.Collections.Generic;

namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// Top level factory that simply creates the initial validation context.
	/// </summary>
	public static class Validate
	{
		/// <summary>
		/// Creates an initial validation context for a single TObject.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="input">The base type to validate</param>
		/// <param name="options">The new options to validate with</param> 
		/// <returns>A new handle to the fluent interface</returns>
		public static IValidationContext<TObject> That<TObject>(TObject input,
			ValidationOptions options = null)
		{
			return new ValidationContext<TObject>(input, options);
		}

		/// <summary>
		/// Creates an initial validation context for a collection of TObject.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="input">A collection of the base type to validate</param>
		/// <param name="options">The new options to validate with</param> 
		/// <returns>A new handle to the fluent interface</returns>
		public static IValidationContext<TObject> ThatAll<TObject>(IEnumerable<TObject> input,
			ValidationOptions options = null)
		{
			return new ValidationCollectionContext<TObject>(input, options);
		}
	}
}
