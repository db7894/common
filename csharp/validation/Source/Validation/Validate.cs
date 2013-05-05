using System;
using System.Collections.Generic;
using Bashwork.General.Validation.Internal;

namespace Bashwork.General.Validation
{
	/// <summary>
	/// Top level factory that simply creates the initial validation context.
	/// </summary>
	public static class Validate
	{
		/// <summary>
		/// Cache of the initialized type validators
		/// </summary>
		private static readonly ValidatorTypeCache _cache = new ValidatorTypeCache();

		/// <summary>
		/// Creates an initial validation context for a single TObject.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="name">An overloaded name for the validator</param>
		/// <param name="options">The new options to validate with</param> 
		/// <returns>A new handle to the fluent interface</returns>
		public static IValidationContext<TObject> That<TObject>(
			string name = null, ValidationOptions options = null)
		{
			return new ValidationContext<TObject>(options)
			{
				Name = name,
			};
		}

		/// <summary>
		/// Creates an initial validation context for a collection of TObject.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="name">An overloaded name for the validator</param>
		/// <param name="options">The new options to validate with</param> 
		/// <returns>A new handle to the fluent interface</returns>
		public static IValidationContext<TObject> ThatAll<TObject>(
			string name = null, ValidationOptions options = null)
		{
			return new ValidationCollectionContext<TObject>(options)
			{
				Name = name,
			};
		}
	}
}
