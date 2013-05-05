using System;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Health;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A healper class to implement a one shot health check
	/// </summary>
	public sealed class GenericHealthCheck : IHealthCheck
	{
		/// <summary>
		/// The name of the health check to appear to the consumer
		/// </summary>
		private string _name;

		/// <summary>
		/// The health check to perform to see if the resouce is functioning
		/// </summary>
		private Func<Tuple<bool, string>> _predicate;

		/// <summary>
		/// Construct a new instance of the GenericHealthCheck
		/// </summary>		
		/// <param name="predicate">The test to execute</param>
		/// <param name="name">The name of the generic health check</param>
		/// <param name="error">The test to execute</param>
		public GenericHealthCheck(Func<bool> predicate, string name = null, string error = null)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}

			_name = name ?? "GenericHealthCheck";
			error = error ?? "The supplied health check failed";
			_predicate = () => Tuple.Create(predicate(), error);
		}

		/// <summary>
		/// Construct a new instance of the GenericHealthCheck
		/// </summary>		
		/// <param name="predicate">The test to execute</param>
		/// <param name="name">The name of the generic health check</param>
		public GenericHealthCheck(Func<Tuple<bool, string>> predicate, string name = null)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}

			_name = name ?? "GenericHealthCheck";
			_predicate = predicate;
		}

		/// <summary>
		/// A quick check that indicates if the current dependency is healthy
		/// </summary>
		/// <returns>true if healthy, false otherwise</returns>
		public bool IsHealthy()
		{
			return PerformHealthCheck().IsFunctioning;
		}

		/// <summary>
		/// A verbose check of the system resource
		/// </summary>
		/// <returns>A successful response if healthy, a failure otherwise</returns>
		public HealthTestResult PerformHealthCheck()
		{
			HealthTestResult response;

			try
			{
				var check = _predicate();
				response = (check.Item1)
					? HealthTestResultFactory.GenerateSuccess(_name)
					: HealthTestResultFactory.GenerateFailure(_name, check.Item2);
			}
			catch (Exception ex)
			{
				response = HealthTestResultFactory.GenerateFailure(_name, ex.Message);
			}

			return response;
		}
	}
}
