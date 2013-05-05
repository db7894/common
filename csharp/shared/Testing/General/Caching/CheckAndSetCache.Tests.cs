using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// CheckAndSetCache.
	/// </summary>
	[TestClass]
	public class CheckAndSetCacheTests
	{
		[TestMethod]
		public void CheckAndSetCache_Factory_InitializesCache()
		{
			var strategy = Expires.Never<int>();
			var cache = CheckAndSetCache.Create<string, int>(strategy);

			Assert.IsNotNull(cache);
		}

		[TestMethod]
		public void CheckAndSetCache_AddAndRemove_Works()
		{
			using (var cache = new CheckAndSetCache<string, object>())
			{
				cache.Add("1", 1);
				cache.Add("2", (k) => (object)int.Parse(k));
				cache.Add("3", 3, (k) => (object)int.Parse(k));
				Assert.AreEqual(3, cache.Count);

				var result = new List<string> { "1", "2", "3" }.All(cache.Remove);
				Assert.IsTrue(result);
				Assert.AreEqual(0, cache.Count);
			}	
		}

		[TestMethod]
		public void CheckAndSetCache_GetExpired_Works()
		{
			var strategy = Expires.Always<object>();
			var cache = new CheckAndSetCache<string, object>(strategy);
			cache.Add("1", 1);
			Assert.AreEqual(null, cache.Get("1"));
			Assert.AreEqual(null, cache.Get("2"));
		}

		[TestMethod]
		public void CheckAndSetCache_Clean_Works()
		{
			var strategy = new GenericExpirationStrategy<object>(v => (int)v.Value == 2);
			using (var cache = new CheckAndSetCache<string, object>(strategy))
			{
				cache.Add("1", 1);
				cache.Add("2", 2);
				cache.Add("3", 3);
				Assert.AreEqual(3, cache.Count);
				cache.Clean();
				Assert.AreEqual(2, cache.Count);
			}
		}

		[TestMethod]
		public void CheckAndSetCache_Clear_Works()
		{
			var strategy = Expires.Always<object>();
			using (var cache = new CheckAndSetCache<string, object>(strategy))
			{
				cache.Add("1", 1);
				cache.Add("2", 2);
				cache.Add("3", 3);
				Assert.AreEqual(3, cache.Count);
				cache.Clear();
				Assert.AreEqual(0, cache.Count);
			}
		}

		[TestMethod]
		public void CheckAndSetCache_Enumeration_Works()
		{
			var strategy = Expires.Never<object>();
			var cache = new CheckAndSetCache<string, object>(strategy)
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
