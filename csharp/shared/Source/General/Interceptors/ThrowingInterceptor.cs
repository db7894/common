using System;
using System.Reflection;
using System.Collections.Generic;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using log4net;

namespace SharedAssemblies.General.Interceptors
{
	/// <summary>
	/// Interceptor that actually throws when a method of a specified interface
	/// is invoked (if no list is specified we throw on all methods) The return
	/// value will always be the default value of the return type
	/// (0 for value, null for ref)
	/// </summary>
	/// <remarks>
	/// Use case could be to block depricated, broken, etc methods from being
	/// called.
	/// </remarks>
	public static class ThrowingInterceptor 
	{
        /// <summary>Out logging handle</summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(ThrowingInterceptor));

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
			var type = typeof(TInterface);
			var methods = new List<MethodInfo>(type.GetMethods());

		    return Create(type, new TImplement(), methods) as TInterface;
		}


        /// <summary>
        /// A static factory to wrap an instance with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <param name="instance">The instance to wrap</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface>(TInterface instance) where TInterface : class
        {
            var type = typeof(TInterface);
            var methods = new List<MethodInfo>(type.GetMethods());
            return Create(type, instance, methods) as TInterface;
        }


        /// <summary>
        /// A static factory to create an instance wrapped with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <typeparam name="TImplement">The implementation to wrap</typeparam>
        /// <param name="methods">A list of methods to throw on</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface, TImplement>(List<MethodInfo> methods)
            where TInterface : class
            where TImplement : class, TInterface, new()
        {
            return Create(typeof(TInterface), new TImplement(), methods) as TInterface;
        }


        /// <summary>
        /// A static factory to wrap an instance with this interceptor
        /// </summary>
        /// <typeparam name="TInterface">The interface to proxy</typeparam> 
        /// <param name="instance">The instance to wrap</param>
        /// <param name="methods">A list of methods to throw on</param>
        /// <returns>A wrapped instance</returns>
        public static TInterface Create<TInterface>(TInterface instance, List<MethodInfo> methods)
            where TInterface : class
        {
            return Create(typeof(TInterface), instance, methods) as TInterface;
        }

        
        /// <summary>
        /// This is the internal helper method that actually does the call into Castle.
        /// We are chosing to use the type where you pass in the type instead of the 
        /// generic to avoid calling assemblies having to directly reference Castle libraries
        /// themselves.
        /// </summary>
        /// <param name="interfaceType">The type of the interface</param>
        /// <param name="target">The target to wrap</param>
        /// <param name="methods">The methods to check for</param>
        /// <returns>The interceptor object created</returns>
        private static object Create(Type interfaceType, object target, List<MethodInfo> methods)
        {
            return _generator.CreateInterfaceProxyWithTarget(interfaceType, target,
                                                             new Thrower(methods));
        }


		/// <summary>
		/// Inner helper class to implement the Castle interceptor.
		/// Hidden as a private inner class so not exposing Castle libraries.
		/// </summary>
		private class Thrower : IInterceptor
		{
			/// <summary>A list of methods to throw when called</summary>
			private readonly List<MethodInfo> _thrown;


			/// <summary>
			/// Constructor to add the methods to throw on
			/// </summary>
			/// <param name="methods">The interface method to be invoked</param>
			public Thrower(List<MethodInfo> methods)
			{
				_thrown = methods;
			}


			/// <summary>
			/// Intercept functor for each method invokation
			/// </summary>
			/// <param name="invocation">The interface method to be invoked</param>
			public void Intercept(IInvocation invocation)
			{
				if (_thrown.Contains(invocation.Method))
				{
					string message = string.Format("Throwing On Invocation Of[{0}]\n",
						invocation.Method.Name);

					if (_log.IsDebugEnabled)
					{
						_log.DebugFormat(message);
					}
					throw new SystemException(message);
				}
				invocation.Proceed();
			}
		}
	}
}
