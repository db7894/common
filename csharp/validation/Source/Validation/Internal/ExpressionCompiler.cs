using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Bashwork.General.Validation.Internal
{
	/// <summary>
	/// Helper class that compiles the expressions so that the library can
	/// actually be performant.
	/// </summary>
	internal static class ExpressionCompiler
	{
		/// <summary>
		/// Compiles a collection of validators
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="contexts">The collection of validations to perform</param>
		/// <returns>The compiled validators</returns>
		public static List<CompiledValidation<TObject>> Compile<TObject>(
			IEnumerable<PredicateContext<TObject>> contexts)
		{
			return contexts.NullSafe().Select(Compile).ToList();
		}

		/// <summary>
		/// Compiles a single validator
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">The single validation to perform</param>
		/// <returns>The compiled validator</returns>
		public static CompiledValidation<TObject> Compile<TObject>(
			PredicateContext<TObject> context)
		{
			return new CompiledValidation<TObject>
			{
				Validate = context.Predicate.Compile(),
				Verbage = CompileVerbage(context),
			};
		}

		/// <summary>
		/// Helper method to generate a populated <see cref="ValidationFailure"/>
		/// given a failed validation.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">The validation to create an error message for</param>
		/// <returns>A populated <see cref="ValidationFailure"/> for the given expression</returns>
		private static ValidationFailure CompileVerbage<TObject>(
			PredicateContext<TObject> context)
		{
			var predicate = context.Predicate;
			var property  = context.PropertyName ?? ExpressionDocumenter.GetPropertyName(predicate);

			return new ValidationFailure
			{
				PropertyName = property,
				ErrorMessage = ExpressionDocumenter.GetValidationName(predicate),
				CustomMessage = context.ErrorMessage,
			};
		}
	}
}
