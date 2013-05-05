using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Threading.UnitTests
{
    /// <summary>
    /// This is a test class for BucketTest and is intended
    /// to contain all BucketTest Unit Tests
    /// </summary>
    [TestClass]
	[Obsolete("These tests are obsolete and will be removed once Bucket<T> is removed.")]
    public class BucketTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void IsEmpty_ReturnsFalse_OnAdd()
        {
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");

            Assert.IsFalse(target.IsEmpty);
        }


        /// <summary>
        /// A test for Timeout
        /// </summary>
        [TestMethod]
        public void Timeout_ReturnsValue_OnCall()
        {
            Bucket<string> target = new Bucket<string>(10);
            target.GetTimeoutInMs = 10;

            Assert.AreEqual(10, target.GetTimeoutInMs);
        }


        /// <summary>
        /// A test for Timeout
        /// </summary>
        [TestMethod]
        public void Timeout_ReturnsNegativeOne_OnBadCall()
        {
            Bucket<string> target = new Bucket<string>(10);
            target.GetTimeoutInMs = -9999;

            Assert.AreEqual(Bucket<string>.InfiniteWait, target.GetTimeoutInMs);
        }


        /// <summary>
        /// A test for Timeout
        /// </summary>
        [TestMethod]
        public void Timeout_ReturnsInfiniteWait_OnInfiniteWaitCall()
        {
            Bucket<string> target = new Bucket<string>(10);
            target.GetTimeoutInMs = Bucket<string>.InfiniteWait;

            Assert.AreEqual(-1, target.GetTimeoutInMs);
        }


        /// <summary>
        /// A test for Timeout
        /// </summary>
        [TestMethod]
        public void Timeout_ReturnsZero_OnCallWithZero()
        {
            Bucket<string> target = new Bucket<string>(10);
            target.GetTimeoutInMs = 0;

            Assert.AreEqual(0, target.GetTimeoutInMs);
        }


        /// <summary>
        /// A test for Timeout
        /// </summary>
        [TestMethod]
        public void InfinteWait_ReturnsNegativeOne_Always()
        {
            Assert.AreEqual(-1, Bucket<string>.InfiniteWait);
        }


        /// <summary>
        /// A test for Timeout
        /// </summary>
        [TestMethod]
        public void Timeout_DefaultsToOneSecond_OnConstruction()
        {
            Bucket<string> target = new Bucket<string>(10);

            Assert.AreEqual(1000, target.GetTimeoutInMs);
        }


        /// <summary>
        /// A test for Timeout
        /// </summary>
        [TestMethod]
        public void Constructor_Defaults_OnDefaultConstruction()
        {
            Bucket<string> target = new Bucket<string>();

            Assert.AreEqual(Bucket<string>.DefaultMaxItemsPerGet, target.MaxItemsPerGet);
        }


        /// <summary>
        /// MaxItemsPerGet defautls on construction
        /// </summary>
        [TestMethod]
        public void MaxItemsPerGet_DefaultsToOneHundred_OnConstruction()
        {
            Bucket<string> target = new Bucket<string>(10);

            Assert.AreEqual(100, target.MaxItemsPerGet);
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void IsEmpty_ReturnsFalse_OnGetAfterAdd()
        {
            string result;
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");
            target.Get(out result);

            Assert.IsTrue(target.IsEmpty);
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Get_ReturnsFirstItem_OnGetAfterAdds()
        {
        	string result1;
        	string result2;
        	string result3;
			string result4;
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");
            target.Add("Two");
            target.Add("Three");
            target.Add("Four");
            target.Get(out result1);
            target.Get(out result2);
            target.Get(out result3);
            target.Get(out result4);

            Assert.AreEqual("One", result1);
            Assert.AreEqual("Two", result2);
            Assert.AreEqual("Three", result3);
            Assert.AreEqual("Four", result4);
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Get_ReturnsFalse_OnGetWhenEmpty()
        {
            string result;
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");
            Assert.IsTrue(target.Get(out result));
            Assert.IsFalse(target.Get(out result));
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Get_ReturnsAll_OnGetAllAfterAdds()
        {
            List<string> results;
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");
            target.Add("Two");
            target.Add("Three");
            target.Add("Four");

            Assert.IsTrue(target.Get(out results));

            Assert.AreEqual(4, results.Count);
            Assert.AreEqual("One", results[0]);
            Assert.AreEqual("Two", results[1]);
            Assert.AreEqual("Three", results[2]);
            Assert.AreEqual("Four", results[3]);

            Assert.IsFalse(target.Get(out results));
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Get_ReturnsMax_OnGetIfMoreThanMax()
        {
            List<string> results;
            Bucket<string> target = new Bucket<string>(10, 2);

            target.Add("One");
            target.Add("Two");
            target.Add("Three");
            target.Add("Four");

            Assert.IsTrue(target.Get(out results));

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("One", results[0]);
            Assert.AreEqual("Two", results[1]);

            Assert.IsTrue(target.Get(out results));

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("Three", results[0]);
            Assert.AreEqual("Four", results[1]);

            Assert.IsFalse(target.Get(out results));
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Get_ReturnsFalse_OnGetAfterGetAll()
        {
            List<string> results;
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");
            target.Add("Two");
            target.Add("Three");
            target.Add("Four");
            Assert.IsTrue(target.Get(out results));
            Assert.IsFalse(target.Get(out results));
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Clear_RemovesAll_OnCall()
        {
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");
            target.Add("Two");
            target.Add("Three");
            target.Add("Four");
            target.Clear();
            Assert.IsTrue(target.IsEmpty);
        }


        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Wait_DoesNotWait_OnCallWhenNotEmpty()
        {
            Bucket<string> target = new Bucket<string>(10);

            target.Add("One");
            target.Add("Two");
            target.Add("Three");
            target.Add("Four");

            string item;
            DateTime start = DateTime.Now;
            target.Get(out item);
            DateTime stop = DateTime.Now;

            double span = (stop - start).TotalMilliseconds;

            Assert.IsTrue(span < 900.0);
        }



        /// <summary>
        /// A test for IsEmpty
        /// </summary>
        [TestMethod]
        public void Wait_Waits_OnCallWhenEmpty()
        {
            Bucket<string> target = new Bucket<string>(10);

            string item;
            DateTime start = DateTime.Now;
            target.Get(out item);
            DateTime stop = DateTime.Now;

            double span = (stop - start).TotalMilliseconds;

            Assert.IsTrue(span >= 900.0);
        }
    }
}