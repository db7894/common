using System;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input meets the specified <see cref="String"/> constraints.
	/// </summary>
	public static class StringValidation
	{
		/// <summary>
		/// Validation that checks if the supplied input test contains the
		/// specified element text.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> Contains<TObject>(
			this IPropertyContext<TObject, string> context, string element)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.Contains(element));
			return context;
		}

		/// <summary>
		/// Validation that checks if the supplied input test does not contain
		/// the specified element text.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> DoesNotContain<TObject>(
			this IPropertyContext<TObject, string> context, string element)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => property.Contains(element).Not());
			return context;
		}

		/// <summary>
		/// Validation that checks if the supplied input test starts with
		/// the specified element text.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <param name="ignoreCase">Set to true to ignore case in the comparison</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> StartsWith<TObject>(
			this IPropertyContext<TObject, string> context, string element, bool ignoreCase=true)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => StartsWith(property, element, ignoreCase));
			return context;
		}

		/// <summary>
		/// Validation that checks if the supplied input test does not start with
		/// the specified element text.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <param name="ignoreCase">Set to true to ignore case in the comparison</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> DoesNotStartWith<TObject>(
			this IPropertyContext<TObject, string> context, string element, bool ignoreCase = true)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => StartsWith(property, element, ignoreCase).Not());
			return context;
		}

		/// <summary>
		/// Validation that checks if the supplied input test ends with
		/// the specified element text.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <param name="ignoreCase">Set to true to ignore case in the comparison</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> EndsWith<TObject>(
			this IPropertyContext<TObject, string> context, string element, bool ignoreCase = true)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => EndsWith(property, element, ignoreCase));
			return context;
		}

		/// <summary>
		/// Validation that checks if the supplied input test does not end with
		/// the specified element text.
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="context">Handle to the current property validation context</param>
		/// <param name="element">An element to check existance of in the collection</param>
		/// <param name="ignoreCase">Set to true to ignore case in the comparison</param>
		/// <returns>A continued handle to the fluent interface</returns>
		public static IPropertyContext<TObject, string> DoesNotEndWith<TObject>(
			this IPropertyContext<TObject, string> context, string element, bool ignoreCase = true)
		{
			context.Guard(MessageResources.NotNullPropertyContext);
			context.Obeys(property => EndsWith(property, element, ignoreCase).Not());
			return context;
		}

		#region Private Helper Methods

		/// <summary>
		/// A helper method that checks if the supplied property starts with the supplied
		/// element text.
		/// </summary>
		/// <param name="property">The property to check for the supplied element</param>
		/// <param name="element">The element to check the supplied property for</param>
		/// <param name="ignoreCase">true to ignore case, false to be case sensitive</param>
		/// <returns>true if property contains element, false otherwise</returns>
		private static bool StartsWith(string property, string element, bool ignoreCase)
		{
			var result = false;

			if (!string.IsNullOrEmpty(property))
			{
				result = property.StartsWith(element, (ignoreCase)
					? StringComparison.InvariantCultureIgnoreCase
					: StringComparison.InvariantCulture);
			}

			return result;
		}

		/// <summary>
		/// A helper method that checks if the supplied property ends with the supplied
		/// element text.
		/// </summary>
		/// <param name="property">The property to check for the supplied element</param>
		/// <param name="element">The element to check the supplied property for</param>
		/// <param name="ignoreCase">true to ignore case, false to be case sensitive</param>
		/// <returns>true if property contains element, false otherwise</returns>
		private static bool EndsWith(string property, string element, bool ignoreCase)
		{
			var result = false;

			if (!string.IsNullOrEmpty(property))
			{
				result = property.EndsWith(element, (ignoreCase)
					? StringComparison.InvariantCultureIgnoreCase
					: StringComparison.InvariantCulture);
			}

			return result;
		}

		#endregion
	}
}
