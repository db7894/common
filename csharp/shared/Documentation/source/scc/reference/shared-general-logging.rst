=========================================================================
Logging --- Log4net Helper Utilities and Extensions
=========================================================================
:Assembly: SharedAssemblies.General.Logging.dll
:Namespace: SharedAssemblies.General.Logging
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.General.Logging
   :platform: Windows, .Net
   :synopsis: Logging - Log4net Helper Utilities and Extensions

.. highlight:: csharp

.. index:: 
    pair: Log4net; Logging Utilities

Introduction
------------------------------------------------------------

The **SharedAssemblies.General.Logging** assembly are a set of *log4net* helper utilities and extensions
methods that attempt to make using *log4net* and performing common logging tasks a little easier.

Currently, there are two classes in this assembly:

    * **BlockLogger** - *Helper class that makes logging entry/exit of a block of code easy.*
    * **LogExtensions** - *Some extensions methods for the* ILog *interface that help usability.*
    * **RollingMaxDaysFileAppender** - *Custom log4net appender which will delete log files older than a configurable number of days*

As new custom appenders are created, this would be an ideal library to store them as well.

Usage
-------------------------------------------------------------

The following are brief examples of using the *Logging* assembly.  For more details, see the
API Reference documentation.

.. class:: BlockLogger

    The **BlockLogger** class is a simple class with a simple purpose: it logs entry into a block 
    on construction, and logs exit of a block on *Dispose()*.  This makes it idea for usage in
    *using* blocks to log entry/exit and duration of a block of code.

    To construct a *BlockLogger*, you call one of the static **Create(...)** factory methods.  
    
    .. method:: Create(log[, level][, description])
    
        :param log: The log4net ILog instance.
        :type log: ILog
        :param level: The logging level to log at.
        :type level: LoggingLevel
        :param description: A textural description of the block.
        :type description: string
        :returns: An instance of BlockLogger with the given log, level, and description.
        :rtype: BlockLogger
        
        The **Create(...)** method is a factory method to return an initialized *BlockLogger* with
        the appropriate settings.  If *level* is not specified, *LoggingLevel.Debug* is assumed.  If 
        *description* is not specified, the description is read from the current method in the stack frame.
    
    Any of the forms of this method create the *BlockLogger* instance with the given logging level and description.  
    From there, the **Dispose()** method invoked from the *using* does the rest:
    
    .. method:: Dispose()
        
        Automatically called at the end of a using block as per *IDisposable* semantics, or can be manually called.
        Either way, will stop the timer and record exit of the block and total duration.
    
    Example::

        /// <summary>
        /// Example class for block logger
        /// </summary>
        public class YourClass
        {
            /// <summary>Grab logger instance for this class</summary>
            private static readonly ILog _log = LogManager.GetLogger(typeof(YourClass));

            /// <summary>
            /// Some method with blocks to time
            /// </summary>
            public static void Main()
            {
                // logs entry and exit of this block using the current method name
                // at a default level of Debug to the _log log4net instance
                using(BlockLogger.Create(_log))
                {
                    // logs entry and exit of this block using the description "Database Call"
                    // at the logging level of informational
                    using(BlockLogger.Create(_log, LoggingLevel.Informational, "Database Call"))
                    {
                        // do some DB work you wish to time independently
                    }

                    // do other work
                }
            }
        }
        
    Yields the following output (assuming fictitious durations)::
    
        2010-03-15 10:35:49,127 [1] DEBUG DocHelper.YourClass [(null)] - Entering YourClass.Main
        2010-03-15 10:35:49,159 [1] INFO  DocHelper.YourClass [(null)] - Entering Database Call
        2010-03-15 10:35:49,471 [1] INFO  DocHelper.YourClass [(null)] - Leaving Database Call (306 ms)
        2010-03-15 10:35:49,502 [1] DEBUG DocHelper.YourClass [(null)] - Leaving YourClass.Main (370 ms)

.. class:: LogExtensions

    The **LogExtensions** class in the *Logging* assemblies fills a need left un-adressed by *log4net*: an easy
    way to log at a programmatic level.  That is, *log4net* gives you several levels at which to log at **compile time**::
    
        _log.Debug("This is a debug message.");
        _log.Info("This is an informational message.");
        _log.Warn("This is a warning message.");
        _log.Error("This is an error message.");
        _log.Fatal("This is a fatal message.");
        
    But, from it's core **ILog** interface, it does not allow you to log at a level determined pragmatically such as::
    
        logLevel = SomeThingThatDeterminesCorrectLevel();
        
        // log4net does not provide this in the ILog interface...
        _log.Log(logLevel, "This is a dynamically determined logging level.");

    Enter the **LogExtensions** class.  This is a collection of extensions methods that creates a set of **Log(...)**
    methods that mirror the explicit counterparts and an **IsLogEnabled(...)** method to query if a dynamic level is
    enabled or not.
    
    Really, these methods are simple at their core, but they make logging based on a dynamic logging level much easier as
    they just extend the current *ILog* interface.
    
    For example, let's say you're creating an interceptor that compares a method duration to a threshold, and if that
    threshold is exceeded, it will log a message.  Should this be a warning?  An error?  It really depends on the application
    and as the writer of that interceptor, you'd hate to dictate that level to a user as it limits flexibility.
    
    Now with the *LoggingExtensions*, you have a means to do that easily.
    
    .. note:: Because log4net does not publicly expose the concept of a *LoggingLevel* enumeration, the *Logging* assembly provides one called, fittingly enough, *LoggingLevel*.
    
    In fact, *BlockLogger* uses the *LoggingExtensions* to allow you to easily specify the level at which to log a block.  This is important because
    if that logging level is not active, you don't want to incur the cost of examining the stack and timers.  So if the logging level is inactive, it simply
    bypasses the logic::
    
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

            // determine if the log is enabled at runtime
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

             // log using the dynamically chosen level at runtime.
            _log.LogFormat(level, "Entering {0}", _description);
        }        
        
    .. note:: You should always check to make sure a log level is enabled before during a formatted log or any complex string building for the log message, this greatly reduces the overhead if the logging level is disabled.
    
    The following are the three methods exposed by *LogExtensions*.  They simply mimic their explicit *log4net* counterparts and their usage is the same, with the
    addition of the *LoggingLevel* parameter to specify the level at runtime.
    
    .. method:: IsLogEnabled(logger, level)
    
        :param logger: *implicit* - The *log4net* ILog reference.
        :type logger: **this** ILog
        :param level: The level to check.
        :type level: LoggingLevel
        :returns: True if the logging level specified is enabled.
        :rtype: bool
        
        This method behaves just like the *log4net* properties *IsDebugEnabled*, *IsInfoEnabled*, etc. except that it is a method that takes a logging level to check.
        
    .. method:: Log(logger, level, message[, exception])
    
        :param logger: *implicit* - The *log4net* ILog reference.
        :type logger: **this** ILog
        :param level: The level to check.
        :type level: LoggingLevel
        :param message: The object to stringify to form the message.
        :type message: object
        :param exception: Optional exception to detail in the log.
        :type exception: Exception
        :rtype: void
    
        This method behaves just like the *log4net* methods *Debug(...)*, *Info(...)*, *Warn(...)*, etc. except that it takes the logging level to use
        at run-time instead of compile time.
    
    .. method:: LogFormat(logger, level, format, args)
    
        :param logger: *implicit* - The *log4net* ILog reference.
        :type logger: **this** ILog
        :param level: The level to check.
        :type level: LoggingLevel
        :param format: The formatting string for the output message.
        :type message: string
        :param args: The arguments to pass to the formatting string.
        :type args: params object[]
        :rtype: void
    
        This method behaves just like the *log4net* methods *DebugFormat(...)*, *InfoFormat(...)*, *WarnFormat(...)*, etc. except that it takes the logging level to use
        at run-time instead of compile time.

.. class:: RollingMaxDaysFileAppender

    **RollingMaxDaysFileAppender** extends the log4net **RollingFileAppender** class adding the ability to automatically
	clean up old log files.  The parent **RollingFileAppender** class provides the ability to clean up excess log files that
	were rolled over based on size but not rolled over based on date.
	
	Using the **RollingMaxDaysFileAppender** class instead of another log4net appender only requires configuration changes
	(i.e. web.config / app.config).  For example, configuration of a **RollingFileAppender** might look like this::

        <appender name="MyServiceRollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
            <file name="File" value="C:\\Bashwork\\logs\\MyService" />
            <appendToFile value="true" />
            <rollingStyle value="Composite" />
            <datePattern value=".yyyy-MM-dd.\l\o\g" />
            <maxSizeRollBackups value="10" />
            <maximumFileSize value="100MB" />
            <staticLogFileName value="true" />
            <layout type="log4net.Layout.PatternLayout">
                <conversionPattern value="%date{HH:mm:ss.fff} [%thread] %-5level %logger{1} - %message%newline" />
            </layout>
        </appender>

    If you want your older applications logs to be cleaned up upon restart of your application (by using 
    **RollingMaxDaysFileAppender**), you can make 2 changes to that appender configuration:
	    * set the appender type to the namespace qualified **RollingMaxDaysFileAppender** class and specifying its assembly
        * add the *maxDaysRollBackups* property to specify the number of days worth of logs to keep.
    The appender configuration might then look something like this::
	
        <appender name="MyServiceRollingLogFileAppender" 
                  type="SharedAssemblies.General.Logging.RollingMaxDaysFileAppender, SharedAssemblies.General.Logging, Version=1.7.0.0, Culture=neutral, PublicKeyToken=ba0ce370e7f06a70">
            <file name="File" value="C:\\Bashwork\\logs\\MyService" />
            <appendToFile value="true" />
            <rollingStyle value="Composite" />
            <datePattern value=".yyyy-MM-dd.\l\o\g" />
            <maxSizeRollBackups value="10" />
            <maxDaysRollBackups value="14" />
            <maximumFileSize value="100MB" />
            <staticLogFileName value="true" />
            <layout type="log4net.Layout.PatternLayout">
                <conversionPattern value="%date{HH:mm:ss.fff} [%thread] %-5level %logger{1} - %message%newline" />
            </layout>
        </appender>

    Currently, this appender does not support purging for file configurations that have the *staticLogFileName* property set
    to *false* (it's default is *true*).  This property, when set to false, adds the *datePattern* suffix to the current 
    day's log file name.
 
    The following is the only method exposed by **RollingMaxDaysFileAppender**. 
    
    .. method:: ActivateOptions()
            
        This method is called by the *log4net* framework during appender initialization.

For more information, see the `API Reference <../../../../Api/index.html>`_.