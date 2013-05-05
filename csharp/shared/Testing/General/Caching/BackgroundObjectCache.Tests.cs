using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Caching;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// BackgroundObjectCache.
	/// </summary>
	[TestClass]
	public class BackgroundObjectCacheTests
	{
		[TestMethod]
		public void BackgroundObjectCache_AlwaysExpires_Test()
		{
			var count = 0;
			using (var cache = new BackgroundObjectCache<int>(() => ++count, TimeSpan.FromMilliseconds(100)))
			{

				Assert.AreEqual(1, cache.Value);
				Assert.AreEqual(1, cache.Value);
				cache.Refresh();
				Assert.AreEqual(2, cache.Value);
				System.Threading.Thread.Sleep(150);
				Assert.AreEqual(3, cache.Value);

				Assert.AreEqual(0, cache.Statistics.Cleanings.Value);
				Assert.AreEqual(0, cache.Statistics.Evictions.Value);
				Assert.AreEqual(4, cache.Statistics.Requests.Value);
				Assert.AreEqual(4, cache.Statistics.Hits.Value);
				Assert.AreEqual(3, cache.Statistics.Updates.Value);
				Assert.AreEqual(0, cache.Statistics.Misses.Value);
			}
		}
	}
}
