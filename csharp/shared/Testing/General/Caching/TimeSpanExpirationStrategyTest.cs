using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.UnitTests
{
    /// <summary>
    /// Exercises the TimeSpanExpirationStrategy class
    /// </summary>
    [TestClass()]
    public class TimeSpanExpirationStrategyTest
    {
        /// <summary>
        /// Tests that a cached item within the specified timespan is not expired
        /// </summary>
        [TestMethod()]
        public void TestWithinTimeSpanNotExpired()
        {
            DateTime timestamp = DateTime.Now.AddMinutes(-30);

            CachedItem<string> cachedItem = new CachedItem<string>(
                "Value",
                new TimeSpanExpirationStrategy<string>(new TimeSpan(1, 0, 0)),
                timestamp);

            Assert.IsFalse(cachedItem.IsExpired);
        }

        /// <summary>
        /// Tests that a cached item outside the specified timespan is expired
        /// </summary>
        [TestMethod()]
        public void TestOutsideTimeSpanIsExpired()
        {
            DateTime timestamp = DateTime.Now.AddMinutes(-30);

            CachedItem<string> cachedItem = new CachedItem<string>(
                "Value",
                new TimeSpanExpirationStrategy<string>(new TimeSpan(0, 15, 0)),
                timestamp);

            Assert.IsTrue(cachedItem.IsExpired);
        }
    }
}