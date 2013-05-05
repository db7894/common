using System;
using System.Collections.Generic;
using System.Linq;


namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Extension methods for the Type class
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Return true if the type is a System.Nullable wrapper of a value type
		/// </summary>
		/// <param name="type">The type to check</param>
		/// <returns>True if the type is a System.Nullable wrapper</returns>
		public static bool IsNullable(this Type type)
		{
			return type.IsGenericType 
				   && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}
		 

		/// <summary>
		/// Loads the configuration from assembly attributes
		/// </summary>
		/// <typeparam name="T">The type of the custom attribute to find.</typeparam>
		/// <param name="typeWithAttributes">The calling assembly to search.</param>
		/// <returns>The custom attribute of type T, if found.</returns>
		public static T GetAttribute<T>(this Type typeWithAttributes)
			where T : Attribute
		{
			return GetAttributes<T>(typeWithAttributes).FirstOrDefault();
		}

		/// <summary>
		/// Loads the configuration from assembly attributes
		/// </summary>
		/// <typeparam name="T">The type of the custom attribute to find.</typeparam>
		/// <param name="typeWithAttributes">The calling assembly to search.</param>
		/// <returns>An enumeration of attributes of type T that were found.</returns>
		public static IEnumerable<T> GetAttributes<T>(this Type typeWithAttributes)
			where T : Attribute
		{
			// Try to find the configuration attribute for the default logger if it exists
			object[] configAttributes = Attribute.GetCustomAttributes(typeWithAttributes,
				typeof(T), false);

			// get just the first one
			if (configAttributes != null && configAttributes.Length > 0)
			{
				foreach (T attribute in configAttributes)
				{
					yield return attribute;
				}
			}
		}
	}
}