using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.HealthCheck.Utilities.Health;

namespace SharedAssemblies.Monitoring.HealthCheck
{
	/// <summary>
	/// A collection of code to test the functionality of the HealthCheckUtility
	/// </summary>
	[TestClass]
	public class HealthCheckUtilityTests
	{
		[TestMethod]
		public void CreateGenericCheck_WithBool_WorksCorrectly()
		{
			var value = true;
			Func<bool> predicate = () => value;
			var check = HealthCheckUtility.CreateGenericCheck(predicate, "Example");

			Assert.IsNotNull(check);
			Assert.IsTrue(check.IsHealthy());

			value = false;
			Assert.IsFalse(check.IsHealthy());
		}

		[TestMethod]
		public void CreateGenericCheck_WithTule_WorksCorrectly()
		{
			var value = true;
			Func<Tuple<bool, string>> predicate = () => Tuple.Create(value, "Error");
			var check = HealthCheckUtility.CreateGenericCheck(predicate, "Example");

			Assert.IsNotNull(check);
			Assert.IsTrue(check.IsHealthy());

			value = false;
			Assert.IsFalse(check.IsHealthy());
		}

		[TestMethod]
		public void CreateFileExistsCheck_WorksCorrectly()
		{
			var file1 = @"C:\WINDOWS\explorer.exe";
			var check1 = HealthCheckUtility.CreateFileExistsCheck(file1);
			Assert.IsNotNull(check1);
			Assert.IsTrue(check1.IsHealthy());

			var file2 = @"C:\a\b\c\d\e\f\g";
			var check2 = HealthCheckUtility.CreateFileExistsCheck(file2);
			Assert.IsNotNull(check2);
			Assert.IsFalse(check2.IsHealthy());
		}
				
		[TestMethod]
		public void CreateHostExistsCheck_WorksCorrectly()
		{
			var hostname1 = "localhost";
			var check1 = HealthCheckUtility.CreateHostExistsCheck(hostname1, TimeSpan.FromSeconds(1));
			Assert.IsNotNull(check1);
			Assert.IsTrue(check1.IsHealthy());

			var hostname2 = "169.0.0.1";
			var check2 = HealthCheckUtility.CreateHostExistsCheck(hostname2, TimeSpan.FromSeconds(1));
			Assert.IsNotNull(check2);
			Assert.IsFalse(check2.IsHealthy());
		}

		[TestMethod]
		[Ignore] // I don't know where this will be run
		public void CreatePortAvailableCheck_WorksCorrectly()
		{
			var hostname1 = "www.bashwork.com";
			var check1 = HealthCheckUtility.CreatePortOpenCheck(hostname1, 80);
			Assert.IsNotNull(check1);
			Assert.IsTrue(check1.IsHealthy());

			var hostname2 = "169.0.0.1";
			var check2 = HealthCheckUtility.CreatePortOpenCheck(hostname2, 80);
			Assert.IsNotNull(check2);
			Assert.IsFalse(check2.IsHealthy());
		}
	}
}
