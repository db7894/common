using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Caching.Tests.Types;
using System.Diagnostics;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// Expires strategy factory.
	/// </summary>
	[TestClass]
	public class ExpiresTests
	{
		[TestMethod]
		public void Expires_Always_Test()
		{
			var strategy = Expires.Always<string>();
			var value = new CachedValue<string>("example", strategy);

			Assert.IsTrue(value.IsExpired);
			Assert.IsTrue(value.IsExpired);
		}

		[TestMethod]
		public void Expires_Never_Test()
		{
			var strategy = Expires.Never<string>();
			var value = new CachedValue<string>("example", strategy);

			Assert.IsFalse(value.IsExpired);
			Assert.IsFalse(value.IsExpired);
		}
		
		[TestMethod]
		public void Expires_NotUsedIn_Test()
		{
			var strategy = Expires.NotUsedIn<string>(TimeSpan.FromSeconds(2));
			var value = new CachedValue<string>("example", strategy);

			Assert.IsFalse(value.IsExpired);
			Thread.Sleep(TimeSpan.FromSeconds(2));
			Assert.IsTrue(value.IsExpired);
		}

		[TestMethod]
		public void Expires_Hits_Test()
		{
			var strategy = Expires.Hits<string>(1);
			var value = new CachedValue<string>("example", strategy);
			
			Assert.IsFalse(value.IsExpired);
			var unwrapped = value.Value; // increment the hits
			Assert.IsTrue(value.IsExpired);
		}

		[TestMethod]
		public void Expires_When_Test()
		{
			var flag = false;
			var strategy = Expires.When<string>(() => flag);
			var value = new CachedValue<string>("example", strategy);

			Assert.IsFalse(value.IsExpired);
			flag = true;
			Assert.IsTrue(value.IsExpired);
		}
		
		[TestMethod]
		public void Expires_NextDay_Test()
		{
			var strategy = Expires.NextDay<string>();
			var value = new CachedValue<string>("example", strategy);
			var secret = new PrivateObject(value);
			Assert.IsFalse(value.IsExpired);
			
			secret.SetProperty("Created", Stopwatch.GetTimestamp() - TimeSpan.TicksPerDay);
			Assert.IsTrue(value.IsExpired);
		}
		
		[TestMethod]
		public void Expires_TimeSpan_Test()
		{
			var strategy = Expires.TimeSpan<string>(TimeSpan.FromDays(-1));
			var value = new CachedValue<string>("example", strategy);
			Assert.IsTrue(value.IsExpired);

			strategy = Expires.TimeSpan<string>(TimeSpan.FromHours(1));
			value = new CachedValue<string>("example", strategy);
			Assert.IsFalse(value.IsExpired);
		}

		[TestMethod]
		public void Expires_At_Test()
		{
			var strategy = Expires.At<string>(DateTime.Now.AddHours(-1));
			var value = new CachedValue<string>("example", strategy);			
			Assert.IsTrue(value.IsExpired);

			strategy = Expires.At<string>(DateTime.Now.AddHours(1));
			value = new CachedValue<string>("example", strategy);
			Assert.IsFalse(value.IsExpired);
		}

		[TestMethod]
		public void Expires_Introspect_Test()
		{
			var strategy = Expires.Introspect<IntrospectionType>();
			var value = new CachedValue<IntrospectionType>(
				new IntrospectionType { Expire = true }, strategy);
			Assert.IsTrue(value.IsExpired);

			strategy = Expires.Introspect<IntrospectionType>();
			value = new CachedValue<IntrospectionType>(
				new IntrospectionType { Expire = false }, strategy);
			Assert.IsFalse(value.IsExpired);
		}
	}
}
