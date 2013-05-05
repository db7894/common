using System;
using System.Collections.Generic;
using Castle.DynamicProxy;


namespace SharedAssemblies.General.Memoizers
{
	/// <summary>
	/// The MethodCallMemorizer is used to cache calls to methods in order to remember their
	/// results if they are called with the same set of inputs multiple times.
	/// </summary>
	public static class GlobalMemoizer<TInterface, TImplement>
		where TInterface : class
		where TImplement : class, TInterface, new()
	{
		// Handle to our proxy generator
		private static readonly ProxyGenerator _generator = new ProxyGenerator();

		// handle and lock to pseudo-singleton 
		private static readonly object _instanceLock = new object();
		private static TInterface _instance;
		
		// time that a cached response can live before expiring
		private static TimeSpan _cacheInterval = TimeSpan.FromSeconds(5.0);

		// time to clean up expired cached items.
		private static TimeSpan _cleanUpInterval = TimeSpan.FromSeconds(60.0);

		// list of methods to intercept, null == all methods.
		private static IEnumerable<string> _methodsToCache = null;

		/// <summary>
		/// Gets or sets the interval that items in the cache are considered fresh.
		/// </summary>
		public static TimeSpan CacheInterval
		{
			get
			{
				lock (_instanceLock)
				{
					return _cacheInterval;
				}
			}

			set
			{
				lock (_instanceLock)
				{
					if (_instance != null)
					{
						throw new InvalidOperationException("Can't set CacheInterval after Instance has been called.");
					}

					_cacheInterval = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the interval between clean-up passes that remove stale cache data.
		/// </summary>
		public static TimeSpan CleanUpInterval
		{
			get
			{
				lock (_instanceLock)
				{
					return _cleanUpInterval;
				}
			}

			set
			{
				lock (_instanceLock)
				{
					if (_instance != null)
					{
						throw new InvalidOperationException("Can't set CleanUpInterval after Instance has been called.");
					}

					_cleanUpInterval  = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the list of methods to cache, if null assumes all methods.
		/// </summary>
		public static IEnumerable<string> MethodsToCache
		{
			get
			{
				lock (_instanceLock)
				{
					return _methodsToCache;
				}
			}

			set
			{
				lock (_instanceLock)
				{
					if (_instance != null)
					{
						throw new InvalidOperationException("Can't set MethodsToCache after Instance has been accessed.");
					}

					_methodsToCache = value;
				}
			}
		}

		/// <summary>
		/// Returns the global memorizer for this interface/implementation combination that will examine 
		/// method calls and use a cache to store the request if subsequent requests with the same arguments 
		/// are passed multiple times within the given time span.
		/// </summary>
		/// <returns>A wrapped instance</returns>
		public static TInterface Instance
		{
			get { return GetInstance(); }
		}

		/// <summary>
		/// Creates the instance of interceptor for the interface and returns it
		/// </summary>
		/// <returns>New instance of the interceptor that satisfies the interface.</returns>
		private static TInterface GetInstance()
		{
			// if already set, don't need to even lock to return
			if (_instance == null)
			{
				// not set, so get lock so we can set it
				lock (_instanceLock)
				{
					// even if we get the lock, we may have been a queued request when it was null but not null now
					if (_instance == null)
					{
						// as long as the cache interval is positive (>0) we will intercept
						if (_cacheInterval.Ticks > 0)
						{
							// if still null, create and set new instance
							_instance = _generator.CreateInterfaceProxyWithTarget(typeof(TInterface), new TImplement(),
								new MethodCallCachingInterceptor(CacheInterval, CleanUpInterval, MethodsToCache))
								   as TInterface;
						}

						// if cache interval is negative or zero, then we are a pass-through to implementation
						else
						{
							_instance = new TImplement();
						}
					}
				}
			}

			return _instance;
		}
	}
}
