using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// GenericBackgroundRefreshCache.
	/// </summary>
	[TestClass]
	public class GenericBackgroundRefreshCacheTests
	{
		[TestMethod]
		public void GenericBackgroundRefreshCache_Factory_InitializesCache()
		{
			var timespan = TimeSpan.FromDays(1);
			var strategy = Expires.Never<string>();
			Func<string, string> factory = (key) => key;
			using (var cache = GenericBackgroundRefreshCache.Create(timespan, strategy, factory))
			{
				Assert.IsNotNull(cache);
			}
		}

		[TestMethod]
		public void GenericOnDemandRefreshCache_Defaults_Tests()
		{
			var timespan = TimeSpan.FromDays(1);
			using (var cache = new GenericBackgroundRefreshCache<string, object>(timespan))
			{
				Assert.IsNull(cache.Get("non-existant"));
			}
		}

		[TestMethod]
		public void GenericBackgroundRefreshCache_AddAndRemove_Functions()
		{
			var timespan = TimeSpan.FromDays(1);
			var strategy = Expires.Never<string>();
			Func<string, string> factory = (key) => key;
			var cache = GenericBackgroundRefreshCache.Create(timespan, strategy, factory);

			cache.Add("key1", "value1");
			cache.Add("key2", key => key + "value2");
			cache.Add("key3", "value3", key => key + "value3");
			Assert.AreEqual(3, cache.Count);

			foreach (var item in cache)
			{
				Assert.IsTrue(cache.Remove(item.Key));
				Assert.AreEqual(item.Key, cache.Get(item.Key));
			}
			Assert.AreEqual(3, cache.Count);

			Assert.AreEqual(1, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(3, cache.Statistics.Evictions.Value);
			Assert.AreEqual(3, cache.Statistics.Requests.Value);
			Assert.AreEqual(0, cache.Statistics.Hits.Value);
			Assert.AreEqual(3, cache.Statistics.Updates.Value);
			Assert.AreEqual(3, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void GenericBackgroundRefreshCache_Clear_Functions()
		{
			var timespan = TimeSpan.FromDays(1);
			var strategy = Expires.Never<string>();
			Func<string, string> factory = (key) => key;
			var cache = GenericBackgroundRefreshCache.Create(timespan, strategy, factory);

			cache.Add("key1", "value1");
			cache.Add("key2", key => key + "value2");
			cache.Add("key3", "value3", key => key + "value3");

			Assert.AreEqual(3, cache.Count);
			cache.Clear();
			Assert.AreEqual(0, cache.Count);

			Assert.AreEqual(1, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(0, cache.Statistics.Evictions.Value);
			Assert.AreEqual(0, cache.Statistics.Requests.Value);
			Assert.AreEqual(0, cache.Statistics.Hits.Value);
			Assert.AreEqual(3, cache.Statistics.Updates.Value);
			Assert.AreEqual(0, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void GenericBackgroundRefreshCache_Clean_Functions()
		{
			var timespan = TimeSpan.FromDays(1);
			var strategy = new GenericExpirationStrategy<string>(v => v.Value == "value1");
			Func<string, string> factory = (key) => key;
			var cache = GenericBackgroundRefreshCache.Create(timespan, strategy, factory);

			cache.Add("key1", "value1");
			cache.Add("key2", key => key + "value2");
			cache.Add("key3", "value3", key => key + "value3");

			Assert.AreEqual(3, cache.Count);
			cache.Clean();
			Assert.AreEqual(2, cache.Count);

			Assert.AreEqual(1, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(0, cache.Statistics.Evictions.Value);
			Assert.AreEqual(0, cache.Statistics.Requests.Value);
			Assert.AreEqual(0, cache.Statistics.Hits.Value);
			Assert.AreEqual(3, cache.Statistics.Updates.Value);
			Assert.AreEqual(0, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void RefreshAction_Populates_MissingCacheValues()
		{
			var timespan = TimeSpan.FromMilliseconds(50);
			var strategy = Expires.Always<int>();
			Func<string, int> factory = (key) => int.Parse(key);
			var cache = new GenericBackgroundRefreshCache<string, int>(timespan, strategy, factory);

			cache.Add("1", 2);
			cache.Add("2", 3);
			Assert.AreEqual(2, cache.Get("1"));
			Assert.AreEqual(3, cache.Get("2"));

			System.Threading.Thread.Sleep(100);
			Assert.AreEqual(1, cache.Get("1"));
			Assert.AreEqual(2, cache.Get("2"));
			Assert.AreEqual(2, cache.Count);

			Assert.AreEqual(0, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(0, cache.Statistics.Evictions.Value);
			Assert.AreEqual(4, cache.Statistics.Requests.Value);
			Assert.AreEqual(4, cache.Statistics.Hits.Value);
			Assert.AreEqual(4, cache.Statistics.Updates.Value);
			Assert.AreEqual(0, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void GenericBackgroundRefreshCache_Enumeration_Works()
		{
			var strategy = Expires.Never<object>();
			var cache = new GenericBackgroundRefreshCache<string, object>(
				TimeSpan.FromDays(1), strategy, key => (object)int.Parse(key))
			{
				{ "1", 1 },
				{ "2", 2 },
				{ "3", 3 },
			};

			foreach (var item in cache)
			{
				Assert.AreEqual(item.Value.Value, (int)cache[item.Key]);
			}

			// just to get 100%
			var enumerable = ((System.Collections.IEnumerable)cache).GetEnumerator();
		}
	}
}
