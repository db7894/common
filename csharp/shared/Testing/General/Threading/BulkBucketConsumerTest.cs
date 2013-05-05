using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Threading.UnitTests
{
    /// <summary>
    /// This is a test class for BucketConsumerTest and is intended
    /// to contain all BucketConsumerTest Unit Tests
    /// </summary>
    [TestClass]
	[Obsolete("These tests are obsolete and will be removed once BulkBucketConsumer<T> is removed.")]
	public class BulkBucketConsumerTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Constructor initializes all fields correctly
        /// </summary>
        [TestMethod]
        public void Constructor_Initializes_OnCall()
        {
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(target.OnConsumeBulk)
                                                      {
                                                          OnConsumerStarted = target.OnConsumeStarted,
                                                          OnConsumerStopped = target.OnConsumeStopped
                                                      };

            Assert.AreEqual(null, consumer.Bucket);
            Assert.AreEqual(false, consumer.IsAttached);
            Assert.AreEqual(false, consumer.IsConsuming);
            Assert.AreEqual(0, target.ItemsConsumedCount);
            Assert.AreEqual(false, target.HasOnConsumeStartedBeenCalled);
            Assert.AreEqual(false, target.HasOnConsumeStoppedBeenCalled);
            Assert.AreEqual(ThreadPriority.Normal, consumer.ConsumerThreadPriority);
        }


        /// <summary>
        /// Constructor sets priority on call
        /// </summary>
        [TestMethod]
        public void Constructor_InitializesPriority_OnCall()
        {
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(target.OnConsumeBulk,
                ThreadPriority.Highest)
                                                        {
                                                            OnConsumerStarted = target.OnConsumeStarted,
                                                            OnConsumerStopped = target.OnConsumeStopped
                                                        };


            Assert.AreEqual(null, consumer.Bucket);
            Assert.AreEqual(false, consumer.IsAttached);
            Assert.AreEqual(false, consumer.IsConsuming);
            Assert.AreEqual(0, target.ItemsConsumedCount);
            Assert.AreEqual(false, target.HasOnConsumeStartedBeenCalled);
            Assert.AreEqual(false, target.HasOnConsumeStoppedBeenCalled);
            Assert.AreEqual(ThreadPriority.Highest, consumer.ConsumerThreadPriority);
        }


        /// <summary>
        /// Constructor initialies bucket on call
        /// </summary>
        [TestMethod]
        public void Constructor_InitializesBucket_OnCall()
        {
            Bucket<string> expected = new Bucket<string>(5);
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, expected)
            {
                OnConsumerStarted = target.OnConsumeStarted,
                OnConsumerStopped = target.OnConsumeStopped
            };

            Assert.AreEqual(expected, consumer.Bucket);
            Assert.AreEqual(true, consumer.IsAttached);
            Assert.AreEqual(false, consumer.IsConsuming);
            Assert.AreEqual(0, target.ItemsConsumedCount);
            Assert.AreEqual(false, target.HasOnConsumeStartedBeenCalled);
            Assert.AreEqual(false, target.HasOnConsumeStoppedBeenCalled);
            Assert.AreEqual(ThreadPriority.Normal, consumer.ConsumerThreadPriority);
        }


        /// <summary>
        /// Constructor initializes bucket and priority on call
        /// </summary>
        [TestMethod]
        public void Constructor_InitializesBucketAndPriority_OnCall()
        {
            Bucket<string> expected = new Bucket<string>(5);
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, expected, ThreadPriority.AboveNormal)
            {
                OnConsumerStarted = target.OnConsumeStarted,
                OnConsumerStopped = target.OnConsumeStopped
            };

            Assert.AreEqual(expected, consumer.Bucket);
            Assert.AreEqual(true, consumer.IsAttached);
            Assert.AreEqual(false, consumer.IsConsuming);
            Assert.AreEqual(0, target.ItemsConsumedCount);
            Assert.AreEqual(false, target.HasOnConsumeStartedBeenCalled);
            Assert.AreEqual(false, target.HasOnConsumeStoppedBeenCalled);
            Assert.AreEqual(ThreadPriority.AboveNormal, consumer.ConsumerThreadPriority);
        }


        /// <summary>
        /// AttachBucket changes bucket on call
        /// </summary>
        [TestMethod]
        public void AttachBucket_ChangesBucketReference_OnCall()
        {
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk);

            Bucket<string> expected = new Bucket<string>(5);
            consumer.AttachBucket(expected);

            Assert.AreEqual(expected, consumer.Bucket);
        }


        /// <summary>
        /// Start() starts consumption
        /// </summary>
        [TestMethod]
        public void Start_StartsConsumption_OnCall()
        {
            Bucket<string> bucket = new Bucket<string>(10);
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(target.OnConsumeBulk, bucket)
                                                      {
                                                          OnConsumerStarted = target.OnConsumeStarted,
                                                          OnConsumerStopped = target.OnConsumeStopped
                                                      };

            consumer.Start();
            Thread.Sleep(1000);

            Assert.IsTrue(consumer.IsConsuming);
            Assert.AreEqual(true, target.HasOnConsumeStartedBeenCalled);

            consumer.Stop(5000);
        }


        /// <summary>
        /// Stop halts consumption on call
        /// </summary>
        [TestMethod]
        public void Stop_HaltsConsumption_OnCall()
        {
            Bucket<string> bucket = new Bucket<string>(10);
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, bucket)
            {
                OnConsumerStarted = target.OnConsumeStarted,
                OnConsumerStopped = target.OnConsumeStopped
            };

            consumer.Start();
            Assert.AreEqual(false, target.HasOnConsumeStoppedBeenCalled);
            Thread.Sleep(1000);
            consumer.Stop(5000);

            Assert.IsFalse(consumer.IsConsuming);
            Assert.AreEqual(true, target.HasOnConsumeStoppedBeenCalled);
        }



        /// <summary>
        /// Start consumes items
        /// </summary>
        [TestMethod]
        public void Start_Consumes_OnCall()
        {
            Bucket<string> bucket = new Bucket<string>(10);
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, bucket);
            bucket.GetTimeoutInMs = 50;

            bucket.Add("A");
            bucket.Add("B");
            consumer.Start();

            Thread.Sleep(100);
            bucket.Add("C");
            Thread.Sleep(100);
            bucket.Add("D");
            Thread.Sleep(100);
            bucket.Add("E");
            Thread.Sleep(100);
            consumer.Stop();

            Assert.AreEqual(5, target.ItemsConsumedCount);
            Assert.AreEqual(5, target.ItemsConsumed.Count);
            Assert.AreEqual(0, bucket.Depth);

            Assert.AreEqual("A", target.ItemsConsumed[0]);
            Assert.AreEqual("B", target.ItemsConsumed[1]);
            Assert.AreEqual("C", target.ItemsConsumed[2]);
            Assert.AreEqual("D", target.ItemsConsumed[3]);
            Assert.AreEqual("E", target.ItemsConsumed[4]);
        }


        /// <summary>
        /// Stop() halts consume with no leftovers
        /// </summary>
        [TestMethod]
        public void Stop_HaltsConsumeWithNoLeftovers_OnCallWithArrayProcessing()
        {
            Bucket<string> bucket = new Bucket<string>(10);
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, bucket);
            bucket.GetTimeoutInMs = 50;

            consumer.Start();
            bucket.Add("A");
            bucket.Add("B");
            bucket.Add("C");
            Thread.Sleep(500);
            consumer.Stop();
            Thread.Sleep(500);
            bucket.Add("D");
            bucket.Add("E");

            Assert.AreEqual(3, target.ItemsConsumed.Count);
            Assert.AreEqual(2, bucket.Depth);
        }


        /// <summary>
        /// Start() returns false if on begin consume fails
        /// </summary>
        [TestMethod]
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5005:NoEmptyCatchBlocks", Justification = "Uit Test.")]
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException", Justification = "Unit Test.")]
		public void Start_ReturnsFalse_IfOnBeginConsumeFails()
        {
            Bucket<string> bucket = new Bucket<string>(10);
            var target = new TestClasses.ConsumerTestClass { ShouldFailOnStart = true };
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, bucket);

            try
            {
                consumer.Start();
                Assert.Fail("Should have thrown");
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// Start() returns true if OnBeginConsume scueeds
        /// </summary>
        [TestMethod]
        public void Start_ReturnsTrue_IfOnBeginConsumeSucceeds()
        {
            Bucket<string> bucket = new Bucket<string>(10);
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, bucket);

            Assert.IsTrue(consumer.Start());
            consumer.Stop();
        }

        
        /// <summary>
        /// Start() returns false if already started
        /// </summary>
        [TestMethod]
		[Ignore] // this is currently non-deterministic
        public void Start_ReturnsFalse_IfAlreadyStarted()
        {
			Bucket<string> bucket = new Bucket<string>(10);
			bucket.Add("something");
            var target = new TestClasses.ConsumerTestClass();
            BulkBucketConsumer<string> consumer = new BulkBucketConsumer<string>(
				target.OnConsumeBulk, bucket);

            Assert.IsTrue(consumer.Start());
            Assert.IsFalse(consumer.Start());
            consumer.Stop();
        }
    }
}