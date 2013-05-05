using System.Collections.Generic;
using log4net;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A collection of configuration options to be used by the abstract
	/// health service.
	/// </summary>
	public sealed class ServiceHealthConfig
	{
		/// <summary>
		/// The logger to use for persisting errors
		/// </summary>
		public ILog Logger { get; set; }

		/// <summary>
		/// A collection of the health checks that should be run for this service
		/// </summary>
		public IEnumerable<IHealthCheck> HealthChecks { get; set; }

		/// <summary>
		/// The current location of the configuration file for this service
		/// </summary>
		public string ConfigurationFile { get; set; }
	}
}
