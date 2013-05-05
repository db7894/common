namespace SharedAssemblies.General.Logging
{
    /// <summary>
    /// The level to use for logging a message or determining if a logging level is enabled
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// Debug messages are intended for information that should not appear in production.
        /// </summary>
        Debug,

        /// <summary>
        /// Informational messages are simply informative about events and not intended to raise alarm.
        /// </summary>
        Informational,

        /// <summary>
        /// Warning messages are not critical errors, but warrant attention.
        /// </summary>
        Warning,

        /// <summary>
        /// Error messages are used to alert of critical issues which may cause bad behaviors.
        /// </summary>
        Error,

        /// <summary>
        /// Fatal message are so bad that the application cannot continue.
        /// </summary>
        Fatal
    }
}
