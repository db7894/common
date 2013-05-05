using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Cleaning.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// GenericCleanupStrategy.
	/// </summary>
	[TestClass]
	public class GenericCleanupStrategyTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_WithBadCache_Throws()
		{
			ICache<string, object> cache = null;
			GenericCleanupStrategy<string, object>.CleanupCallback predicate
				= stream => stream.Select(s => s.Key);
			var strategy = new GenericCleanupStrategy<string, object>(cache, predicate);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_WithBadPredicate_Throws()
		{
			ICache<string, object> cache = new CheckAndSetCache<string, object>(
				Expires.Never<object>());
			GenericCleanupStrategy<string, object>.CleanupCallback predicate = null;
			var strategy = new GenericCleanupStrategy<string, object>(cache, predicate);
		}

		[TestMethod]
		public void Create_WithGoodPredicate_Succeeds()
		{
			ICache<string, object> cache = new CheckAndSetCache<string, object>(
				Expires.Never<object>());
			GenericCleanupStrategy<string, object>.CleanupCallback predicate
				= stream => stream.Select(s => s.Key);
			var strategy = new GenericCleanupStrategy<string, object>(cache, predicate);

			Assert.IsNotNull(strategy);
		}

		[TestMethod]
		public void Create_WithDefaultOptions_Succeeds()
		{
			ICache<string, object> cache = new CheckAndSetCache<string, object>(
				Expires.Never<object>());
			GenericCleanupStrategy<string, object>.CleanupCallback predicate
				= stream => stream.Select(s => s.Key);
			var strategy = new GenericCleanupStrategy<string, object>(cache, predicate);

			Assert.AreEqual(CleanupOptions.Default, strategy.Options);
		}

		[TestMethod]
		public void IsExpired_WorksCorrectly_Test()
		{
			ICache<string, object> cache = new CheckAndSetCache<string, object>(
				Expires.Never<object>())
			{
				{ "key1", "value1" },
				{ "key2", "value2" },
				{ "key3", "value3" },
			};
			GenericCleanupStrategy<string, object>.CleanupCallback predicate
				= stream => stream.Select(s => s.Key);
			var strategy = new GenericCleanupStrategy<string, object>(cache, predicate);

			Assert.AreEqual(3, cache.Count);
			strategy.PerformCleanup();
			Assert.AreEqual(0, cache.Count);
		}
	}
}
