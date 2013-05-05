using System.Collections.Generic;
using SharedAssemblies.General.Database;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A collection of configuration options to be used by the abstract health service.
	/// </summary>
	public sealed class DatabaseHealthConfig
	{
		/// <summary>
		/// The handle to IDatabaseUtility that we will use to test the
		/// requested procedures.
		/// </summary>
		public IDatabaseUtility Database { get; set; }

		/// <summary>
		/// The collection of procedures that we would like to test
		/// </summary>
		public IEnumerable<string> Procedures { get; set;  }
	}
}