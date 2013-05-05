using System;
using System.Diagnostics;
using log4net;


namespace SharedAssemblies.General.Logging
{
    /// <summary>
    /// <para>
    /// Automatically logs entry and exit from a block of code and the elapsed duration
    /// </para>
    /// <para>
    /// The goal of this log is to automatically log entry/exit of a block and
    /// the duration it took, this will later tie into performance counter classes to
    /// automatically report this duration, count, or averages.
    /// </para>
    /// <para>
    /// The Create() call will return null if logging is not enabled at the level chosen (Debug
    /// if not specified).  This is to avoid object creation and garbage collection overhead if
    /// the logging level is disabled.  As long as you are using in a "using" block this will work
    /// fine, but if you use manually, you should check for null after Create().
    /// </para>
    /// <para>
    /// Usage:
    /// </para>
    /// <para>
    /// public void SomeFunction()
    /// {
    ///     using(BlockLogger.Create(_myLog))
    ///     {
    ///         // your code here
    ///     }
    /// }
    /// </para>
    /// <para>
    /// This will cause the user to see the following in the log:
    /// </para>
    /// <para>
    ///     DEBUG: Entering SomeClass::SomeFunction at 2008-02-07 11:30:00.001
    ///     ...
    ///     DEBUG: Leaving SomeClass::SomeFunction at 2008-02-07 11:30:05.005 (5004 ms)
    /// </para>
    /// </summary>
    public class BlockLogger : IDisposable
    {
        /// <summary>
        /// The default number of stack frames to skip when logging a method
        /// </summary>
        public const int DefaultStackFramesToSkip = 2;

        /// <summary>The instance of the logger to log to.</summary>
        private ILog _log;

        /// <summary>The level to log at.</summary>
        private readonly LoggingLevel _level;

        /// <summary>The description of the block to log.</summary>
        private readonly string _description;

        /// <summary>The timer for timing the method.</summary>
        private readonly Stopwatch _timer;      


        /// <summary>
        /// Creates a block log that creates a begin message on construction and an end message
        /// on disposal
        /// </summary>
        /// <param name="log">The log to write the entry/exit</param>
        /// <param name="level">The level to log the entry/exit at</param>
        /// <param name="description">the description of the entry/exit event</param>
        private BlockLogger(ILog log, LoggingLevel level, string description)
        {
            _log = log;
            _level = level;
            _description = description;
            _timer = new Stopwatch();
            _timer.Start();

            _log.LogFormat(level, "Entering {0}", _description);
        }


        /// <summary>
        /// Static method to return a block log given a generic log, this will log at the
        /// debug level in the specific log instance.
        /// </summary>
        /// <param name="log">Generic log wrapper.</param>
        /// <param name="level">Level to log block at</param>
        /// <returns>Block log block</returns>
        public static BlockLogger Create(ILog log, LoggingLevel level)
        {
            return Create(log, level, DefaultStackFramesToSkip);
        }


        /// <summary>
        /// Static method to return a block log given a generic log, this will log at the
        /// debug level in the specific log instance.
        /// </summary>
        /// <param name="log">Generic log wrapper.</param>
        /// <param name="level">Level to log block at</param>
        /// <param name="description">Description of block to log</param>
        /// <returns>Block log block</returns>
        public static BlockLogger Create(ILog log, LoggingLevel level, string description)
        {
            BlockLogger logger = null;

            // only create if log is enabled, otherwise returns null
            if(log.IsLogEnabled(level))
            {
                logger = new BlockLogger(log, level, description);
            }

            return logger;
        }


        /// <summary>
        /// Static method to return a block log block from the stack, will build description from
        /// the call stack at the default frame depth of 2.
        /// </summary>
        /// <param name="log">A delegate that will log the string source</param>
        /// <returns>Block log block</returns>
        public static BlockLogger Create(ILog log)
        {
            // it is imperitive these only go down 2 levels so that we don't pick wrong stack
            // frame, so if you nest the calls deeper, make sure you adjust frame
            return Create(log, LoggingLevel.Debug, DefaultStackFramesToSkip);
        }


        /// <summary>
        /// Static method to return a block log block from the stack
        /// </summary>
        /// <param name="log">A delegate that will log the string source</param>
        /// <param name="description">The specified description of this block</param>
        /// <returns>Block log block</returns>
        public static BlockLogger Create(ILog log, string description)
        {
            return Create(log, LoggingLevel.Debug, description);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting 
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // log exit if log still exists and debug is enabled
            if (_log != null)
            {
                _timer.Stop();

                _log.LogFormat(_level, "Leaving {0} ({1} ms)", _description,
                           _timer.ElapsedMilliseconds);

                // dissociate with the delegate
                _log = null;
            }
        }

        
        /// <summary>
        /// Static method to return a block log block from the stack, will build description
        /// from the call stack at the specified frame depth.
        /// </summary>
        /// <param name="log">A delegate that will log the string source</param>
        /// <param name="level">The level to log the block as</param>
        /// <param name="skipFrames">Number of frames to skip in stack trace</param>
        /// <returns>Block log block</returns>
        private static BlockLogger Create(ILog log, LoggingLevel level, int skipFrames)
        {
            BlockLogger logger = null;

            if (log.IsLogEnabled(level))
            {
                // if description is null, build from class/method name
                var stackFrame = new StackFrame(skipFrames, false);

                logger = new BlockLogger(log, level,
                    string.Format("{0}.{1}", stackFrame.GetMethod().DeclaringType.Name,
                        stackFrame.GetMethod().Name));
            }

            return logger;
        }
    }
}