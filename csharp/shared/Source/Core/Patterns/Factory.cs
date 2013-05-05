using System;
using System.Diagnostics.CodeAnalysis;

namespace SharedAssemblies.Core.Patterns
{
    /// <summary>
    /// Creates a concrete class from the assembly name and fully qualified class name.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Creates a concrete object given an assembly and concrete class name.
		/// This will throw an exception if the class could not be activated.
        /// </summary>
        /// <typeparam name="T">The type of item to dynamically create.</typeparam>
        /// <param name="assembly">Name of the assembly where the class lives.</param>
        /// <param name="concreteClass">Name of the concrete class (fully qualified).</param>
        /// <returns>Refernce to the newly created order adapter.</returns>
        public static T Create<T>(string assembly, string concreteClass)
			where T : class 
        {
            return Create<T>(assembly, concreteClass, true);
        }
        
        /// <summary>
        /// Creates a concrete object given an assembly and concrete class name.  This will
        /// either return null or throw if the object could not be activated depending on the 
        /// value of shouldThrowOnFailure.
        /// </summary>
        /// <typeparam name="T">The type of item to dynamically create.</typeparam>
        /// <param name="assembly">Name of the assembly where the class lives.</param>
        /// <param name="concreteClass">Name of the concrete class (fully qualified).</param>
        /// <param name="shouldThrowOnFailure">
		/// Throws exception if the creation fails instead of returning null.
		/// </param>
        /// <returns>Refernce to the newly created order adapter.</returns>
        /// <exception>
		/// throws any exception propogated from Activator.CreateInstance unless
		/// shouldThrowOnFailure is set to false.
		/// </exception>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "If the type can not be created for any reason, we need to catch.  " +
				"Unfortunately, this is an existing design decision and to modify it could alter " +
				"programs using this class, so this one is grandfathered in for now.")]
		public static T Create<T>(string assembly, string concreteClass, bool shouldThrowOnFailure)
			where T : class
        {
            T result = null;

            try
            {
                var handle = Activator.CreateInstance(assembly, concreteClass);

                if (handle != null)
                {
                    result = (T) handle.Unwrap();
                }
            }
            catch (Exception)
            {
                if (shouldThrowOnFailure)
                {
                    throw;
                }
            }

            return result;
        }
    }
}