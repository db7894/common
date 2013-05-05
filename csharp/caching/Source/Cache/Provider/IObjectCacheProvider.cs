using System;

namespace SharedAssemblies.General.Caching.Provider
{
	/// <summary>
	/// Interface to a single object cache provider.
	/// </summary>
	/// <typeparam name="TValue">The value to manage</typeparam>
	public interface IObjectCacheProvider<TValue> : IDisposable
	{
		/// <summary>
		/// Get or set the underlying cache value
		/// </summary>
		/// <returns>The underlying held value</returns>
		TValue Value { get; set; }
	}
}
