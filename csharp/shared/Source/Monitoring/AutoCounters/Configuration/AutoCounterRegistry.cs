using System.Collections.Generic;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// A class that maintains a registry of all auto counters and categories
    /// </summary>
    internal class AutoCounterRegistry 
    {
        /// <summary>
        /// Gets the set of categories defined in this registry
        /// </summary>
        internal Dictionary<string, AutoCounterCategoryRegistration> Categories { get; private set; }


        /// <summary>
        /// Gets the set of collections defined in the registry
        /// </summary>
        internal Dictionary<string, AutoCounterCollectionRegistration> Collections { get; private set; }


        /// <summary>
        /// Gets the set of counters defined in the registry
        /// </summary>
        internal Dictionary<string, AutoCounterRegistration> Counters { get; private set; }


        /// <summary>
        /// Gets the set of unique names in the registry
        /// </summary>
        internal Dictionary<string, ICounterRegistration> UniqueNames { get; private set; }


        /// <summary>
        /// Gets the set of heartbeats in the registry
        /// </summary>
        internal List<AutoHeartbeatRegistration> Heartbeats { get; private set; }


        /// <summary>
        /// Construct the registry with an empty category registration list
        /// </summary>
        internal AutoCounterRegistry()
        {
            Categories = new Dictionary<string, AutoCounterCategoryRegistration>();
            Collections = new Dictionary<string, AutoCounterCollectionRegistration>();
            Counters = new Dictionary<string, AutoCounterRegistration>();
            UniqueNames = new Dictionary<string, ICounterRegistration>();
            Heartbeats = new List<AutoHeartbeatRegistration>();
        }

        
        /// <summary>
        /// Attempt to get the registration, if the counter is not found, will return null./
        /// </summary>
        /// <param name="uniqueName">Unique name of the counter, collection, or category.</param>
        /// <returns>The registration associated with a unique item name.</returns>
        internal ICounterRegistration Get(string uniqueName)
        {
            // attempt to get the counter, returning null if not found
            ICounterRegistration counterToGet;
            if (!UniqueNames.TryGetValue(uniqueName, out counterToGet))
            {
                counterToGet = null;
            }

            return counterToGet;
        }


        /// <summary>
        /// Attempt to get a strongly typed registration, if the counter is not found or is not of the
        /// requested type, this method will return null./
        /// </summary>
        /// <param name="uniqueName">Unique name of the counter, collection, or category.</param>
        /// <typeparam name="TRegistration">The registration types</typeparam>
        /// <returns>The registration associated with a unique item name.</returns>
        internal TRegistration Get<TRegistration>(string uniqueName) 
            where TRegistration : class, ICounterRegistration
        {
            return Get(uniqueName) as TRegistration;
        }


        /// <summary>
        /// start all heartbeats registered
        /// </summary>
        internal void StartHeartbeats()
        {
            Heartbeats.ForEach(hb => hb.Start());
        }


        /// <summary>
        /// Stop all heartbeats registered
        /// </summary>
        internal void StopHeartbeats()
        {
            Heartbeats.ForEach(hb => hb.Stop());            
        }
    }
}
