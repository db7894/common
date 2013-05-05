using System;

namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// Interface to a cache around a single value.
	/// </summary>
	/// <typeparam name="TValue">The type of the underlying cached value</typeparam>
	public interface IObjectCache<TValue> : IDisposable
	{
		/// <summary>
		/// Force a refresh of the cached value
		/// </summary>
		/// <returns>The result of the operation</returns>
		bool Refresh();

		/// <summary>
		/// Retrieve the underlying cached value
		/// </summary>
		/// <returns>The underlying held value</returns>
		TValue Value { get; }

		/// <summary>
		/// Gets the current statistics for the cache
		/// </summary>
		CacheStatistics Statistics { get; }
	}
}
