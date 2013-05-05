using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;

namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// This class is a test fixture for the Disposable class.
    /// </summary>
    [TestClass]
    public class DisposableTest
    {
        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullActionThrows()
        {
            Action disposeAction = null;

            Disposable.Create(disposeAction);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullFuncThrows()
        {
            Func<bool> disposeAction = null;

            Disposable.Create(disposeAction);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        public void DisposeWithActionTriggersActionOnHappyPath()
        {
            int callCount = 0;

            using (Disposable.Create(() => { ++callCount; }))
            {
                Assert.AreEqual(0, callCount);
            }

            Assert.AreEqual(1, callCount);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        public void DisposeWithActionTriggersActionOnExceptionPath()
        {
            int callCount = 0;

            try
            {
                using (Disposable.Create(() => { ++callCount; }))
                {
                    Assert.AreEqual(0, callCount);

                    throw new Exception();
                }
            }
            catch (Exception)
            {
            }

            Assert.AreEqual(1, callCount);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        public void DisposeWithActionWithRepeatedDisposeOnlyDisposesOnce()
        {
            int callCount = 0;
            IDisposable item = Disposable.Create(() => { ++callCount; });

            using (item)
            {
                Assert.AreEqual(0, callCount);
            }

            Assert.AreEqual(1, callCount);

            // these should not increment
            item.Dispose();
            item.Dispose();
            item.Dispose();

            Assert.AreEqual(1, callCount);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        public void DisposeWithFuncTriggersFuncOnHappyPath()
        {
            int callCount = 0;

            using (Disposable.Create(() => { ++callCount; return true; }))
            {
                Assert.AreEqual(0, callCount);
            }

            Assert.AreEqual(1, callCount);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        public void DisposeWithFuncTriggersFuncOnExceptionPath()
        {
            int callCount = 0;

            try
            {
                using (Disposable.Create(() => { ++callCount; return true; }))
                {
                    Assert.AreEqual(0, callCount);

                    throw new Exception();
                }
            }
            catch (Exception)
            {
            }

            Assert.AreEqual(1, callCount);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        public void DisposeWithFuncWithRepeatedReturningTrueDisposeOnlyDisposesOnce()
        {
            int callCount = 0;
            var item = Disposable.Create(() => { ++callCount; return true; });

            using (item)
            {
                Assert.AreEqual(0, callCount);
            }

            Assert.AreEqual(1, callCount);

            // these should not increment since the Func returns true
            item.Dispose();
            item.Dispose();
            item.Dispose();

            Assert.AreEqual(1, callCount);
        }

        /// <summary>
        /// Test method for Disposable class
        /// </summary>
        [TestMethod]
        public void DisposeWithFuncWithRepeatedReturningFalseDisposesMany()
        {
            int callCount = 0;
            var item = Disposable.Create(() => { ++callCount; return false; });

            using (item)
            {
                Assert.AreEqual(0, callCount);
            }

            Assert.AreEqual(1, callCount);

            // these should increment since the Func returns false
            item.Dispose();
            Assert.AreEqual(2, callCount);

            item.Dispose();
            Assert.AreEqual(3, callCount);

            item.Dispose();
            Assert.AreEqual(4, callCount);
        }
    }
}
