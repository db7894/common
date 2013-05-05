using System;

namespace Bashwork.General.Validation.Internal
{
	/// <summary>
	/// Represents a single compiled validation step and its neccessary
	/// error verbage.
	/// </summary>
	/// <typeparam name="TObject">The object this will validate</typeparam>
	internal sealed class CompiledValidation<TObject>
	{
		/// <summary>
		/// The compiled validation check
		/// </summary>
		public Predicate<TObject> Validate;

		/// <summary>
		/// The error verbage for the supplied validator
		/// </summary>
		public ValidationFailure Verbage;
	}
}
