using System.Collections.Generic;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// Factory used to generate pre-propulated <see cref="GetApplicationHealthResponse"/> instances.
	/// </summary>
	public static class GetApplicationHealthResponseFactory
	{
		/// <summary>
		/// Generates a successful application health test result.
		/// </summary>
		/// <returns>A fully populated <see cref="GetApplicationHealthResponse"/> instance.</returns>
		public static GetApplicationHealthResponse GenerateSuccess()
		{
			return new GetApplicationHealthResponse
			{
				IsFunctioning = true,
				Dependencies = new List<HealthTestResult>(),
			};
		}

		/// <summary>
		/// Generates a failed application health test result.
		/// </summary>
		/// <returns>A fully populated <see cref="GetApplicationHealthResponse"/> instance.</returns>
		public static GetApplicationHealthResponse GenerateFailure()
		{
			return new GetApplicationHealthResponse
			{
				IsFunctioning = false,
				Dependencies = new List<HealthTestResult>(),
			};
		}
	}
}
