namespace SharedAssemblies.Monitoring.AutoCounters
{
    /// <summary>
    /// Enumerates the list of actions that an AutoCounter will perform
    /// if the underlying performance counter was not found.
    /// </summary>
    public enum CreateFailedAction
    {
        /// <summary>
        /// If the counter being created does not exist, the calling assembly will be
        /// examined to see if a default action was declared in the assembly
        /// attributes, if not, it will take the CreateStub action.
        /// </summary>
        Default,

        /// <summary>
        /// <para>
        /// If the counter being created does not exist, the AutoCounter will wrap a
        /// stub strategy that does nothing and always returns -1 as the value.  In this
        /// way, it doesn't throw (blowing up unit tests), but still shows a suspect value
        /// so it doesn't look like there is simply nothing to report.
        /// </para>
        /// <para>
        /// That is, if you were looking at system monitor and saw Number Errors = 0, you would 
        /// know the counter exists and had a value of zero.  Whereas if you saw Number Errors = -1,
        /// you would know that the counter did not exist.
        /// </para>
        /// </summary>
        CreateStub,

        /// <summary>
        /// If the counter being created does not exist on the current machine, the 
        /// AutoCounter will throw an AutoCounterException.
        /// </summary>
        ThrowException
    }
}
