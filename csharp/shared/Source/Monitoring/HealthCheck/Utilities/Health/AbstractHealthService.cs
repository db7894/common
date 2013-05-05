using System;
using System.Linq;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A service contract that can be used to retrieve the current status
	/// of a given application.
	/// </summary>
	public abstract class AbstractHealthService : IHealthServiceContract
	{
		/// <summary>
		/// Handle to the service health configuration
		/// </summary>
		protected abstract ServiceHealthConfig Health { get; set; }

		/// <summary>
		/// Get the current statistics of the application and its dependencies.
		/// </summary>
		/// <returns>The result of the operation</returns>
		public bool IsSystemHealthy()
		{
			return GetApplicationHealth().IsFunctioning;
		}

		/// <summary>
		/// Retrieve the current health of the application and its dependencies.
		/// </summary>
		/// <returns>The result of the operation</returns>
		public GetApplicationHealthResponse GetApplicationHealth()
		{
			var response = new GetApplicationHealthResponse();

			try
			{
				response.Dependencies = Health.HealthChecks.NullSafe()
					.Select(test => test.PerformHealthCheck()).ToList();
				response.IsFunctioning = response.Dependencies.All(test => test.IsFunctioning);
			}
			catch (Exception ex)
			{
				Health.Logger.Debug("Error determining the service health", ex);
				response = GetApplicationHealthResponseFactory.GenerateFailure();
			}

			return response;
		}

		/// <summary>
		/// Get the current statistics of the application and its dependencies.
		/// </summary>
		/// <returns>The result of the operation</returns>
		public GetApplicationVersionsResponse GetApplicationVersions()
		{
			var response = new GetApplicationVersionsResponse();

			try
			{
				response.Versions = HealthVersionUtility.GetFromCurrentAppDomain();
			}
			catch (Exception ex)
			{
				Health.Logger.Debug("Error determining the service health", ex);
			}

			return response;
		}

		/// <summary>
		/// Get the current statistics of the application and its dependencies.
		/// </summary>
		/// <returns>The result of the operation</returns>
		public GetApplicationConfigurationResponse GetApplicationConfiguration()
		{
			var response = new GetApplicationConfigurationResponse();

			try
			{
				response.Configurations = HealthConfigUtility.GetFromAppConfig(Health.ConfigurationFile);
			}
			catch (Exception ex)
			{
				Health.Logger.Debug("Error determining the service health", ex);
			}

			return response;
		}
	}
}

