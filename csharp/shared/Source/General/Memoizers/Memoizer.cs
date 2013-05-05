using System;
using System.Collections.Generic;
using Castle.DynamicProxy;


namespace SharedAssemblies.General.Memoizers
{
	/// <summary>
	/// The MethodCallMemorizer is used to cache calls to methods in order to remember their
	/// results if they are called with the same set of inputs multiple times.
	/// </summary>
	public static class Memoizer
	{
		// Handle to our proxy generator
		private static readonly ProxyGenerator _generator = new ProxyGenerator();

		/// <summary>
		/// Create a memorizer that will examine method calls and use a cache to store
		/// the request if subsequent requests with the same arguments are passed multiple
		/// times within the given time span.
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <typeparam name="TImplement">The implementation to wrap</typeparam>
		/// <param name="cacheInterval">The interval to cache calls.</param>
		/// <param name="cleanInterval">The interval to clean up cache.</param>
		/// <returns>A wrapped instance</returns>
		public static TInterface Create<TInterface, TImplement>(TimeSpan cacheInterval, TimeSpan cleanInterval)
			where TInterface : class
			where TImplement : class, TInterface, new()
		{
			return Create(typeof(TInterface), new TImplement(), cacheInterval, cleanInterval, null) as TInterface;
		}

		/// <summary>
		/// Create a memorizer that will examine method calls and use a cache to store
		/// the request if subsequent requests with the same arguments are passed multiple
		/// times within the given time span.
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <param name="instance">The instance to wrap</param>
		/// <param name="cacheInterval">The interval to cache calls.</param>
		/// <param name="cleanInterval">The interval to clean up cache.</param>
		/// <returns>A wrapped instance</returns>
		public static TInterface Create<TInterface>(TInterface instance, TimeSpan cacheInterval, TimeSpan cleanInterval) 
			where TInterface : class
		{
			return Create(typeof(TInterface), instance, cacheInterval, cleanInterval, null) as TInterface;
		}

		/// <summary>
		/// Create a memorizer that will examine method calls and use a cache to store
		/// the request if subsequent requests with the same arguments are passed multiple
		/// times within the given time span.
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <typeparam name="TImplement">The implementation to wrap</typeparam>
		/// <param name="cacheInterval">The interval to cache calls.</param>
		/// <param name="cleanInterval">The interval to clean up cache.</param>
		/// <param name="methods">A list of methods to throw on</param>
		/// <returns>A wrapped instance</returns>
		public static TInterface Create<TInterface, TImplement>(TimeSpan cacheInterval, TimeSpan cleanInterval, IEnumerable<string> methods)
				where TInterface : class
				where TImplement : class, TInterface, new()
		{
			return Create(typeof(TInterface), new TImplement(), cacheInterval, cleanInterval, methods) as TInterface;
		}

		/// <summary>
		/// Create a memorizer that will examine method calls and use a cache to store
		/// the request if subsequent requests with the same arguments are passed multiple
		/// times within the given time span.
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <param name="instance">The target to be intercepted.</param>
		/// <param name="cacheInterval">The interval to cache calls.</param>
		/// <param name="cleanInterval">The interval to clean up cache.</param>
		/// <param name="methods">The methods to intercept.</param>
		/// <returns>An interceptor that intercepts and caches method call responses.</returns>
		public static TInterface Create<TInterface>(TInterface instance, TimeSpan cacheInterval,
			TimeSpan cleanInterval, IEnumerable<string> methods)
				where TInterface : class
		{
			return Create(typeof(TInterface), instance, cacheInterval, cleanInterval, methods) as TInterface;
		}

		/// <summary>
		/// Create a memorizer that will examine method calls and use a cache to store
		/// the request if subsequent requests with the same arguments are passed multiple
		/// times within the given time span.
		/// </summary>
		/// <param name="interfaceType">The interface that the target satisfies.</param>
		/// <param name="target">The target to be intercepted.</param>
		/// <param name="cacheInterval">The interval to cache calls.</param>
		/// <param name="cleanInterval">The interval to clean up cache.</param>
		/// <param name="methods">The methods to intercept.</param>
		/// <returns>An interceptor that intercepts and caches method call responses.</returns>
		private static object Create(Type interfaceType, object target, TimeSpan cacheInterval, TimeSpan cleanInterval, IEnumerable<string> methods)
		{
			// if the cache interval is positive (> 0), then we wrap with our interceptor
			if (cacheInterval.Ticks > 0)
			{
				var interceptor = new MethodCallCachingInterceptor(cacheInterval, cleanInterval, methods);

				// create a mix-in so that we can query the interceptor for its Memoizer statistics
				var options = new ProxyGenerationOptions();
				options.AddMixinInstance(new InterceptorController(interceptor));

				return _generator.CreateInterfaceProxyWithTarget(interfaceType, target, options, interceptor);
			}

			// otherwise, if cache interval is zero (or less), then we are straight pass-through to implementation.
			return target;
		}
	}
}
