using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// SpecificOnDemandRefreshCache.
	/// </summary>
	[TestClass]
	public class SpecificOnDemandRefreshCacheTests
	{
		[TestMethod]
		public void SpecificOnDemandRefreshCache_Factory_InitializesCache()
		{
			var strategy = Expires.Never<int>();
			Func<string, int> missing = (key) => int.Parse(key);
			using (var cache = SpecificOnDemandRefreshCache.Create(strategy, missing))
			{
				Assert.IsNotNull(cache);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void SpecificOnDemandRefreshCache_Defaults_Tests()
		{
			using (var cache = new SpecificOnDemandRefreshCache<string, object>())
			{
				cache.Get("non-existant");
			}
		}

		[TestMethod]
		public void SpecificOnDemandRefreshCache_AddAndRemove_Functions()
		{
			var strategy = Expires.Never<string>();
			Func<string, string> missing = (key) => key;
			var cache = SpecificOnDemandRefreshCache.Create(strategy, missing);

			cache.Add("key1", "value1");
			cache.Add("key2", key => key + "value2");
			cache.Add("key3", (Func<string, string>)null);
			cache.Add("key4", "value4", key => key + "value4");
			Assert.AreEqual(4, cache.Count);

			foreach (var item in cache)
			{
				Assert.IsTrue(cache.Remove(item.Key));
				Assert.AreEqual(item.Key, cache.Get(item.Key));
			}
			Assert.AreEqual(4, cache.Count);

			Assert.AreEqual(1, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(4, cache.Statistics.Evictions.Value);
			Assert.AreEqual(4, cache.Statistics.Requests.Value);
			Assert.AreEqual(0, cache.Statistics.Hits.Value);
			Assert.AreEqual(4, cache.Statistics.Updates.Value);
			Assert.AreEqual(4, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void SpecificOnDemandRefreshCache_GetExpired_Functions()
		{
			var strategy = Expires.Always<string>();
			Func<string, string> missing = (key) => key;
			var cache = SpecificOnDemandRefreshCache.Create(strategy, missing);

			cache.Add("key1", "value1");
			cache.Add("key2", "value2");
			Assert.AreEqual(2, cache.Count);
			Assert.AreEqual("key1", cache.Get("key1"));
			Assert.AreEqual("key2", cache.Get("key2"));

			Assert.AreEqual(0, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(0, cache.Statistics.Evictions.Value);
			Assert.AreEqual(2, cache.Statistics.Requests.Value);
			Assert.AreEqual(2, cache.Statistics.Hits.Value);
			Assert.AreEqual(4, cache.Statistics.Updates.Value);
			Assert.AreEqual(0, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void SpecificOnDemandRefreshCache_Clear_Functions()
		{
			var strategy = Expires.Never<string>();
			Func<string, string> missing = (key) => key;
			var cache = SpecificOnDemandRefreshCache.Create(strategy, missing);
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
		public void SpecificOnDemandRefreshCache_Clean_Functions()
		{
			var strategy = new GenericExpirationStrategy<string>(v => v.Value == "value1");
			Func<string, string> missing = (key) => key;
			var cache = SpecificOnDemandRefreshCache.Create(strategy, missing);
			cache.Add("key1", "value1");
			cache.Add("key2", key => key + "value2"); // defaults to expired until next pull
			cache.Add("key3", "value3", key => key + "value3");

			Assert.AreEqual(3, cache.Count);
			cache.Clean();
			Assert.AreEqual(1, cache.Count);

			Assert.AreEqual(1, cache.Statistics.Cleanings.Value);
			Assert.AreEqual(0, cache.Statistics.Evictions.Value);
			Assert.AreEqual(0, cache.Statistics.Requests.Value);
			Assert.AreEqual(0, cache.Statistics.Hits.Value);
			Assert.AreEqual(3, cache.Statistics.Updates.Value);
			Assert.AreEqual(0, cache.Statistics.Misses.Value);
		}

		[TestMethod]
		public void SpecificOnDemandRefreshCache_Enumeration_Works()
		{
			var strategy = Expires.Never<int>();
			var cache = new SpecificOnDemandRefreshCache<string, int>(strategy, int.Parse)
			{
				{ "1", 1 },
				{ "2", 2 },
				{ "3", 3 },
			};

			foreach (var item in cache)
			{
				Assert.AreEqual(item.Value.Value, cache[item.Key]);
			}

			// just to get 100%
			var enumerable = ((System.Collections.IEnumerable)cache).GetEnumerator();
		}
	}
}
