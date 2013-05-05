using System;
using System.Web.Caching;
using SharedAssemblies.General.Caching;
using Cache.Integration.Configuration;
using System.Collections.Specialized;

namespace Cache.Integration.Session
{
	/// <summary>
	/// An implementation of the session store provider using the hold this
	/// cache as a backend.
	/// </summary>
	public class HoldThisOutputCacheProvider : OutputCacheProvider
	{
		private ICache<string, object> _cache;

		/// <summary>
		///  Gets a brief, friendly description of the session provider
		/// </summary>
		public override string Description
		{
			get { return "A output cache provider implemented by the HoldThis Cache"; }
		}

		/// <summary>
		/// Gets the friendly name used to refer to the provider during configuration.
		/// </summary>
		public override string Name
		{
			get { return "HoldThisOutputCacheProvider"; }
		}

		/// <summary>
		/// Initializes the provider.
		/// </summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of configuration values</param>
		public override void Initialize(string name, NameValueCollection config)
		{
			_cache = CacheConfigurationFactory.Initialize<string, object>();
		}

		/// <summary>
		/// Add a new value at the specified key
		/// </summary>
		/// <param name="key">The key to add a value at</param>
		/// <param name="entry">The value to add at the specified key</param>
		/// <param name="utcExpiry">The expiration of the value</param>
		/// <returns>The resulting added value</returns>
		public override object Add(string key, object entry, DateTime utcExpiry)
		{
			_cache.Add(key, entry);
			return entry;
		}

		/// <summary>
		/// Get the value at the specified key
		/// </summary>
		/// <param name="key">The key to get a value at</param>
		/// <returns>The requested value or null</returns>
		public override object Get(string key)
		{
			return _cache.Get(key);
		}

		/// <summary>
		/// Remove the value at the specified key
		/// </summary>
		/// <param name="key">The key to remove a value at</param>
		public override void Remove(string key)
		{
			_cache.Remove(key);
		}

		/// <summary>
		/// Set the value at the supplied key to the supplied value
		/// </summary>
		/// <param name="key">The key to set a value at</param>
		/// <param name="entry">The value to add at the specified key</param>
		/// <param name="utcExpiry">The expiration of the value</param>
		public override void Set(string key, object entry, DateTime utcExpiry)
		{
			_cache.Add(key, entry);
		}
	}
}
