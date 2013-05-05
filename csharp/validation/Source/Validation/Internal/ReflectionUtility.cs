using System;
using System.Linq;
using System.Reflection;

namespace Bashwork.General.Validation.Internal
{
	/// <summary>
	/// A collection of helper methods to abstract away dealing with reflection
	/// </summary>
	internal static class ReflectionUtility
	{
		/// <summary>
		/// Helper method to find another method reflectively
		/// </summary>
		/// <typeparam name="TInput">The input parameter type of the method</typeparam>
		/// <param name="location">The location of the method</param>
		/// <param name="name">The name of the method</param>
		/// <returns>The MethodInfo handle to the method</returns>
		public static MethodInfo FindMethod<TInput>(Type location, string name)
		{
			var handle = location // so we can reflect on static types
				.GetMethods(BindingFlags.Static | BindingFlags.Public)
				.Where(method => method.Name == name).First();

			return handle.MakeGenericMethod(typeof(TInput));
		}
	}
}
