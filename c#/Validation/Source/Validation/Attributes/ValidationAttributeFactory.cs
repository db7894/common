using System;
using System.Collections.Concurrent;
using System.Reflection;
using SharedAssemblies.General.Validation.Internal;
using SharedAssemblies.General.Validation.Resources;

namespace SharedAssemblies.General.Validation.Attributes
{
	/// <summary>
	/// A factory that can be used to find a validator for the given type in the current
	/// or another specified assembly. This maintains performance by caching the searches
	/// as well as the compiled method delegates.
	/// </summary>
	public static class ValidationAttributeFactory
	{
		/// <summary>
		/// Handle to the cache of possible methods for each specified assembly
		/// </summary>
		private static readonly Lazy<ConcurrentDictionary<Assembly, AssemblyValidatorCache>> _assemblyCache =
			new Lazy<ConcurrentDictionary<Assembly, AssemblyValidatorCache>>();
		
		/// <summary>
		/// Handle to the cache for the already compiled validation delegates
		/// </summary>
		 private static readonly Lazy<ConcurrentDictionary<Type, Delegate>> _methodCache
			= new Lazy<ConcurrentDictionary<Type, Delegate>>();

		/// <summary>
		/// Used to retrieve a delegate that can validate the specified type
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <returns>A delegate for the requested type validator</returns>
		public static Func<TObject, ValidationResult> GetValidator<TObject>()
		{
			return GetValidator<TObject>(Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Used to retrieve a delegate that can validate the specified type
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="assembly">The assembly to retrieve the validator from</param>
		/// <returns>A delegate for the requested type validator</returns>
		public static Func<TObject, ValidationResult> GetValidator<TObject>(Assembly assembly)
		{
			var result = (Func<TObject, ValidationResult>)_methodCache.Value.GetOrDefault(typeof(TObject));
			if (result == null)
			{
				var validator = GetAssemblyCache(assembly).Get(typeof(TObject));

				if (validator.Item1 == false)
				{
					throw new ArgumentException(MessageResources.CannotFindValidator);
				}

				result = ConvertMethodToPredicate<TObject>(validator.Item2);
			}

			return result;
		}

		#region Private Helper Methods

		/// <summary>
		/// Helper method to compile a <see cref="MethodInfo"/> into a delegate and add
		/// it too the method cache.
		/// </summary>
		/// <typeparam name="TObject">The top level complex type to validate</typeparam>
		/// <param name="method">The method to compile into a delegate</param>
		/// <returns>A delegate for the requested type validator</returns>
		private static Func<TObject, ValidationResult> ConvertMethodToPredicate<TObject>(MethodInfo method)
		{
			var result = Delegate.CreateDelegate(typeof(Func<TObject, ValidationResult>), method);
			_methodCache.Value[typeof(TObject)] = result;

			return (Func<TObject, ValidationResult>)result;
		}

		/// <summary>
		/// This either retrieves an existing assembly cache or populates one from the
		/// specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly to retrieve a method cache for</param>
		/// <returns>A populated cache for this assembly</returns>
		private static AssemblyValidatorCache GetAssemblyCache(Assembly assembly)
		{
			var result = _assemblyCache.Value.SafeGet(assembly);
			var validators = result.Item2;

			if (result.Item1 == false)
			{
				validators = new AssemblyValidatorCache(assembly);
				_assemblyCache.Value[assembly] = validators;
			}

			return validators;
		}

		#endregion
	}
}
