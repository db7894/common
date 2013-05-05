using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace SharedAssemblies.General.Caching.Cleaning.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// CacheJanitor.
	/// </summary>
	[TestClass]
	public class CacheJantitorTests
	{
		[TestMethod]
		public void Janitor_Initializes_Correctly()
		{
			var janitor = CacheJanitor.Instance;
			Assert.IsNotNull(janitor);
		}

		[TestMethod]
		public void Janitor_RegistersAndStarts_Correctly()
		{
			var janitor = CacheJanitor.Instance;
			var secret = new PrivateObject(janitor);
			var cache = new CheckAndSetCache<string, object>(Expires.Always<object>());

			Assert.AreEqual(TaskStatus.Created, ((Task)secret.GetField("_janitor")).Status);
			janitor.Register(cache, Cleanup.Expired<string, object>(cache));
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			Assert.AreEqual(TaskStatus.Running, ((Task)secret.GetField("_janitor")).Status);
			janitor.Unregister(cache);
		}

		[TestMethod]
		public void Janitor_RegistersTwice_Correctly()
		{
			var janitor = CacheJanitor.Instance;
			var secret = new PrivateObject(janitor);
			var cache = new CheckAndSetCache<string, object>(Expires.Always<object>());
			var hashcode = cache.GetHashCode();

			janitor.Register(cache, Cleanup.Expired<string, object>(cache));
			janitor.Register(cache, Cleanup.Expired<string, object>(cache));
			// TODO validate later
			Assert.IsTrue(janitor.Unregister(cache));
		}

		[TestMethod]
		public void Janitor_AdjustsFrequency_Correctly()
		{
			var janitor = CacheJanitor.Instance;
			var secret = new PrivateObject(janitor);
			var cache1 = new CheckAndSetCache<string, object>(Expires.Always<object>());
			var cache2 = new CheckAndSetCache<string, object>(Expires.Always<object>());
			var cleanup1 = Cleanup.Expired<string, object>(cache1);
			var cleanup2 = Cleanup.Expired<string, object>(cache2);

			cleanup1.Options = new CleanupOptions
			{
				Frequency = TimeSpan.FromMilliseconds(200),
			};
			cleanup2.Options = new CleanupOptions
			{
				Frequency = TimeSpan.FromMilliseconds(100),
			};

			// starts with frequency of 200
			janitor.Register(cache1, cleanup1);
			Assert.AreEqual(cleanup1.Options.Frequency, (TimeSpan)secret.GetField("SleepTime"));

			// starts with frequency of 100
			janitor.Register(cache2, cleanup2);
			Assert.AreEqual(cleanup2.Options.Frequency, (TimeSpan)secret.GetField("SleepTime"));

			janitor.Unregister(cache1);
			janitor.Unregister(cache2);
		}

		[TestMethod]
		public void Janitor_CleansUp_Correctly()
		{
			var janitor = CacheJanitor.Instance;
			var secret = new PrivateObject(janitor);
			var cache = new CheckAndSetCache<string, object>(Expires.Always<object>())
			{
				{ "key1", "value1" },
				{ "key2", "value2" },
				{ "key3", "value3" },
			};
			var cleanup = Cleanup.Expired<string, object>(cache);
			cleanup.Options = new CleanupOptions
			{
				Frequency = TimeSpan.FromMilliseconds(50),
			};

			Assert.AreEqual(3, cache.Count);
			janitor.Register(cache, cleanup);
			// cleanup starts
			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			Assert.AreEqual(0, cache.Count);
			janitor.Unregister(cache);
		}
	}
}
