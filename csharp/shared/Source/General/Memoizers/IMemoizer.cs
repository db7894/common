namespace SharedAssemblies.General.Memoizers
{
	/// <summary>
	/// This interface is supported by the MethoCallCachingInterceptor to allow access to statistics
	/// and manipulation of the cache.
	/// </summary>
	public interface IMemoizer
	{
		/// <summary>
		/// Gets the number of hits on the Memoizer that resulted in a successful cache pull.
		/// </summary>
		int MemoizerHitCount { get; }

		/// <summary>
		/// Gets the number of misses on the Memoizer that resulted in a call to the wrapped object.
		/// </summary>
		int MemoizerMissCount { get; }

		/// <summary>
		/// Gets the number of objects being held in the Memoizer cache.
		/// </summary>
		int MemoizerCacheSize { get; }

		/// <summary>
		/// Clears the Memoizer cache of all cached method call data.
		/// </summary>
		void ClearMemoizerCache();

		/// <summary>
		/// Clears the hit and miss counts of the memoizer.
		/// </summary>
		void ClearMemoizerCounts();
	}
}
