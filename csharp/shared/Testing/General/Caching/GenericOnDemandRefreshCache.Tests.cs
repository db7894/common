using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// GenericOnDemandRefreshCache.
	/// </summary>
	[TestClass]
	public class GenericOnDemandRefreshCacheTests
	{
		[TestMethod]
		public void GenericOnDemandRefreshCache_Factory_InitializesCache()
		{
			var strategy = Expires.Never<string>();
			Func<string, string> missing = (key) => key;
			var cache = GenericOnDemandRefreshCache.Create(strategy, missing);

			Assert.IsNotNull(cache);
		}

		[TestMethod]
		public void GenericOnDemandRefreshCache_Defaults_Tests()
		{
			using (var cache = new GenericOnDemandRefreshCache<string, object>())
			{
				Assert.IsNull(cache.Get("non-existant"));
			}
		}

		[TestMethod]
		public void GenericOnDemandRefreshCache_AddAndRemove_Functions()
		{
			var strategy = Expires.Never<string>();
			Func<string, string> missing = (key) => key;
			var cache = GenericOnDemandRefreshCache.Create(strategy, missing);

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
		public void GenericOnDemandRefreshCache_Clear_Functions()
		{
			var strategy = Expires.Never<string>();
			Func<string, string> missing = (key) => key;
			var cache = GenericOnDemandRefreshCache.Create(strategy, missing);

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
		public void GenericOnDemandRefreshCache_Clean_Functions()
		{
			var strategy = new GenericExpirationStrategy<string>(v => v.Value == "value1");
			Func<string, string> missing = (key) => key;
			var cache = GenericOnDemandRefreshCache.Create(strategy, missing);

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
			var strategy = Expires.Always<int>();
			Func<string, int> factory = (key) => int.Parse(key);
			var cache = new GenericOnDemandRefreshCache<string, int>(strategy, factory);

			Assert.AreEqual(1, cache.Get("1"));
			Assert.AreEqual(2, cache.Get("2"));
			Assert.AreEqual(1, cache.Get("1"));
			Assert.AreEqual(2, cache.Get("2"));
			Assert.AreEqual(2, cache.Count);

			Assert.AreEqual(0, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(0, cache.Statistics.Evictions.Value);
			Assert.AreEqual(4, cache.Statistics.Requests.Value);
			Assert.AreEqual(2, cache.Statistics.Hits.Value);
			Assert.AreEqual(4, cache.Statistics.Updates.Value);
			Assert.AreEqual(2, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void GenericOnDemandRefreshCache_Enumeration_Works()
		{
			var strategy = Expires.Never<int>();
			var cache = new GenericOnDemandRefreshCache<string, int>(
				strategy, int.Parse)
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
