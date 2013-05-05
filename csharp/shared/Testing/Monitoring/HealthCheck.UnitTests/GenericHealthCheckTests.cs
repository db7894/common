using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Testing;
using SharedAssemblies.Monitoring.HealthCheck.Utilities.Health;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;
using System.Collections.Generic;

namespace SharedAssemblies.Monitoring.HealthCheck
{
	/// <summary>
	/// A collection of code to test the functionality of the GenericHealthCheck
	/// </summary>
	[TestClass]
	public class GenericHealthCheckTests
	{
		[TestMethod]
		public void CreateGenericCheck_WithBool_WithBadParameters_Works()
		{
			var check = new GenericHealthCheck(() => true);
			Assert.IsNotNull(check);

			AssertEx.Throws(() => new GenericHealthCheck((Func<bool>)null));
		}

		[TestMethod]
		public void CreateGenericCheck_WithTuple_WithBadParameters_Works()
		{
			var check = new GenericHealthCheck(() => Tuple.Create(true, "success"));
			Assert.IsNotNull(check);

			AssertEx.Throws(() => new GenericHealthCheck((Func<Tuple<bool, string>>)null));
		}

		[TestMethod]
		public void CreateGenericCheck_WithBool_Defaults_Work()
		{
			var value = true;
			var check = new GenericHealthCheck(() => value, "CheckName", "ErrorValue");
			var result = check.PerformHealthCheck();
			var expected = new HealthTestResult
			{
				IsFunctioning = true,
				Name = "CheckName",
				ResponseMessages = new List<string> { "Success" },
			};
			AssertEx.AreEqual(expected, result);

			value = false;
			result = check.PerformHealthCheck();
			expected = new HealthTestResult
			{
				IsFunctioning = false,
				Name = "CheckName",
				ResponseMessages = new List<string> { "ErrorValue" },
			};
			AssertEx.AreEqual(expected, result);

			value = true;
			check = new GenericHealthCheck(() => value);
			result = check.PerformHealthCheck();
			expected = new HealthTestResult
			{
				IsFunctioning = true,
				Name = "GenericHealthCheck",
				ResponseMessages = new List<string> { "Success" },
			};
			AssertEx.AreEqual(expected, result);

			value = false;
			result = check.PerformHealthCheck();
			expected = new HealthTestResult
			{
				IsFunctioning = false,
				Name = "GenericHealthCheck",
				ResponseMessages = new List<string> { "The supplied health check failed" },
			};
			AssertEx.AreEqual(expected, result);
		}

		[TestMethod]
		public void CreateGenericCheck_WithBool_WorksCorrectly()
		{
			var value = true;
			Func<bool> predicate = () => value;
			var check = new GenericHealthCheck(predicate, "Example");

			Assert.IsNotNull(check);
			Assert.IsTrue(check.IsHealthy());

			value = false;
			Assert.IsFalse(check.IsHealthy());
		}

		[TestMethod]
		public void CreateGenericCheck_WithTuple_WorksCorrectly()
		{
			var value = true;
			Func<Tuple<bool, string>> predicate = () => Tuple.Create(value, "success");
			var check = new GenericHealthCheck(predicate, "Example");

			Assert.IsNotNull(check);
			Assert.IsTrue(check.IsHealthy());

			value = false;
			Assert.IsFalse(check.IsHealthy());
		}

		[TestMethod]
		public void CreateGenericCheck_WithException_WorksCorrectly()
		{
			Func<bool> predicate = () => {
				throw new Exception("Something");
			};
			var check = new GenericHealthCheck(predicate, "Example");
			var message = "Something";
			var result  = check.PerformHealthCheck();

			Assert.AreEqual(result.Name, "Example");
			Assert.IsFalse(result.IsFunctioning);
			Assert.AreEqual(result.ResponseMessages.First(), message);
		}
	}
}
