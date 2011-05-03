using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Bashwork.Validation.Resources;

namespace Bashwork.Validation.Internal
{
	/// <summary>
	/// A helper class to abstract away extracting error messages from
	/// expression trees.
	/// </summary>
	public static class ExpressionDocumenter
	{
		/// <summary>
		/// A regex that is used to remove the crazy generic mangled names that really
		/// don't help our error messages.
		/// </summary>
		private static readonly Regex _mangledNamesRegex =
			new Regex(@"value\([^)]+\)\.", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>
		/// A regex that is used to remove the crazy generic mangled names that really
		/// don't help our error messages.
		/// </summary>
		private static readonly Regex _pascalCaseRegex =
			new Regex(@"([A-Z])", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>
		/// Parsing method that returns the parameter name from the requested
		/// from the Expression.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="expression">The expression tree to parse</param>
		/// <returns>A string representation of the requested value</returns>
		public static string GetParameterName<TObject, TProperty>(
			Expression<Func<TObject, TProperty>> expression)
		{
			expression.Guard(MessageResources.NotNullExpression);
			var parameter = expression.Parameters.First();

			return parameter.Type.Name + " " + parameter.Name;
		}

		/// <summary>
		/// Parsing method that returns the property name that is being requested
		/// from the MemberExpression.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="expression">The expression tree to parse</param>
		/// <returns>A string representation of the requested value</returns>
		public static string GetPropertyName<TObject, TProperty>(
			Expression<Func<TObject, TProperty>> expression)
		{
			expression.Guard(MessageResources.NotNullExpression);
			return expression.Body.ToString();
		}

		/// <summary>
		/// Parsing method that returns the cleaned property name from a supplied
		/// predicate expression.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="expression">The expression tree to parse</param>
		/// <returns>A string representation of the requested value</returns>
		public static string GetPropertyName<TObject>(
			Expression<Predicate<TObject>> expression)
		{
			expression.Guard(MessageResources.NotNullExpression);

			var result = "null"; // in case of a null complex type
			var wrapper = expression.Body as InvocationExpression;

			if ((wrapper != null) && (wrapper.Arguments.Count != 0))
			{
				var parameter = wrapper.Arguments.First() as InvocationExpression;
				var selector = parameter.Expression as LambdaExpression;

				var handleName = selector.Parameters.First().Name;
				var propertyName = selector.Body.ToString();

				result = propertyName.Replace(handleName + ".", string.Empty).SplitPascalCase();
			}

			return result;
		}

		/// <summary>
		/// Parsing method that returns the validation name from a supplied
		/// predicate expression.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="expression">The expression tree to parse</param>
		/// <returns>A string representation of the requested value</returns>
		public static string GetValidationName<TObject>(
			Expression<Predicate<TObject>> expression)
		{
			expression.Guard(MessageResources.NotNullExpression);

			var result = "null"; // in case of a null complex type
			var wrapper = expression.Body as InvocationExpression;

			if (wrapper != null)
			{
				var predicate = wrapper.Expression as LambdaExpression;

				// This is probobly the wrong way to go about doing this as
				// it could get quite expensive.  I will more than likely need
				// to introduce a context container for the property if I want
				// this level of debugging...sigh
				// if (predicate.Body.NodeType == ExpressionType.Call)
				// {
				//     var inner = predicate.Body as MethodCallExpression;
				//     var parameters = inner.Arguments.Last() as MemberExpression;
				//     var constants = parameters.Expression as ConstantExpression;
				// }
				var validation = predicate.Body.ToString();

				result = _mangledNamesRegex.Replace(validation, string.Empty);
			}

			return result;
		}

		/// <summary>
		/// Parsing method that returns the expression body from the requested
		/// from the Expression.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <typeparam name="TProperty">The property type to validate</typeparam>
		/// <param name="expression">The expression tree to parse</param>
		/// <returns>A string representation of the requested value</returns>
		public static string GetPredicateValue<TObject, TProperty>(
			Expression<Func<TObject, TProperty>> expression)
		{
			expression.Guard(MessageResources.NotNullExpression);
			return expression.Body.ToString();
		}

		/// <summary>
		/// A helper method to add a space between the capital letters of the
		/// supplied input string.
		/// </summary>
		/// <param name="input">The string to split between capital letters</param>
		/// <returns>The input string split between capital letters</returns>
		private static string SplitPascalCase(this string input)
		{
			return string.IsNullOrEmpty(input)
				? input
				: _pascalCaseRegex.Replace(input, " $1").Trim();
		}
	}
}
