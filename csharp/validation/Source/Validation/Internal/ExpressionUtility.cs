using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Bashwork.General.Validation.Internal
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
			// TObject object => predicate(Invoke(type => type.TProperty, object)) => Bool
			var member = Expression.Invoke(selector, selector.Parameters);
			var expression = Expression.Invoke(predicate, member);
			return Expression.Lambda<Predicate<TObject>>(expression, selector.Parameters);
		}

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
			Expression<Func<TObject, IEnumerable<TProperty>>> selector, Expression<Predicate<TProperty>> predicate)
		{
			// TObject object => Invoke(type => type.TProperty, object).All(predicate) => Bool
			var query = ReflectionUtility.FindMethod<TProperty>(typeof(Enumerable), "All");
			var inner = Expression.Lambda<Func<TProperty, bool>>(predicate.Body, predicate.Parameters);
			var member = Expression.Invoke(selector, selector.Parameters);
			var compiled = Expression.Constant(inner.Compile()); // have to compile inner delegate
			var expression = Expression.Call(query, member, compiled);
			return Expression.Lambda<Predicate<TObject>>(expression, selector.Parameters);
		}
	}
}
