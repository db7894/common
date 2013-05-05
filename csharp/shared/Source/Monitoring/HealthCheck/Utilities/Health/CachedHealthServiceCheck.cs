using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Health;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A healper class to implement a one shot health check
	/// </summary>
	public sealed class CachedHealthServiceCheck : IHealthServiceContract
	{
		/// <summary>
		/// The health check to perform to see if the resouce is functioning
		/// </summary>
		private readonly Func<GetApplicationHealthResponse> _healthCheck;

		/// <summary>
		/// The current cached value of the health check
		/// </summary>
		private GetApplicationHealthResponse _current = null;
		private readonly long _refreshDelay;
		private long _lastRefresh;

		/// <summary>
		/// Initailize a CachedHealthServiceCheck class with the supplied checks
		/// </summary>
		/// <param name="checks">The health checks to perform</param>
		/// <param name="delay">The delay between health check runs</param>
		public CachedHealthServiceCheck(IEnumerable<IHealthCheck> checks, TimeSpan delay)
		{
			if (checks == null || checks.Count() == 0)
			{
				throw new ArgumentNullException("checks");
			}
			_healthCheck = WrapHealthCheck(checks);
			_refreshDelay = delay.Ticks;
		}

		/// <summary>
		/// Initailize a CachedHealthServiceCheck class with the supplied check
		/// </summary>
		/// <param name="check">The health check to perform</param>
		/// <param name="delay">The delay between health check runs</param>
		public CachedHealthServiceCheck(Func<GetApplicationHealthResponse> check, TimeSpan delay)
		{
			if (check == null)
			{
				throw new ArgumentNullException("check");
			}
			_healthCheck = check;
			_refreshDelay = delay.Ticks;
		}

		/// <summary>
		/// A quick check that indicates if the current dependency is healthy
		/// </summary>
		/// <returns>true if healthy, false otherwise</returns>
		public bool IsSystemHealthy()
		{
			return GetApplicationHealth().IsFunctioning;
		}

		/// <summary>
		/// A verbose check of the system resource
		/// </summary>
		/// <returns>A successful response if healthy, a failure otherwise</returns>
		public GetApplicationHealthResponse GetApplicationHealth()
		{
			var timestamp = Stopwatch.GetTimestamp();

			if ((_current == null) || !_current.IsFunctioning || (timestamp - _lastRefresh) >= _refreshDelay)
			{
				_current = _healthCheck();
				_lastRefresh = timestamp;
			}

			return _current;
		}

		#region Private Helper Methods

		/// <summary>
		/// A helper method to build a health check function given a collection of checks
		/// </summary>
		/// <param name="checks">The checks to wrap a function around</param>
		/// <returns></returns>
		private static Func<GetApplicationHealthResponse> WrapHealthCheck(IEnumerable<IHealthCheck> checks)
		{
			return (/*nothing*/) => {
				var results = checks.Select(check => check.PerformHealthCheck()).ToList();

				return new GetApplicationHealthResponse
				{
					Dependencies = results,
					IsFunctioning = results.All(result => result.IsFunctioning),
				};
			};
		}

		#endregion
	}
}
