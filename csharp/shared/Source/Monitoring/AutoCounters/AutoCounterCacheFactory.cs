using System.Collections.Concurrent;
using System.Reflection;

using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;

namespace SharedAssemblies.Monitoring.AutoCounters
{
    /// <summary>
    /// <para>
    /// Constructs an AutoCounterCache instance appropriate for the given assembly 
    /// by analyzing the assembly level AutoCounter attributes.
    /// </para>
    /// <para>
    /// This cache factory maintains a cache singleton for each assembly so that 
    /// they can be uniformly constructed without worry of redundancy.
    /// </para>
    /// </summary>
    public static class AutoCounterCacheFactory
    {
        /// <summary>the cache of autocounters per assembly</summary>
        private static readonly ConcurrentDictionary<Assembly, ICounterCache> _counterCache
			= new ConcurrentDictionary<Assembly, ICounterCache>();

        /// <summary>The cache of create failed settings per assembly</summary>
		private static readonly ConcurrentDictionary<Assembly, CreateFailedAction> _createFailedCache
			= new ConcurrentDictionary<Assembly, CreateFailedAction>();

		/// <summary>
		/// Maps an assembly to the non-locking attribute for that assembly.  A null attribute
		/// indicates that the counters for that assembly should use the default locking behavior.
		/// </summary>
		private static readonly ConcurrentDictionary<Assembly, NonLockingAutoCountersAttribute> _nonLockingCounterFlushCache
			= new ConcurrentDictionary<Assembly, NonLockingAutoCountersAttribute>();

        /// <summary>
        /// Gets an auto counter cache relevant to the assembly that makes the call to GetCache().
        /// </summary>
        /// <returns>An ICounterCache implementation instance.</returns>
        public static ICounterCache GetCache()
        {
            return GetCache(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets an auto counter cache relevant to the target assembly given.
        /// </summary>
        /// <param name="targetAssembly">Assembly to check for AutoCounter attributes.</param>
        /// <returns>An ICounterCache implementation instance.</returns>
        public static ICounterCache GetCache(Assembly targetAssembly)
        {
			return _counterCache.GetOrAdd(targetAssembly, assem =>
			{
				var defaultAction = GetDefaultCreateFailedAction(assem);
				var interval = GetUseNonLockingOption(assem);
				var registry = AutoCounterAssemblyLoader.LoadCounterRegistry(assem, defaultAction, interval);
	
				return new AutoCounterCache(registry, interval);
			});
        }

        /// <summary>
        /// Gets the default action to take if creating an AutoCounter fails as specified by
        /// the AutoCounterCreateFailedDefaultActionAttribute in the assembly that is
        /// calling this method.
        /// </summary>
        /// <returns>The default action to take if creating an auto counter fails.</returns>
        public static CreateFailedAction GetDefaultCreateFailedAction()
        {
            return GetDefaultCreateFailedAction(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets the default action to take if creating an AutoCounter fails as specified by
        /// the AutoCounterCreateFailedDefaultActionAttribute in the assembly specified.
        /// </summary>
        /// <param name="targetAssembly">The assembly to check</param>
        /// <returns>The default action to take if creating an auto counter fails.</returns>
        public static CreateFailedAction GetDefaultCreateFailedAction(Assembly targetAssembly)
        {
			return _createFailedCache.GetOrAdd(targetAssembly, 
										AutoCounterAssemblyLoader.LoadDefaultCreateFailedAction );
        }

		/// <summary>
		/// Gets the time interval between updates to non-locking counters. A null result
		/// indicates that the counters for the assembly should use the default locking
		/// behavior.
		/// </summary>
		/// <param name="targetAssembly">Assembly to examine</param>
		/// <returns>Time interval information for non-locking counters, or null</returns>
		public static NonLockingAutoCountersAttribute GetUseNonLockingOption(Assembly targetAssembly)
		{
			return _nonLockingCounterFlushCache.GetOrAdd(targetAssembly, 
									assem => assem.GetAttribute<NonLockingAutoCountersAttribute>());
		}
    }
}
