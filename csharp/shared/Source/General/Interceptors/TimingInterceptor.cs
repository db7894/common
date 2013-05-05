using System;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using log4net;
using SharedAssemblies.General.Logging;


namespace SharedAssemblies.General.Interceptors
{
    /// <summary>
    /// Interceptor that performs a block timing of an interfaces
    /// methods invocations
    /// </summary>
    /// <remarks>
    /// Use cases include performance testing a service or providing
    /// system monitoring.
    /// </remarks>
    public static class TimingInterceptor 
    {
        /// <summary>Out logging handle</summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof(TimingInterceptor));

        /// <summary>Handle to our proxy generator</summary>
        private static readonly ProxyGenerator _generator = new ProxyGenerator();


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
            return Create(typeof(TInterface), new TImplement()) as TInterface;
        }


        /// <summary>
        /// A static factory to wrap an instance with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <param name="instance">The instance to wrap</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface>(TInterface instance) where TInterface : class
        {
            return Create(typeof(TInterface), instance) as TInterface;
        }


        /// <summary>
        /// This is the internal helper method that actually does the call into Castle.
        /// We are chosing to use the type where you pass in the type instead of the 
        /// generic to avoid calling assemblies having to directly reference Castle libraries
        /// themselves.
        /// </summary>
        /// <param name="interfaceType">The type of the interface</param>
        /// <param name="target">The target to wrap</param>
        /// <returns>The interceptor object created</returns>
        private static object Create(Type interfaceType, object target)
        {
            return _generator.CreateInterfaceProxyWithTarget(interfaceType, target,
                                                             new BlockTimer());
        }


		/// <summary>
		/// Implementation of the Castle interceptor.  Hidden as a private inner class
		/// to avoid exposing the castle library directly.
		/// </summary>
		private class BlockTimer : IInterceptor
		{
			/// <summary>
			/// Intercept functor for each method invokation
			/// </summary>
			/// <param name="invocation">The interface method to be invoked</param>
			public void Intercept(IInvocation invocation)
			{
				using (BlockLogger.Create(_log, invocation.Method.Name))
				{
					invocation.Proceed();
				}
			}
		}
	}
}
