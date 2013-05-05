
namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// A collection of delegates that are missing for one reason or another
	/// from the BCL.
	/// </summary>
	public static class CommonDelegates
	{
		/// <summary>
		/// Represents a method that checks the state of some criteria in the system
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public delegate bool Predicate();
	}
}
