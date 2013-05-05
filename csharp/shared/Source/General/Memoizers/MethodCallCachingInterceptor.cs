using System;
using System.Linq;
using System.Collections.Generic;
using SharedAssemblies.General.Caching;
using Castle.Core.Interceptor;


namespace SharedAssemblies.General.Memoizers
{
	/// <summary>
	/// The actual Castle interceptor that intercepts method calls
	/// </summary>
	internal sealed class MethodCallCachingInterceptor : IInterceptor
	{
		private readonly Cache<MethodCall, object> _cache;
		private readonly IEnumerable<string> _methods;
		private volatile int _hits;
		private volatile int _misses;

		/// <summary>
		/// Gets the number of hits on the Memoizer that resulted in a successful cache pull.
		/// </summary>
		public int HitCount { get { return _hits; } }

		/// <summary>
		/// Gets the number of misses on the Memoizer that resulted in a call to the wrapped object.
		/// </summary>
		public int MissCount { get { return _misses; } }

		/// <summary>
		/// Gets the number of objects being held in the Memoizer cache.
		/// </summary>
		public int CacheSize { get { return _cache.Count; } }

		/// <summary>
		/// Constructs the interceptor given the time spans and methods.
		/// </summary>
		/// <param name="cacheInterval">The interval to cache the method result.</param>
		/// <param name="cleanInterval">The interval to clean the cache.</param>
		/// <param name="methods">The methods to intercept.</param>
		public MethodCallCachingInterceptor(TimeSpan cacheInterval, TimeSpan cleanInterval, IEnumerable<string> methods)
		{
			_methods = methods;

			// define the cache to be for MethodCall (name + arguments) with an
			// expiration time span of 
			_cache = new Cache<MethodCall, object>(
				new TimeSpanExpirationStrategy<object>(cacheInterval), cleanInterval);
		}

		/// <summary>
		/// Clears the Memoizer cache of all cached method call data.
		/// </summary>
		public void ClearCache()
		{
			_cache.Clear();
		}

		/// <summary>
		/// Clears the hit and miss counts of the memoizer.
		/// </summary>
		public void ClearCounts()
		{
			_hits = 0;
			_misses = 0;
		}

		/// <summary>
		/// Intercept function for each method invocation
		/// </summary>
		/// <param name="invocation">The interface method to be invoked</param>
		public void Intercept(IInvocation invocation)
		{
			CachedItem<object> returnValue = null;

			// if we are intercepting this method, check cache
			if (_methods == null || _methods.Contains(invocation.Method.Name))
			{
				var call = new MethodCall
				{
					MethodName = invocation.Method.Name,
					Arguments = invocation.Arguments
				};

				returnValue = _cache.Get(call);

				// if there was no cache item or it has expired, call method
				if (returnValue == null || returnValue.IsExpired)
				{
					invocation.Proceed();

					_cache.Add(call, invocation.ReturnValue);

					unchecked
					{
						++_misses;
					}
				}

				// otherwise cache is still valid, return cached result
				else
				{
					invocation.ReturnValue = returnValue.Value;

					unchecked
					{
						++_hits;
					}
				}
			}

			// if method is not being intercepted, just proceed
			else
			{
				invocation.Proceed();
			}
		}
	}
}
