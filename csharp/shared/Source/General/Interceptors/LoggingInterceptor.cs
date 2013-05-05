using System;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using log4net;
using SharedAssemblies.General.Logging;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.General.Interceptors
{
	/// <summary>
	/// Interceptor that serializes and logs the input and output
	/// as well as performs a block time logging of every method
	/// invokation of an interface
	/// </summary>
	/// <remarks>Use cases included service tracing/debugging and servicelogging</remarks>
	public static class LoggingInterceptor
	{
	    /// <summary>Our logging handle</summary>
	    private static readonly ILog _log = LogManager.GetLogger(typeof(LoggingInterceptor));

		/// <summary>Handle to our proxy generator</summary>
		private static readonly ProxyGenerator _generator = new ProxyGenerator();

        /// <summary>The default level to log parameters and returns</summary>
        private static readonly LoggingLevel _defaultLoggingLevel = LoggingLevel.Debug;

		/// <summary>
		/// A static factory to create an instance wrapped with this interceptor
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <typeparam name="TImplement">The implementation to wrap</typeparam>
		/// <returns>A wrapped instance</returns>
		public static TInterface Create<TInterface, TImplement>()
			where TInterface : class
			where TImplement : class, TInterface, new()
		{
			return Create(typeof(TInterface), new TImplement(), _defaultLoggingLevel) as TInterface;
		}


		/// <summary>
		/// A static factory to wrap an instance with this interceptor
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <param name="instance">The instance to wrap</param>
		/// <returns>A wrapped instance</returns>
		public static TInterface Create<TInterface>(TInterface instance) where TInterface : class
		{
		    return Create(typeof(TInterface), instance, _defaultLoggingLevel) as TInterface;
		}

        
        /// <summary>
        /// A static factory to create an instance wrapped with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <typeparam name="TImplement">The implementation to wrap</typeparam>
        /// <param name="level">The level to log the function calls at</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface, TImplement>(LoggingLevel level)
            where TInterface : class
            where TImplement : class, TInterface, new()
        {
            return Create(typeof(TInterface), new TImplement(), level) as TInterface;
        }


        /// <summary>
        /// A static factory to wrap an instance with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <param name="instance">The instance to wrap</param>
        /// <param name="level">The level to log the function calls at</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface>(TInterface instance, LoggingLevel level) 
            where TInterface : class
        {
            return Create(typeof(TInterface), instance, level) as TInterface;
        }




        /// <summary>
        /// This is the internal helper method that actually does the call into Castle.
        /// We are chosing to use the type where you pass in the type instead of the 
        /// generic to avoid calling assemblies having to directly reference Castle libraries
        /// themselves.
        /// </summary>
        /// <param name="interfaceType">The type of the interface</param>
        /// <param name="target">The target to wrap</param>
        /// <param name="levelToLog">The level to log the parameters</param>
        /// <returns>The interceptor object created</returns>
        private static object Create(Type interfaceType, object target, LoggingLevel levelToLog)
        {
            return _generator.CreateInterfaceProxyWithTarget(interfaceType, target,
                                                             new Logger(levelToLog));
        }


		/// <summary>
		/// Internal class which does the actual IInterceptor intercepting,
		/// this is to keep castle artifacts from bleeding up a level.
		/// </summary>
		private class Logger : IInterceptor
		{
			/// <summary>Level to log at</summary>
			private readonly LoggingLevel _levelToLog;

			/// <summary>
			/// Constructs a logging interceptor at the given logging level
			/// </summary>
			/// <param name="levelToLog">The level to lot at</param>
			public Logger(LoggingLevel levelToLog)
			{
				_levelToLog = levelToLog;
			}


			/// <summary>
			/// Intercept functor for each method invokation
			/// </summary>
			/// <param name="invocation">The interface method to be invoked</param>
			public void Intercept(IInvocation invocation)
			{
				using (BlockLogger.Create(_log, invocation.Method.Name))
				{
					if (_log.IsLogEnabled(_levelToLog))
					{
						invocation.Arguments.ForEach(
							param => _log.LogFormat(_levelToLog, "IN: request = {0}\n{1}", param,
													param.ToXml(true)));
					}

					invocation.Proceed();

					if (_log.IsLogEnabled(_levelToLog))
					{
						if (invocation.ReturnValue != null)
						{
							_log.LogFormat(_levelToLog, "OUT: return = {0}\n{1}", invocation.ReturnValue,
								invocation.ReturnValue.ToXml(true));
						}
						else
						{
							_log.Log(_levelToLog, "OUT: void");
						}
					}
				}
			}
		}
	}
}
