using System;

namespace ArmTrap
{
	[Flags]
	public enum FeatureFlags
	{
		Logging = 0x01,
		Dump    = 0x02, // TODO
		ForceTermination = 0x04 // TODO
	}
}
