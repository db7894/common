using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// CacheStatistics.
	/// </summary>
	[TestClass]
	public class CacheStatisticsTests
	{
		[TestMethod]
		public void Statistics_Initializes_Correctly()
		{
			var statistics = new CacheStatistics();
			Assert.IsNotNull(statistics);
		}

		[TestMethod]
		public void Statistics_Increments_Correctly()
		{
			var statistics = new CacheStatistics();

			Assert.AreEqual(1, statistics.Cleanings.Increment());
			Assert.AreEqual(1, statistics.Evictions.Increment());
			Assert.AreEqual(1, statistics.Hits.Increment());
			Assert.AreEqual(1, statistics.Misses.Increment());
			Assert.AreEqual(1, statistics.Requests.Increment());
			Assert.AreEqual(1, statistics.Updates.Increment());

			Assert.AreEqual(2, statistics.Cleanings.Increment());
			Assert.AreEqual(2, statistics.Evictions.Increment());
			Assert.AreEqual(2, statistics.Hits.Increment());
			Assert.AreEqual(2, statistics.Misses.Increment());
			Assert.AreEqual(2, statistics.Requests.Increment());
			Assert.AreEqual(2, statistics.Updates.Increment());
		}

		[TestMethod]
		public void Statistics_GetsValue_Correctly()
		{
			var statistics = new CacheStatistics();

			Assert.AreEqual(0, statistics.Cleanings.Value);
			Assert.AreEqual(0, statistics.Evictions.Value);
			Assert.AreEqual(0, statistics.Hits.Value);
			Assert.AreEqual(0, statistics.Misses.Value);
			Assert.AreEqual(0, statistics.Requests.Value);
			Assert.AreEqual(0, statistics.Updates.Value);

			Assert.AreEqual(1, statistics.Cleanings.Increment());
			Assert.AreEqual(1, statistics.Evictions.Increment());
			Assert.AreEqual(1, statistics.Hits.Increment());
			Assert.AreEqual(1, statistics.Misses.Increment());
			Assert.AreEqual(1, statistics.Requests.Increment());
			Assert.AreEqual(1, statistics.Updates.Increment());

			Assert.AreEqual(1, statistics.Cleanings.Value);
			Assert.AreEqual(1, statistics.Evictions.Value);
			Assert.AreEqual(1, statistics.Hits.Value);
			Assert.AreEqual(1, statistics.Misses.Value);
			Assert.AreEqual(1, statistics.Requests.Value);
			Assert.AreEqual(1, statistics.Updates.Value);
		}

		[TestMethod]
		public void Statistics_GetRates_Correctly()
		{
			var statistics = new CacheStatistics();

			// when we haven't had any requests yet we always get 1.0
			Assert.AreEqual(1.0, statistics.MissRate());
			Assert.AreEqual(1.0, statistics.HitRate());

			for (int i = 0; i < 10; ++i)
			{
				statistics.Hits.Increment();
				statistics.Requests.Increment();
			}
			Assert.AreEqual(0.0, statistics.MissRate());
			Assert.AreEqual(1.0, statistics.HitRate());

			for (int i = 0; i < 10; ++i)
			{
				statistics.Misses.Increment();
				statistics.Requests.Increment();
			}

			Assert.AreEqual(0.5, statistics.MissRate());
			Assert.AreEqual(0.5, statistics.HitRate());
		}
	}
}
