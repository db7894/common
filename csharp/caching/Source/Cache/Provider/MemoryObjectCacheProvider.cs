
namespace SharedAssemblies.General.Caching.Provider
{
	/// <summary>
	/// Implementation of a object cache provider that stores
	/// the cached value in memory.
	/// </summary>
	/// <typeparam name="TValue">The value to manage</typeparam>
	public class MemoryObjectCacheProvider<TValue> : IObjectCacheProvider<TValue>
	{
		private volatile CachedValue<TValue> _value;

		/// <summary>
		/// Initialize a new instance of the MemoryObjectCacheProvider
		/// </summary>
		/// <param name="value">The default value for the provider</param>
		public MemoryObjectCacheProvider(TValue value = default(TValue))
		{
			_value = new CachedValue<TValue>(value);
		}

		/// <summary>
		/// Get or set the value managed by the provider
		/// </summary>
		public TValue Value
		{
			// reads and writes of reference types are guaranteed to be atomic
			get { return _value.Value; }
			set { _value = new CachedValue<TValue>(value); }
		}

		/// <summary>
		/// Cleanup all the managed resources
		/// </summary>
		public void Dispose()
		{}			
	}

	/// <summary>
	/// Companion object for the MemoryObjectCacheProvider type
	/// </summary>
	public static class MemoryObjectCacheProvider
	{
		/// <summary>
		/// Create a new instance of the MemoryObjectCacheProvider class
		/// </summary>
		/// <typeparam name="TValue">The type of the underlying value</typeparam>
		/// <param name="value">The default value for the provider</param>
		/// <returns>An initialized IObjectCacheProvider</returns>
		public static IObjectCacheProvider<TValue> Create<TValue>(TValue value)
		{
			return new MemoryObjectCacheProvider<TValue>(value);
		}
	}
}
