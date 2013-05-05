using System;
using System.Collections.Generic;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// This class mirrors the xml file that can be used to install 
    /// the counters.
    /// </summary>
    internal class AutoCounterCategoryRegistration : ICounterRegistration
    {
        /// <summary>
        /// Category name
        /// </summary>
        public string UniqueName { get; private set; }


        /// <summary>
        /// True if counter has multiple instances, usually best kept false
        /// </summary>
        public InstanceType InstanceType { get; private set; }


        /// <summary>
        /// List of AutoCounters to install
        /// </summary>
        public IDictionary<string, AutoCounterRegistration> AutoCounters { get; private set; }


        /// <summary>
        /// Category description
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="instanceType">The type of auto-counter instance</param>
        /// <param name="uniqueName">The unique name to store the auto counter under</param>
        public AutoCounterCategoryRegistration(InstanceType instanceType, string uniqueName)
        {
			if (uniqueName == null)
			{
				throw new ArgumentNullException("uniqueName");
			}

            UniqueName = uniqueName;
            InstanceType = instanceType;
            AutoCounters = new Dictionary<string, AutoCounterRegistration>();
        }


        /// <summary>
        /// Always returns null, categories have no underlying counter artifact.
        /// </summary>
        /// <returns>Always returns null.</returns>
        public ICounter GetCounter()
        {
            return null;
        }


        /// <summary>
        /// Always returns null, categories have no underlying counter artifacts.
        /// </summary>
        /// <param name="instanceName">The name of the instance of the counter to get.</param>
        /// <returns>Always returns null.</returns>
        public ICounter GetCounterInstance(string instanceName)
        {
            return null;
        }
    }
}