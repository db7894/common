using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.UnitTests
{
    /// <summary>
    /// Exercises the Cache utility.
    /// </summary>
    [TestClass]
    public class MultiThreadedCacheTest
    {
        /// <summary>
        /// Tests retrieving an item from the cache that should be there.
        /// </summary>
        [TestMethod]
        public void TestRetrieveFromCache()
        {
            // Never expire cache
            var cache = new Cache<string, CachableObject>(x => false);
            cache.Add("one", new CachableObject(1));

            CachableObject one = cache.GetValue("one");

            Assert.IsNotNull(one);
        }

        /// <summary>
        /// Tests retrieving an item from the cache that is not in the cache. The expected result is 
        /// that it returns null.
        /// </summary>
        [TestMethod]
        public void TestRetrieveFromCacheReturnsNullIfNotFound()
        {
            // Never expire cache
            var cache = new Cache<string, CachableObject>(x => false);
            cache.Add("one", new CachableObject(1));

            CachableObject two = cache.GetValue("two");

            Assert.IsNull(two);
        }

        /// <summary>
        /// Tests that when using the GetValidValue method from cache, that an expired item does
        /// not get returned.
        /// </summary>
        [TestMethod]
        public void TestRetrieveValidValueFromCacheAfterExpiredReturnsNull()
        {
            // Set timespan to expire after 3 seconds
            TimeSpan ts = new TimeSpan(0, 0, 0, 3);

            var cache = new Cache<string, CachableObject>(x => (DateTime.Now - x.Timestamp > ts));
            cache.Add("one", new CachableObject(1));

            var before = cache.GetValidValue("one");

            Thread.Sleep(5000);

            var after = cache.GetValidValue("one");

            Assert.IsNotNull(before);
            Assert.IsNull(after);
        }

        /// <summary>
        /// Tests that the default predicate is used if we do not define one, and that no exceptions are thrown.
        /// </summary>
        [TestMethod]
        public void TestNoExceptionIfPredicateNotSet()
        {
            ArgumentException thrownException = null;

            var cache = new Cache<string, CachableObject>();

            try
            {
                cache.Get("ABC");
            }
            catch (ArgumentException ex)
            {
                thrownException = ex;
            }

            Assert.IsNull(thrownException);
        }

        /// <summary>
        /// Tests that if a custom predicate is set via the constructor, that no exceptions are thrown.
        /// </summary>
        [TestMethod]
        public void TestNoExceptionIfPredicateSetViaConstructor()
        {
            ArgumentException thrownException = null;

            var cache = new Cache<string, CachableObject>(x => true);

            try
            {
                cache.Get("ABC");
            }
            catch (ArgumentException ex)
            {
                thrownException = ex;
            }

            Assert.IsNull(thrownException);
        }

        /// <summary>
        /// Tests that we can retrieve expired items when using the Get method.
        /// </summary>
        [TestMethod]
        public void TestCanRetrieveExpiredCachedItems()
        {
            // Set timespan to expire after 3 seconds
            TimeSpan ts = new TimeSpan(0, 0, 0, 3);

            var cache = new Cache<string, CachableObject>(x => (DateTime.Now - x.Timestamp > ts));
            cache.Add("one", new CachableObject(1));

            var before = cache.Get("one");
            Assert.IsFalse(before.IsExpired);

            Thread.Sleep(5000);

            var after = cache.Get("one");
            Assert.IsNotNull(after);
            Assert.IsTrue(after.IsExpired);
        }

        /// <summary>
        /// Tests to see if manual expiration removes the item from valid pull
        /// </summary>
        [TestMethod]
        public void Expire_ExpiresItem_WhenPassedTrue()
        {
            var cache = new Cache<string, CachableObject>(x => false);
            cache.Add("one", new CachableObject(1));

            var before = cache.Get("one");
            Assert.IsFalse(before.IsExpired);

            cache.Expire("one");

            var after = cache.Get("one");
            Assert.IsTrue(after.IsExpired);

            Assert.IsNull(cache.GetValidValue("one"));
        }

        /// <summary>
        /// Tests that we can retrieve expired values when using the GetValue method.
        /// </summary>
        [TestMethod]
        public void TestCanRetrieveExpiredCachedValues()
        {
            // Set timespan to expire after 3 seconds
            TimeSpan ts = new TimeSpan(0, 0, 0, 3);

            var cache = new Cache<string, CachableObject>(x => (DateTime.Now - x.Timestamp > ts));
            cache.Add("one", new CachableObject(1));

            Thread.Sleep(5000);
            
            var after = cache.GetValue("one");
            Assert.IsNotNull(after);
        }

        /// <summary>
        /// Tests that we can overwrite a cached object, and the cached date gets updated.
        /// </summary>
        [TestMethod]
        public void TestOverwriteCachedObject()
        {
            // Never expire cache
            var cache = new Cache<string, CachableObject>(x => false);
            cache.Add("one", new CachableObject(1));

            var before = cache.Get("one");

            Thread.Sleep(1000);
            cache.Add("one", new CachableObject(2));

            var after = cache.Get("one");

            Assert.IsNotNull(before);
            Assert.IsNotNull(after);
            Assert.AreEqual(1, before.Value.Number);
            Assert.AreEqual(2, after.Value.Number);
            Assert.AreNotEqual(before.Timestamp, after.Timestamp);
            Assert.IsTrue(before.Timestamp < after.Timestamp);
        }


        /// <summary>
        /// Dummy object to allow caching
        /// </summary>
        internal class CachableObject
        {
			/// <summary>
			/// Property to get/set the number
			/// </summary>
			public int Number
			{
				get
				{
					return int.Parse(Value);
				}

				set
				{
					Value = value.ToString();
				}
			}

			/// <summary>
			/// The value to get/set
			/// </summary>
			public string Value { get; private set; }
			
			
			/// <summary>
            /// Constructs using the given number
            /// </summary>
            /// <param name="number">Number to wrap</param>
            public CachableObject(int number)
            {
                Number = number;
            }
        }
    }
}