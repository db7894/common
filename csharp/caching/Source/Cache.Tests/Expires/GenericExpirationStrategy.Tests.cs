using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// GenericExpirationStrategy.
	/// </summary>
	[TestClass]
	public class GenericExpirationStrategyTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_WithBadPredicate_Throws()
		{
			Predicate<CachedValue<string>> predicate = null;
			var strategy = new GenericExpirationStrategy<string>(predicate);
		}

		[TestMethod]
		public void Create_WithGoodPredicate_Succeeds()
		{
			Predicate<CachedValue<string>> predicate = (item) => true;
			var strategy = new GenericExpirationStrategy<string>(predicate);

			Assert.IsNotNull(strategy);
		}

		[TestMethod]
		public void IsExpired_WorksCorrectly_Test()
		{
			Predicate<CachedValue<string>> predicate = (item) => (item.Value == "expired");
			var strategy = new GenericExpirationStrategy<string>(predicate);

			Assert.IsFalse(strategy.IsExpired(new CachedValue<string>("good")));
			Assert.IsTrue(strategy.IsExpired(new CachedValue<string>("expired")));
		}
	}
}
