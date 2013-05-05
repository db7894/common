using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.UnitTests
{
    /// <summary>
    /// Exercises the Cache utility.
    /// </summary>
    [TestClass()]
    public class DifferentDayExpirationStrategy
    {
        /// <summary>
        /// Tests that an item cached on the same day is not expired
        /// </summary>
        [TestMethod()]
        public void TestSameDateIsNotExpired()
        {
            DateTime timestamp = DateTime.Now;

            CachedItem<string> cachedItem = new CachedItem<string>(
                "Value",
                new DifferentDayExpirationStrategy<string>(),
                timestamp);

            Assert.IsFalse(cachedItem.IsExpired);
        }

        /// <summary>
        /// Tests that an item cached on the same day, but a different time is not expired
        /// </summary>
        [TestMethod()]
        public void TestDifferentTimeIsNotExpired()
        {
            DateTime timestamp = DateTime.Now - new TimeSpan(0, 0, 1, 1);

            CachedItem<string> cachedItem = new CachedItem<string>(
                "Value",
                new DifferentDayExpirationStrategy<string>(),
                timestamp);

            Assert.IsFalse(cachedItem.IsExpired);
        }

        /// <summary>
        /// Tests that an item cached on the same day/month, but a different year is expired
        /// </summary>
        [TestMethod()]
        public void TestDifferentYearIsExpired()
        {
            DateTime timestamp = DateTime.Now.AddYears(-1);

            CachedItem<string> cachedItem = new CachedItem<string>(
                "Value",
                new DifferentDayExpirationStrategy<string>(),
                timestamp);

            Assert.IsTrue(cachedItem.IsExpired);
        }

        /// <summary>
        /// Tests that an item cached on the same day/year, but a different month is expired
        /// </summary>
        [TestMethod()]
        public void TestDifferentMonthIsExpired()
        {
            DateTime timestamp = DateTime.Now.AddMonths(-1);

            CachedItem<string> cachedItem = new CachedItem<string>(
                "Value",
                new DifferentDayExpirationStrategy<string>(),
                timestamp);

            Assert.IsTrue(cachedItem.IsExpired);
        }

        /// <summary>
        /// Tests that an item cached on the same month/year, but a different day is expired
        /// </summary>
        [TestMethod()]
        public void TestDifferentDayIsExpired()
        {
            DateTime timestamp = DateTime.Now.AddDays(-1);

            CachedItem<string> cachedItem = new CachedItem<string>(
                "Value",
                new DifferentDayExpirationStrategy<string>(),
                timestamp);

            Assert.IsTrue(cachedItem.IsExpired);
        }
    }
}