using System.Collections.Generic;
using System.Linq;

namespace SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health
{
	/// <summary>
	/// Factory used to generate pre-populated <see cref="HealthTestResult"/> instances.
	/// </summary>
	public static class HealthTestResultFactory
	{
		/// <summary>
		/// Generates a successful dependency test result.
		/// </summary>
		/// <param name="name">The name of the dependency.</param>
		/// <param name="message">A success message.</param>
		/// <returns>A fully populated <see cref="HealthTestResult"/> instance.</returns>
		public static HealthTestResult GenerateSuccess(string name, string message = "Success")
		{
			return new HealthTestResult
			{
				Name = name,
				IsFunctioning = true,
				ResponseMessages = new List<string> { message },
			};
		}

		/// <summary>
		/// Generates a failed dependency test result.
		/// </summary>
		/// <param name="name">The name of the dependency</param>
		/// <param name="message">The reason for the failure</param>
		/// <returns>A fully populated <see cref="HealthTestResult"/> instance.</returns>
		public static HealthTestResult GenerateFailure(string name, string message = "Failure")
		{
			return new HealthTestResult
			{
				Name = name,
				IsFunctioning = false,
				ResponseMessages = new List<string> { message },
			};
		}
	}
}
