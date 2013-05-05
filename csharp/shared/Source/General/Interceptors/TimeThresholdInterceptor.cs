using System;
using System.Diagnostics;
using System.Text;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using log4net;
using SharedAssemblies.General.Logging;


namespace SharedAssemblies.General.Interceptors
{
    /// <summary>
    /// Interceptor that performs a method and checks time against a threshold, logging
    /// a warning if the time goes above that threshold.
    /// </summary>
    /// <remarks>
    /// Use cases include checking database or web service calls for long queries.
    /// </remarks>
    public static class TimeThresholdInterceptor 
    {
        /// <summary>Out logging handle</summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof(TimeThresholdInterceptor));

        /// <summary>Handle to our proxy generator</summary>
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        /// <summary>the default level to log threshold exceeded messages</summary>
        private const LoggingLevel _defaultLoggingLevel = LoggingLevel.Warning;


        /// <summary>
        /// A static factory to create an instance wrapped with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <typeparam name="TImplement">The implementation to wrap</typeparam>
        /// <param name="threshold">The threshold that will cause a warning if exceeded</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface, TImplement>(TimeSpan threshold)
            where TInterface : class
            where TImplement : class, TInterface, new()
        {
            return Create(typeof(TInterface), new TImplement(), threshold, _defaultLoggingLevel) 
				as TInterface;
        }


        /// <summary>
        /// A static factory to create an instance wrapped with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <typeparam name="TImplement">The implementation to wrap</typeparam>
        /// <param name="threshold">The threshold that will cause a warning if exceeded</param>
        /// <param name="level">The level to log threshold exceeded messages</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface, TImplement>(TimeSpan threshold, LoggingLevel level)
            where TInterface : class
            where TImplement : class, TInterface, new()
        {
            return Create(typeof(TInterface), new TImplement(), threshold, level) as TInterface;
        }


        /// <summary>
        /// A static factory to wrap an instance with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <param name="instance">The instance to wrap</param>
        /// <param name="threshold">The threshold that will cause a warning if exceeded</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface>(TInterface instance, TimeSpan threshold) 
            where TInterface : class
        {
            return Create(typeof(TInterface), instance, threshold, _defaultLoggingLevel) as TInterface;
        }

        
        /// <summary>
        /// A static factory to wrap an instance with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <param name="instance">The instance to wrap</param>
        /// <param name="threshold">The threshold that will cause a warning if exceeded</param>
        /// <param name="level">The level at which to log the threshold exceeded message</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface>(TInterface instance, TimeSpan threshold, 
            LoggingLevel level) where TInterface : class
        {
            return Create(typeof(TInterface), instance, threshold, level) as TInterface;
        }


        /// <summary>
        /// This is the internal helper method that actually does the call into Castle.
        /// We are chosing to use the type where you pass in the type instead of the 
        /// generic to avoid calling assemblies having to directly reference Castle libraries
        /// themselves.
        /// </summary>
        /// <param name="interfaceType">The type of the interface</param>
        /// <param name="target">The target to wrap</param>
        /// <param name="threshold">The threshold that will cause a warning if exceeded</param>
        /// <param name="level">The level at which to log the threshold exceeded message</param>
        /// <returns>The interceptor object created</returns>
        private static object Create(Type interfaceType, object target,
			TimeSpan threshold, LoggingLevel level)
        {
            if (threshold.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException("threshold",
                    "The threshold TimeSpan must be positive.");
            }

            return _generator.CreateInterfaceProxyWithTarget(interfaceType, target,
                                                             new TimedThreshold(threshold, level));
        }


		/// <summary>
		/// The interceptor that is created to intercept the interface calls.
		/// Hidden as a private inner class so not exposing Castle libraries.
		/// </summary>
		private class TimedThreshold : IInterceptor
		{
			/// <summary>
			/// The level at which to log when threshold is exceeded.
			/// </summary>
			private readonly LoggingLevel _loggingLevel;

			/// <summary>
			/// The threshold as a positive timespan that triggers a log message.
			/// </summary>
			private readonly TimeSpan _threshold;

			/// <summary>
			/// Marking constructor private so must use Create methods
			/// </summary>
			/// <param name="threshold">threshold over which to log a message.</param>
			/// <param name="level">logging level to log warning at.</param>
			public TimedThreshold(TimeSpan threshold, LoggingLevel level)
			{
				_loggingLevel = level;
				_threshold = threshold;
			}


			/// <summary>
			/// Intercept functor for each method invokation
			/// </summary>
			/// <param name="invocation">The interface method to be invoked</param>
			public void Intercept(IInvocation invocation)
			{
				// time the method invocation
				var timer = Stopwatch.StartNew();

				invocation.Proceed();

				timer.Stop();

				// check if threshold is exceeded
				if (timer.Elapsed > _threshold)
				{
					if (_log.IsLogEnabled(_loggingLevel))
					{
						var builder = new StringBuilder("Time to execute ");
						builder.Append(invocation.Method.Name);
						builder.Append(" took ");
						builder.Append(timer.ElapsedMilliseconds);
						builder.Append(" ms.  Arguments = [");

						foreach (var argument in invocation.Arguments)
						{
							builder.Append(argument);
							builder.Append(",");
						}

						builder.Append("].  Instance = [");
						builder.Append(invocation.InvocationTarget.ToString());
						builder.Append("].");

						_log.Log(_loggingLevel, builder.ToString());
					}
				}
			}
		}
	}
}
