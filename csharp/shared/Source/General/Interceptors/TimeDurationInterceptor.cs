using System;
using System.Diagnostics;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;


namespace SharedAssemblies.General.Interceptors
{
	/// <summary>
	/// Interceptor that checks the duration of a method call and calls an action when done, passing
	/// the duration of the method in milliseconds.
	/// </summary>
	/// <remarks>
	/// Use cases include performance testing a service or providing
	/// system monitoring.
	/// </remarks>
	public static class TimeDurationInterceptor
	{
		/// <summary>Handle to our proxy generator</summary>
		private static readonly ProxyGenerator _generator = new ProxyGenerator();


		/// <summary>
		/// A static factory to create an instance wrapped with this interceptor
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <typeparam name="TImplement">The implementation to wrap</typeparam>
		/// <param name="durationHandler">An action to handle the duration of the method call</param>
		/// <returns>A wrapped instance</returns>
		public static TInterface Create<TInterface, TImplement>(Action<TimeSpan> durationHandler)
			where TInterface : class
			where TImplement : class, TInterface, new()
		{
			return Create(typeof(TInterface), new TImplement(), durationHandler) as TInterface;
		}


		/// <summary>
		/// A static factory to wrap an instance with this interceptor
		/// </summary>
		/// <typeparam name="TInterface">The interface to proxy</typeparam> 
		/// <param name="instance">The instance to wrap</param>
		/// <param name="durationHandler">An action to handle the duration of the method call</param>
		/// <returns>A wrapped instance</returns>
		public static TInterface Create<TInterface>(TInterface instance, Action<TimeSpan> durationHandler) 
			where TInterface : class
		{
			return Create(typeof(TInterface), instance, durationHandler) as TInterface;
		}


		/// <summary>
		/// This is the internal helper method that actually does the call into Castle.
		/// We are choosing to use the type where you pass in the type instead of the 
		/// generic to avoid calling assemblies having to directly reference Castle libraries
		/// themselves.
		/// </summary>
		/// <param name="interfaceType">The type of the interface</param>
		/// <param name="target">The target to wrap</param>
		/// <param name="durationHandler">An action to handle the duration of the method call</param>
		/// <returns>The interceptor object created</returns>
		private static object Create(Type interfaceType, object target, Action<TimeSpan> durationHandler)
		{
			return _generator.CreateInterfaceProxyWithTarget(interfaceType, target,
															 new DurationTimer(durationHandler));
		}


		/// <summary>
		/// Implementation of the Castle interceptor.  Hidden as a private inner class
		/// to avoid exposing the castle library directly.
		/// </summary>
		private class DurationTimer : IInterceptor
		{
			private Action<TimeSpan> _durationHandler;


			/// <summary>
			/// Constructs an instance of a duration timer with the duration handler given.
			/// </summary>
			/// <param name="durationHandler">The action to call with the duration of the method call</param>
			public DurationTimer(Action<TimeSpan> durationHandler)
			{
				_durationHandler = durationHandler;
			}


			/// <summary>
			/// Intercept functor for each method invocation
			/// </summary>
			/// <param name="invocation">The interface method to be invoked</param>
			public void Intercept(IInvocation invocation)
			{
				var timer = Stopwatch.StartNew();

				// execute method
				invocation.Proceed();

				// if duration handler is not null, send the duration of the method call to it
				if (_durationHandler != null)
				{
					_durationHandler(timer.Elapsed);
				}
			}
		}
	}
}
