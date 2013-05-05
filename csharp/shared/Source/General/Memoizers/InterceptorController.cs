namespace SharedAssemblies.General.Memoizers
{
	/// <summary>
	/// Helper class that is used to control the interceptor
	/// </summary>
	internal class InterceptorController : IMemoizer
	{
		private MethodCallCachingInterceptor _interceptor;

		/// <summary>
		/// Gets the number of hits on the Memoizer that resulted in a successful cache pull.
		/// </summary>
		public int MemoizerHitCount
		{
			get { return _interceptor.HitCount; }
		}

		/// <summary>
		/// Gets the number of misses on the Memoizer that resulted in a call to the wrapped object.
		/// </summary>
		public int MemoizerMissCount
		{
			get { return _interceptor.MissCount; }
		}

		/// <summary>
		/// Gets the number of objects being held in the Memoizer cache.
		/// </summary>
		public int MemoizerCacheSize
		{
			get { return _interceptor.CacheSize; }
		}

		/// <summary>
		/// Constructs a new instance of the interceptor controller given an interceptor.
		/// </summary>
		/// <param name="interceptor"></param>
		public InterceptorController(MethodCallCachingInterceptor interceptor)
		{
			_interceptor = interceptor;
		}

		/// <summary>
		/// Clears the Memoizer cache of all cached method call data.
		/// </summary>
		public void ClearMemoizerCache()
		{
			_interceptor.ClearCache();
		}

		/// <summary>
		/// Clears the hit and miss counts of the memoizer.
		/// </summary>
		public void ClearMemoizerCounts()
		{
			_interceptor.ClearCounts();
		}
	}
}
