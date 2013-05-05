
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Health
{
	/// <summary>
	/// An interface to be used by a third party dependency to
	/// examine the current health of said dependency.
	/// </summary>
	public interface IHealthCheck
	{
		/// <summary>
		/// A quick check that indicates if the current dependency is healthy
		/// </summary>
		/// <returns>true if healthy, false otherwise</returns>
		bool IsHealthy();

		/// <summary>
		/// A verbose check of the system resource
		/// </summary>
		/// <returns>A successful response if healthy, a failure otherwise</returns>
		HealthTestResult PerformHealthCheck();
	}
}
