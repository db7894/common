namespace SharedAssemblies.Monitoring.AutoCounters
{
    /// <summary>
    /// Enumeration that lists the types of auto counter categories that can be created.
    /// </summary>
    public enum InstanceType
    {
        /// <summary>
        /// The category supports only one instance within it, to try to use an instance name in this 
        /// category will result in an error or null counter depending on the value of the
        /// CreateFailedAction specified.
        /// </summary>
        SingleInstance,


        /// <summary>
        /// The category can support multiple instances of all counters within it.  Ommitting an instance
        /// name in this category will result in an error or null counter depending on the value of the
        /// CreateFailedAction specified.
        /// </summary>
        MultiInstance,
    }
}
