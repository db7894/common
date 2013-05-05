using System;
using System.Web;
using System.Web.SessionState;
using SharedAssemblies.General.Caching;
using System.Collections.Specialized;
using Cache.Integration.Configuration;

namespace Cache.Integration.Session
{
	/// <summary>
	/// An implementation of the session store provider using the hold this
	/// cache as a backend.
	/// </summary>
	public class HoldThisSessionStoreProvider : SessionStateStoreProviderBase 
	{
		private ICache<string, HoldThisSessionState> _cache;
				
		/// <summary>
		///  Gets a brief, friendly description of the session provider
		/// </summary>
		public override string Description
		{
			get { return "A session store provider implemented by the HoldThis Cache"; }
		}

		/// <summary>
		/// Gets the friendly name used to refer to the provider during configuration.
		/// </summary>
		public override string Name
		{
			get { return "HoldThisSessionStoreProvider"; }
		}

		/// <summary>
		/// Initializes the provider.
		/// </summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of configuration values</param>
		public override void Initialize(string name, NameValueCollection config)
		{
			_cache = CacheConfigurationFactory.Initialize<string, HoldThisSessionState>();
		}

		/// <summary>
		/// Initialize a new SessionStateStoreData given the supplied values
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="timeout">The timeout for the specified session</param>
		/// <returns>An initialized session state storage</returns>
		public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
		{
			return new SessionStateStoreData(new SessionStateItemCollection(),
				SessionStateUtility.GetSessionStaticObjects(context), timeout);
		}

		/// <summary>
		/// Create an uninitialized session state for the supplied id
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="id">The unique identifier for this entry</param>
		/// <param name="timeout">The timeout for the specified session</param>
		public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
		{
			_cache.Add(id, new HoldThisSessionState
			{
				Locked = false,
				LockId = 0,
				SessionId = id,
				SessionItems = null,
			});
		}

		/// <summary>
		/// Clean up all handled dependencies
		/// </summary>
		public override void Dispose()
		{
			_cache.Dispose();
		}

		/// <summary>
		/// Callback run after the request is handled
		/// </summary>
		/// <param name="context">The http context for this request</param>
		public override void EndRequest(HttpContext context)
		{ }

		/// <summary>
		/// Retrive the specified session state object
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="id">The unique id for the cached session</param>
		/// <param name="locked">A flag indicating if this session is locked</param>
		/// <param name="lockAge">The current age of the lock</param>
		/// <param name="lockId">The identifier of who is holding the lock</param>
		/// <param name="actions">Any actions for the session state</param>
		/// <returns>The requested session state</returns>
		public override SessionStateStoreData GetItem(HttpContext context, string id,
			out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			return GetItemImplementation(context, id,
				out locked, out lockAge, out lockId, out actions);
		}

		/// <summary>
		/// Retrive the specified session state object
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="id">The unique id for the cached session</param>
		/// <param name="locked">A flag indicating if this session is locked</param>
		/// <param name="lockAge">The current age of the lock</param>
		/// <param name="lockId">The identifier of who is holding the lock</param>
		/// <param name="actions">Any actions for the session state</param>
		/// <returns>The requested session state</returns>
		public override SessionStateStoreData GetItemExclusive(HttpContext context, string id,
			out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			return GetItemImplementation(context, id,
				out locked, out lockAge, out lockId, out actions);
		}

		/// <summary>
		/// Callback run before the request is handled
		/// </summary>
		/// <param name="context">The http context for this request</param>
		public override void InitializeRequest(HttpContext context)
		{ }

		/// <summary>
		/// Release any held locks on the resource
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="id">The unique id for the cached session</param>
		/// <param name="lockId">The identifier of who is holding the lock</param>
		public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
		{
			var handle = _cache.Get(id);
			if (handle != null)
			{
				handle.Locked = false;
				handle.LockId = lockId;
				_cache.Add(id, handle); // push updates
			}
		}

		/// <summary>
		/// Remove an item from the session state cache
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="id">The unique id for the cached session</param>
		/// <param name="lockId">The identifier of who is holding the lock</param>
		/// <param name="item">The session data to remove</param>
		public override void RemoveItem(HttpContext context, string id,
			object lockId, SessionStateStoreData item)
		{
			_cache.Remove(id);
		}

		/// <summary>
		/// Update the timeout for a given cache entry
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="id">The unique identifier for this entry</param>
		public override void ResetItemTimeout(HttpContext context, string id)
		{
			var handle = _cache.Get(id); // touching udpates the timestamp
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <param name="item"></param>
		/// <param name="lockId"></param>
		/// <param name="newItem"></param>
		public override void SetAndReleaseItemExclusive(HttpContext context, string id,
			SessionStateStoreData item, object lockId, bool newItem)
		{
			if (newItem)
			{
				_cache.Add(id, new HoldThisSessionState
				{
					Locked = false,
					LockId = lockId,
					SessionId = id,
					SessionItems = item.Items,
					Timeout = item.Timeout,
				});
			}
			else
			{
				var handle = _cache.Get(id);
				if (handle != null)
				{
					handle.Timeout = item.Timeout;
					handle.Locked = false;
					handle.LockId = lockId;
					handle.SessionItems = item.Items;
					_cache.Add(id, handle); // push the new update
				}
			}
		}

		/// <summary>
		/// Set the callback for when the given item expires
		/// </summary>
		/// <param name="expireCallback">The callback to attach</param>
		/// <returns>The result of the operation</returns>
		public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
		{
			// this is not supported yet, maybe in the future!
			return false;
		}

		#region Private Helper Methods

		/// <summary>
		/// Initialize an existing SessionStateStoreData given the supplied values
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="items">The session items to clone</param>
		/// <param name="timeout">The timeout for the specified session</param>
		/// <returns>An initialized session state storage</returns>
		private SessionStateStoreData RebuildExistingStoreData(HttpContext context,
			SessionStateItemCollection items, int timeout)
		{
			return new SessionStateStoreData(items,
				SessionStateUtility.GetSessionStaticObjects(context), timeout);
		}

		/// <summary>
		/// Retrive the specified session state object
		/// </summary>
		/// <param name="context">The http context for this request</param>
		/// <param name="id">The unique id for the cached session</param>
		/// <param name="locked">A flag indicating if this session is locked</param>
		/// <param name="lockAge">The current age of the lock</param>
		/// <param name="lockId">The identifier of who is holding the lock</param>
		/// <param name="actions">Any actions for the session state</param>
		/// <returns>The requested session state</returns>
		public SessionStateStoreData GetItemImplementation(HttpContext context, string id,
			out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			locked = false;
			lockAge = TimeSpan.Zero;
			lockId = null;
			actions = 0;

			SessionStateStoreData result = null;

			var handle = _cache.Get(id);
			if (handle != null)
			{
				locked = handle.Locked;
				lockId = handle.LockId;
				actions = handle.StateActions;
				result = (actions == SessionStateActions.InitializeItem)
					? CreateNewStoreData(context, int.MinValue)
					: RebuildExistingStoreData(context, (SessionStateItemCollection)handle.SessionItems, handle.Timeout);
			}

			return result;
		}

		#endregion
	}
}
