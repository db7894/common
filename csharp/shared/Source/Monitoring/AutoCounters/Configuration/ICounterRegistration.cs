using System.Collections.Generic;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// A simple holding record to track the cached counters and their instances.  We are doing this
    /// so that we can load the cache once from the registry and then NOT need to lock on the map itself,
    /// but instead lock on the individual counters itself, this helps improve concurrency.
    /// </summary>
    internal interface ICounterRegistration 
    {
        /// <summary>
        /// Determines whether the counter is single or multiple instance
        /// </summary>
        InstanceType InstanceType { get; }


        /// <summary>
        /// The unique name this ICounter instance can be referenced by
        /// </summary>
        string UniqueName { get; }


        /// <summary>
        /// Returns a valid counter if this is a single-instance AutoCounter or
        /// AutoCounterCollection, otherwise returns null.
        /// </summary>
		/// <returns>Reference to the underlying counter</returns>
        ICounter GetCounter();


        /// <summary>
        /// Returns an instance of a counter if this is a multi-instance AutoCounter or
        /// AutoCounterCollection, otherwise returns null.
        /// </summary>
        /// <param name="instanceName">The name of the instance of the counter to get.</param>
        /// <returns>Returns the underlying instance counter</returns>
        ICounter GetCounterInstance(string instanceName);
    }
}
