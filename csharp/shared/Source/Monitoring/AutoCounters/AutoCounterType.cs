namespace SharedAssemblies.Monitoring.AutoCounters
{
    /// <summary>
    /// Enum that specifies the type of auto counter being requested.
    /// </summary>
    public enum AutoCounterType
    {
        /// <summary>
        /// If the auto counter already exists on the system, it will assume the type specified 
        /// by the performance counter that underlies it.  If the counter does not exist, this
        /// will equal a counter of type Count.
        /// </summary>
        Unknown,


        /// <summary>
        /// A rolling average time counter is used to keep track of an average of the last 
        /// several samples of the time it took to execute a block.
        /// </summary>
        RollingAverageTime,


        /// <summary>
        /// The average time counter is the win32 specific average timer counter which resets
        /// quickly to zero if no calls are being made at the time, which can skew the numbers.
        /// </summary>
        AverageTime,


        /// <summary>
        /// This counter keeps track of the number of times a block of code was hit per second.
        /// </summary>
        CountsPerSecond,


        /// <summary>
        /// This counter keeps track of the total number of times a block of code was hit.
        /// </summary>
        TotalCount,


        /// <summary>
        /// This counter keeps track of the elapsed time since the counter was updated.
        /// </summary>
        ElapsedTime,
    }
}
