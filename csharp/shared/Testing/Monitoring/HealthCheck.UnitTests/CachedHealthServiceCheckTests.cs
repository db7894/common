using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Testing;
using SharedAssemblies.Monitoring.HealthCheck.Utilities.Health;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;
using System.Collections.Generic;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Health;

namespace SharedAssemblies.Monitoring.HealthCheck
{
	/// <summary>
	/// A collection of code to test the functionality of the CachedHealthServiceCheck
	/// </summary>
	[TestClass]
	public class CachedHealthServiceCheckTests
	{
		[TestMethod]
		public void Create_WithBadParameters_Fails()
		{
			Func<GetApplicationHealthResponse> check = null;
			IEnumerable<IHealthCheck> checks = null;
			IEnumerable<IHealthCheck> empty = new List<IHealthCheck>();

			AssertEx.Throws(() => new CachedHealthServiceCheck(check, TimeSpan.FromSeconds(1)));
			AssertEx.Throws(() => new CachedHealthServiceCheck(checks, TimeSpan.FromSeconds(1)));
			AssertEx.Throws(() => new CachedHealthServiceCheck(empty, TimeSpan.FromSeconds(1)));
		}

		/// <summary>
		/// random numbers with negative parameters
		/// </summary>
		[TestMethod]
		public void HealthCheck_IsCached_Correctly()
		{
			var value = true;
			Func<GetApplicationHealthResponse> test = () => new GetApplicationHealthResponse
			{
				IsFunctioning = value,
				Dependencies = new List<HealthTestResult>(),
			};

			var check = new CachedHealthServiceCheck(test, TimeSpan.FromSeconds(1));
			Assert.IsTrue(check.IsSystemHealthy());
			value = false;
			Assert.IsTrue(check.IsSystemHealthy());
			System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
			Assert.IsFalse(check.IsSystemHealthy());
		}
	}
}
