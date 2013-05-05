using System.Diagnostics.CodeAnalysis;

namespace SharedAssemblies.Core.Patterns
{
    /// <summary>
    /// Second form of LazySingleton where the true class type and the representative
    /// as type can be separate (for instance, if you want a true type to be a class
    /// and the as type be an interface instead.
    /// </summary>
    /// <typeparam name="T">True type of singleton</typeparam>
    public static class Singleton<T> where T : new()
    {
		/// <summary>
		/// Returns the singleton instance using lazy-instantiation
		/// </summary>
		public static T Instance
		{
			get { return LazySingleton.InsideInstance; }
		}

		/// <summary>
        /// Lazy Singleton inner class so loads once in a thread safe manner
        /// </summary>
        private static class LazySingleton
        {
            /// <summary>the singleton instance</summary>
            internal static readonly T InsideInstance = new T();

            /// <summary>
            /// static constructor for the singleton to keep lazy
            /// </summary>
            [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", 
                "SA1409:RemoveUnnecessaryCode",
                Justification = "Necessary to have empty static constructor for lazy singleton.")]
            static LazySingleton()
            {
                // explicit static constructor to tell C# compiler not to mark 
                // type as beforefieldinit
            }
        }
    }
}