using System;
using System.Linq.Expressions;

namespace Bashwork.Validation.Internal
{
	/// <summary>
	/// A collection of helper methods to abstract away dealing with expression tress.
	/// </summary>
	internal static class ExpressionUtility
	{
		/// <summary>
		/// The following helper method will combine a member expression and a predicate
		/// such that the extracted member will be passed to the given predicate, all wrapped
		/// in a pretty lambda.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="selector">The expression used to retrieve a property</param>
		/// <param name="predicate">The expression used to test the given property</param>
		/// <returns>The final combined expression</returns>
		public static Expression<Predicate<TObject>> CombineExpression<TObject, TProperty>(
			Expression<Func<TObject, TProperty>> selector, Expression<Predicate<TProperty>> predicate)
		{
			var expression = Expression.Invoke(predicate,
				Expression.Invoke(selector, selector.Parameters));
			return Expression.Lambda<Predicate<TObject>>(expression, selector.Parameters);
		}
	}
}
