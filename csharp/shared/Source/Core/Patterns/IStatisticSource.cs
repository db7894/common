using System.Collections.Generic;


namespace SharedAssemblies.Core.Patterns
{
	/// <summary>
	/// Interface that specifies that the implementer is a source of statistics that can be queried.
	/// </summary>
	public interface IStatisticSource
	{
		/// <summary>
		/// Queries the statistics from the implementer, which are returned back as an enumeration of KVPs.
		/// </summary>
		/// <returns></returns>
		IDictionary<string, object> QueryStatistics();

		/// <summary>
		/// Clears any statistics to their default levels.
		/// </summary>
		void ClearStatistics();
	}
}
