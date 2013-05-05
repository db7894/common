using System;
using System.Diagnostics.CodeAnalysis;


namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// Abstract base class representing a resource to retrieve
	/// bashwork encryption keys
	/// </summary>
	/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.NamingAnalyzer",
		"ST3003:AbstractClassesMustStartWithAbstract", 
		Justification = "Legacy name, would break dependent classes so obsoleting.")]
	[Obsolete("Use AbstractKeyManager instead.", true)]
	public abstract class KeyManager : AbstractKeyManager
	{
	}
}