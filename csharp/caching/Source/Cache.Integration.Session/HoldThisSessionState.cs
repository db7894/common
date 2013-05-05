using System;
using System.Web.SessionState;

namespace Cache.Integration.Session
{
	/// <summary>
	/// A wrapper that contains the current session state for the
	/// hold this cache.
	/// </summary>
	[Serializable]
	public class HoldThisSessionState
	{
		public int Timeout { get; set; }
		public string SessionId { get; set; }
		public object LockId { get; set; }
		public bool Locked { get; set; }
		public ISessionStateItemCollection SessionItems { get; set; }
		public SessionStateActions StateActions { get; set; }
	}
}
