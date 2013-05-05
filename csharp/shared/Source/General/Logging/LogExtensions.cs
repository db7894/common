using System;
using log4net;


namespace SharedAssemblies.General.Logging
{
    /// <summary>
    /// Extension methods for ILog to enable generic logging at an specified level at
    /// runtime instead of at compile-time.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Determines if logging is enabled at a given level.
        /// </summary>
        /// <param name="logger">The log4net ILog instance to log to.</param>
        /// <param name="level">Level to check.</param>
        /// <returns>True if that level is enabled and false if not.</returns>
        public static bool IsLogEnabled(this ILog logger, LoggingLevel level)
        {
            // if it's an unknown level or improper enum value, we're not enabled
            bool isEnabled = false;

            switch (level)
            {
                case LoggingLevel.Debug:
                    isEnabled = logger.IsDebugEnabled;
                    break;
                case LoggingLevel.Informational:
                    isEnabled = logger.IsInfoEnabled;
                    break;
                case LoggingLevel.Warning:
                    isEnabled = logger.IsWarnEnabled;
                    break;
                case LoggingLevel.Error:
                    isEnabled = logger.IsErrorEnabled;
                    break;
                case LoggingLevel.Fatal:
                    isEnabled = logger.IsFatalEnabled;
                    break;
            }

            return isEnabled;
        }


        /// <summary>
        /// Logs a simple message to the log at debug level.
        /// </summary>
        /// <param name="logger">The log4net ILog instance to log to.</param>
        /// <param name="level">The level to log at, allows you to specify dynamically.</param>
        /// <param name="message">The message to write to the log.</param>
        public static void Log(this ILog logger, LoggingLevel level, object message)
        {
            switch (level)
            {
                case LoggingLevel.Debug:
                    logger.Debug(message);
                    break;
                case LoggingLevel.Informational:
                    logger.Info(message);
                    break;
                case LoggingLevel.Warning:
                    logger.Warn(message);
                    break;
                case LoggingLevel.Error:
                    logger.Error(message);
                    break;
                case LoggingLevel.Fatal:
                    logger.Fatal(message);
                    break;
            }
        }


        /// <summary>
        /// Logs a message and exception to the log at specified level.
        /// </summary>
        /// <param name="logger">The log4net ILog instance to log to.</param>
        /// <param name="level">The level to log at, allows you to specify dynamically.</param>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="exception">The exception to write to the log.</param>
        public static void Log(this ILog logger, LoggingLevel level, object message, Exception exception)
        {
            switch (level)
            {
                case LoggingLevel.Debug:
                    logger.Debug(message, exception);
                    break;
                case LoggingLevel.Informational:
                    logger.Info(message, exception);
                    break;
                case LoggingLevel.Warning:
                    logger.Warn(message, exception);
                    break;
                case LoggingLevel.Error:
                    logger.Error(message, exception);
                    break;
                case LoggingLevel.Fatal:
                    logger.Fatal(message, exception);
                    break;
            }
        }


        /// <summary>
        /// Logs a formatted message to the log at the specified level.  
        /// </summary>
        /// <param name="logger">The log4net ILog instance to log to.</param>
        /// <param name="level">The level to log at, allows you to specify dynamically.</param>
        /// <param name="format">Format string.</param>
        /// <param name="args">The array of formatting arguments.</param>
        public static void LogFormat(this ILog logger, LoggingLevel level, string format, 
			params object[] args)
        {
            switch (level)
            {
                case LoggingLevel.Debug:
                    logger.DebugFormat(format, args);
                    break;
                case LoggingLevel.Informational:
                    logger.InfoFormat(format, args);
                    break;
                case LoggingLevel.Warning:
                    logger.WarnFormat(format, args);
                    break;
                case LoggingLevel.Error:
                    logger.ErrorFormat(format, args);
                    break;
                case LoggingLevel.Fatal:
                    logger.FatalFormat(format, args);
                    break;
            }
        }
    }
}
