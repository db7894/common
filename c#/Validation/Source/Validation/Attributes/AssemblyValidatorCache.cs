using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SharedAssemblies.General.Validation.Internal;

namespace SharedAssemblies.General.Validation.Attributes
{
	/// <summary>
	/// A class that abstracts creating a cache for all the available validation
	/// methods in a given assembly in a lazy manner.
	/// </summary>
	internal sealed class AssemblyValidatorCache
	{
		/// <summary>
		/// The assembly that this cache is for
		/// </summary>
		public Assembly Assembly { get; private set; }

		/// <summary>
		/// Handle to the dictionary of the validator method information that can be
		/// used to validate the given type.
		/// </summary>
		private readonly Lazy<IDictionary<Type, MethodInfo>> _cache;

		/// <summary>
		/// Creates a new instance of the AssemblyValidatorCache class
		/// </summary>
		/// <param name="assembly">The assembly to build a lazy cache for</param>
		public AssemblyValidatorCache(Assembly assembly)
		{
			Assembly = assembly;
			_cache = new Lazy<IDictionary<Type, MethodInfo>>(()
				=> GenerateAssemblyCache(assembly));
		}

		/// <summary>
		/// Retrieves a method info validator instance for the specified type
		/// </summary>
		/// <param name="type">The type to get a validator for</param>
		/// <returns>A tuple representing the result of the operation</returns>
		public Tuple<bool, MethodInfo> Get(Type type)
		{
			return _cache.Value.SafeGet(type);
		}

		#region Private Helper Methods

		/// <summary>
		/// Helper method to generate an assembly cache of all the possible method 
		/// validators in the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly to search in</param>
		/// <returns>A dictionary of all the possible methods in the assembly</returns>
		private static IDictionary<Type, MethodInfo> GenerateAssemblyCache(Assembly assembly)
		{
			// someone's access modifiers can't tell me what I can and can't validate with!
			const BindingFlags bindingFlags = BindingFlags.Static
				| BindingFlags.Public | BindingFlags.NonPublic;

			return GetAllValidators(assembly).SelectMany(type => type.GetMethods(bindingFlags)
					.Where(IsValidationMethod))
				.ToDictionary(GetTypeMethodValidates, method => method);
		}

		/// <summary>
		/// Helper method to retrieve the type the specified method validator
		/// validates.
		/// </summary>
		/// <param name="method">The method to retrieve the type from</param>
		/// <returns>The type the method validates</returns>
		private static Type GetTypeMethodValidates(MethodInfo method)
		{
			var attribute = typeof(ValidatorMethodAttribute);
			
			// try the attribute first, then fail back to querying the type
			var type = method.GetCustomAttributes(attribute, true)
				.Cast<ValidatorMethodAttribute>()
				.First().ValidatorType;

			return type ?? method.GetParameters().First().GetType();
		}

		/// <summary>
		/// Helper method to retrieve all the types from the given assembly
		/// that are possible validator class containers.
		/// </summary>
		/// <param name="assembly">The assembly to search in</param>
		/// <returns>A colection of the possible validator class types</returns>
		private static IEnumerable<Type> GetAllValidators(Assembly assembly)
		{
			var collection = assembly.GetTypes().Where(IsValidationClass);
			return collection;
		}

		/// <summary>
		/// Helper method to check if the specified method has been decorated with
		/// the <see cref="ValidatorMethodAttribute"/>.
		/// </summary>
		/// <param name="method">The method to check for being a validation method</param>
		/// <returns>true if successful, false otherwise</returns>
		private static bool IsValidationMethod(MethodInfo method)
		{
			var attribute = typeof(ValidatorMethodAttribute);
			return method.GetCustomAttributes(attribute, true).Any();
		}

		/// <summary>
		/// Helper method to check if the specified type has been decorated with
		/// the <see cref="ValidatorAttribute"/>.
		/// </summary>
		/// <param name="type">The type to check for being a validation class</param>
		/// <returns>true if successful, false otherwise</returns>
		private static bool IsValidationClass(Type type)
		{
			var attribute = typeof(ValidatorAttribute);
			return type.GetCustomAttributes(attribute, true).Any();
		}

		#endregion
	}
}
