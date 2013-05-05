using System.ComponentModel;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;


namespace $safeprojectname$
{
	/// <summary>
	/// This creates an auto counter installer local in this project that will be
	/// invoked when the installutil .NET Framework utility is run against this executable.
	/// </summary>
	[RunInstaller(true)]
	public class $safeprojectname$AutoCounterInstaller : AutoCounterInstaller
	{
		// no members need be created, the inheritence is sufficient.
	}
}
