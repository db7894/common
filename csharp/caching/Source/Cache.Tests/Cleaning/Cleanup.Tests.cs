using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace SharedAssemblies.General.Caching.Cleaning.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// Cleanup strategy factory.
	/// </summary>
	[TestClass]
	public class CleanupTests
	{
		#region Test Initialization

		private ICache<string, object> Cache;
		private bool Expired;

		[TestInitialize]
		public void TestSetupMethod()
		{
			Expired = false;
			Cache = new CheckAndSetCache<string, object>(Expires.When<object>(() => Expired))
			{
			    { "key1", "value1" },
			    { "key2", "value2" },
			    { "key3", "value3" },
			};
		}

		#endregion

		[TestMethod]
		public void Cleanup_Nothing_Test()
		{
			var strategy = Cleanup.Nothing(Cache);
			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key1", "key2", "key3" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_LeastPopular_Test()
		{
			var strategy = Cleanup.LeastPopular(Cache, 2);

			for (int i = 1; i < 4; ++i)
			{
				for (int j = i; j > 0; --j)
				{
					var unused = Cache.Get("key" + i);
				}
			}

			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key3" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_AllButMostPopular_Test()
		{
			var strategy = Cleanup.AllButMostPopular(Cache, 1);

			for (int i = 1; i < 4; ++i)
			{
				for (int j = i; j > 0; --j)
				{
					var unused = Cache.Get("key" + i);
				}
			}

			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key3" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_LeastRecentlyUsed_Test()
		{
			var strategy = Cleanup.LeastRecentlyUsed(Cache, 2);

			// update the touch value
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			var unused = Cache.Get("key2");

			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key2" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_AllButMostRecentlyUsed_Test()
		{
			var strategy = Cleanup.AllButMostRecentlyUsed(Cache, 1);

			// update the touch value
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			var unused = Cache.Get("key1");
			
			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key1" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_BoundedAtFifo_Test()
		{
			var strategy = Cleanup.BoundedAtFifo(Cache, 2);

			// add some later elements
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			Cache.Add("key4", "value4");
			Cache.Add("key5", "value5");

			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key4", "key5" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_BoundedAtLifo_Test()
		{
			var strategy = Cleanup.BoundedAtLifo(Cache, 3);

			// add some later elements
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			Cache.Add("key4", "value4");
			Cache.Add("key5", "value5");

			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key1", "key2", "key3" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_Expired_Test()
		{
			var strategy = Cleanup.Expired(Cache);

			// all elements are expired by strategy
			Expired = true;

			var result = strategy.PerformCleanup();
			var expected = new List<string> { };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_OlderThan_Test()
		{
			var strategy = Cleanup.OlderThan(Cache, TimeSpan.Zero);

			// all elements are at least some time old
			Thread.Sleep(TimeSpan.FromMilliseconds(50));

			var result = strategy.PerformCleanup();
			var expected = new List<string> { };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Cleanup_YoungerThan_Test()
		{
			var strategy = Cleanup.YoungerThan(Cache, TimeSpan.FromDays(1));

			// no element is this old

			var result = strategy.PerformCleanup();
			var expected = new List<string> { };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}
		
		[TestMethod]
		public void Cleanup_NotUsedIn_Test()
		{
			var delay = Stopwatch.GetTimestamp() + TimeSpan.FromSeconds(1).Ticks;
			var strategy = Cleanup.NotUsedIn(Cache, TimeSpan.FromMilliseconds(150));

			do
			{   // spinwait so we don't context switch
				var unused = Cache.Get("key1"); // update touch time
			} while (Stopwatch.GetTimestamp() < delay);

			var result = strategy.PerformCleanup();
			var expected = new List<string> { "key1" };
			var actual = Cache.Select(item => item.Key).ToList();

			CollectionAssert.AreEquivalent(expected, actual);
			Assert.IsTrue(result);
		}
	}
}
