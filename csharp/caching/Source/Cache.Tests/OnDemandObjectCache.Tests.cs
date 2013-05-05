using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// OnDemandObjectCache.
	/// </summary>
	[TestClass]
	public class OnDemandObjectCacheTests
	{
		[TestMethod]
		public void OnDemandObjectCache_AlwaysExpires_Test()
		{
			var count = -1; // we initialize (increment by one) on creation
			var strategy = Expires.Always<int>();
			using (var cache = new OnDemandObjectCache<int>(() => ++count, strategy))
			{
				Assert.AreEqual(1, cache.Value);
				Assert.AreEqual(2, cache.Value);
				cache.Refresh();
				Assert.AreEqual(4, cache.Value);

				Assert.AreEqual(0, cache.Statistics.Cleanings.Value);
				Assert.AreEqual(0, cache.Statistics.Evictions.Value);
				Assert.AreEqual(3, cache.Statistics.Requests.Value);
				Assert.AreEqual(0, cache.Statistics.Hits.Value);
				Assert.AreEqual(5, cache.Statistics.Updates.Value);
				Assert.AreEqual(3, cache.Statistics.Misses.Value);
			}
		}

		[TestMethod]
		public void OnDemandObjectCache_Statistics_Test()
		{
			var count = -1; // we initialize (increment by one) on creation
			var strategy = Expires.Never<int>();
			using (var cache = new OnDemandObjectCache<int>(() => ++count, strategy))
			{
				Assert.AreEqual(0, cache.Value);
				Assert.AreEqual(0, cache.Value);
				cache.Refresh();
				Assert.AreEqual(1, cache.Value);

				Assert.AreEqual(0, cache.Statistics.Cleanings.Value);
				Assert.AreEqual(0, cache.Statistics.Evictions.Value);
				Assert.AreEqual(3, cache.Statistics.Requests.Value);
				Assert.AreEqual(3, cache.Statistics.Hits.Value);
				Assert.AreEqual(2, cache.Statistics.Updates.Value);
				Assert.AreEqual(0, cache.Statistics.Misses.Value);
			}			
		}
	}
}
