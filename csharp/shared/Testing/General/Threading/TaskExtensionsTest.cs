using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Threading.Extensions;


namespace SharedAssemblies.General.Threading.UnitTests
{
    /// <summary>
    /// This is a test class for BucketTest and is intended
    /// to contain all BucketTest Unit Tests
    /// </summary>
    [TestClass]
    public class TaskExtensionsTest
    {
        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryDisposeThrowsOnNullSource()
        {
            Task task = null;

            task.TryDispose();
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryDisposeReturnsFalseWhenNotStarted()
        {
            var task = new Task(() => Thread.Sleep(1000));

            var result = task.TryDispose();

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryDisposeReturnsTrueWhenNotStartedButCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            var result = task.TryDispose();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryDisposeReturnsTrueWhenStartedAfterCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            try
            {
                task.Start();
            }
            catch (Exception)
            {
            }

            var result = task.TryDispose();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryDisposeReturnsFalseWhenRunning()
        {
            var task = Task.Factory.StartNew(() => Thread.Sleep(1000));

            var result = task.TryDispose();

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryDisposeReturnsTrueWhenFinished()
        {
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token);

            // make sure completes
            task.Wait(1000);

            var result = task.TryDispose();

            Assert.IsTrue(result);
        }


        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryDisposeReturnsTrueWhenCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token);
            taskSource.Cancel();

            // make sure cancel completes
            task.TryWait(TimeSpan.FromSeconds(1.0));

            var result = task.TryDispose();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitThrowsOnNullSource()
        {
            Task task = null;

            task.TryWait();
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitWithTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task task = null;

            task.TryWait(token.Token);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitWithTimeSpanThrowsOnNullSource()
        {
            Task task = null;

            task.TryWait(TimeSpan.FromSeconds(5.0));
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitWithTimeSpanAndTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task task = null;

            task.TryWait(TimeSpan.FromSeconds(5.0), token.Token);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitWithTimeoutThrowsOnNullSource()
        {
            Task task = null;

            task.TryWait(5000);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitWithTimeoutAndTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task task = null;

            task.TryWait(5000, token.Token);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithTimeSpanReturnsFalseWhenNotStarted()
        {
            var task = new Task(() => Thread.Sleep(1000));

            var result = task.TryWait(TimeSpan.FromSeconds(2.0));

            Assert.IsFalse(result);
        }


        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithTimeSpanReturnsTrueWhenNotStartedButCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            var result = task.TryWait(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithTimeSpanReturnsTrueWhenStartedAfterCancel()
        {
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            try
            {
                task.Start();
            }
            catch (Exception)
            {
            }

            var result = task.TryWait(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithTimeSpanReturnsTrueWhenFinished()
        {
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));

            var result = task.TryWait(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithTimeSpanReturnsTrueWhenFinishedBeforeWait()
        {
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));

            // wait till task actually done
            task.TryWait();

            // now try waiting again on already done task
            var result = task.TryWait(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithTimeSpanReturnsTrueWhenCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token);
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWait(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithTimeSpanReturnsTrueWhenFaulted()
        {
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token);

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWait(TimeSpan.FromSeconds(2.0));

            task.TryDispose();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutReturnsFalseWhenNotStarted()
        {
            var task = new Task(() => Thread.Sleep(1000));

            var result = task.TryWait(2000);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutReturnsTrueWhenNotStartedButCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            var result = task.TryWait(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutReturnsTrueWhenStartedAfterCancel()
        {
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            try
            {
                task.Start();
            }
            catch (Exception)
            {
            }

            var result = task.TryWait(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutReturnsTrueWhenFinished()
        {
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));

            var result = task.TryWait(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutReturnsTrueWhenFinishedBeforeWait()
        {
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));

            // wait till task actually done
            task.TryWait();

            // now try waiting again on already done task
            var result = task.TryWait(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutReturnsTrueWhenCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token);
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWait(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutReturnsTrueWhenFaulted()
        {
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token);

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWait(2000);

            task.TryDispose();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsFalseWhenNotStarted()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000));

            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsFalseWhenNotStartedWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000));

            // cancel wait before we even begin
            cxlSource.Cancel();

            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnTrueWhenNotStartedButCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnTrueWhenNotStartedButCancelledWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            // cancel the wait
            cxlSource.Cancel();

            // due to underlying nature of Wait(), even if token is cancelled, if task already complete returns true.
            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenStartedAfterCancel()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();

            try
            {
                task.Start();
            }
            catch (Exception)
            {
            }

            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenStartedAfterCancelWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new Task(() => Thread.Sleep(1000), taskSource.Token);
            taskSource.Cancel();
            cxlSource.Cancel();

            try
            {
                task.Start();
            }
            catch (Exception)
            {
            }

            // due to underlying nature of Wait(), even if token is cancelled, if task already complete returns true.
            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenFinished()
        {
            var cxlSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));

            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsFalseWhenFinishedWithCancelledWait()
        {
            var cxlSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));
            cxlSource.Cancel();

            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenFinishedBeforeWait()
        {
            var cxlSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));

            // wait till task actually done
            task.TryWait();

            // now try waiting again on already done task
            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenFinishedBeforeWaitWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10));
            cxlSource.Cancel();

            // wait till task actually done
            task.TryWait();

            // due to underlying nature of Wait(), even if token is cancelled, if task already complete returns true.
            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token);
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenFaulted()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token);

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWait(2000, cxlSource.Token);

            task.TryDispose();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitWithIntTimeoutAndTokenReturnsTrueWhenCancelledWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token);
            taskSource.Cancel();
            cxlSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));

            // due to underlying nature of Wait(), even if token is cancelled, if task already complete returns true.
            var result = task.TryWait(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAllThrowsOnNullSource()
        {
            Task[] tasks = null;

            tasks.TryWaitAll();
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAllWithTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task[] tasks = null;

            tasks.TryWaitAll(token.Token);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAllWithTimeSpanThrowsOnNullSource()
        {
            Task[] tasks = null;

            tasks.TryWaitAll(TimeSpan.FromSeconds(5.0));
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAllWithTimeSpanAndTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task[] tasks = null;

            tasks.TryWaitAll(TimeSpan.FromSeconds(5.0), token.Token);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAllWithTimeoutThrowsOnNullSource()
        {
            Task[] tasks = null;

            tasks.TryWaitAll(5000);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAllWithTimeoutAndTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task[] tasks = null;

            tasks.TryWaitAll(5000, token.Token);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithTimeSpanReturnsFalseWhenNotStarted()
        {
            var task = new[] { new Task(() => Thread.Sleep(1000)) };

            var result = task.TryWaitAll(TimeSpan.FromSeconds(2.0));

            Assert.IsFalse(result);
        }


        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithTimeSpanReturnsTrueWhenNotStartedButCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            var result = task.TryWaitAll(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithTimeSpanReturnsTrueWhenStartedAfterCancel()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            var result = task.TryWaitAll(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithTimeSpanReturnsTrueWhenFinished()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            var result = task.TryWaitAll(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithTimeSpanReturnsTrueWhenFinishedBeforeWait()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            // wait till task actually done
            task.TryWaitAll();

            // now try waiting again on already done task
            var result = task.TryWaitAll(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithTimeSpanReturnsTrueWhenCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAll(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithTimeSpanReturnsTrueWhenFalted()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token) };

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAll(TimeSpan.FromSeconds(2.0));

            Array.ForEach(task, t => t.TryDispose());

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutReturnsFalseWhenNotStarted()
        {
            var task = new[] { new Task(() => Thread.Sleep(1000)) };

            var result = task.TryWaitAll(2000);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutReturnsTrueWhenNotStartedButCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            var result = task.TryWaitAll(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutReturnsTrueWhenStartedAfterCancel()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            var result = task.TryWaitAll(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutReturnsTrueWhenFinished()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            var result = task.TryWaitAll(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutReturnsTrueWhenFinishedBeforeWait()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            // wait till task actually done
            task.TryWaitAll();

            // now try waiting again on already done task
            var result = task.TryWaitAll(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutReturnsTrueWhenCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAll(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutReturnsTrueWhenFaulted()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token) };

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAll(2000);

            Array.ForEach(task, t => t.TryDispose());

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsFalseWhenNotStarted()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000)) };

            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsFalseWhenNotStartedWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000)) };
            cxlSource.Cancel();

            // Due to WaitAll() throwing when cancellation token is cancelled, TryWaitAll() returns false on cancel
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnTrueWhenNotStartedButCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnFalseWhenNotStartedButCancelledWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();
            cxlSource.Cancel();

            // Due to WaitAll() throwing when cancellation token is cancelled, TryWaitAll() returns false on cancel
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsTrueWhenStartedAfterCancel()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsFalseWhenStartedAfterCancelWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();
            cxlSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            // Due to WaitAll() throwing when cancellation token is cancelled, TryWaitAll() returns false on cancel
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsTrueWhenFinished()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsFalseWhenFinishedWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };
            cxlSource.Cancel();

            // Due to WaitAll() throwing when cancellation token is cancelled, TryWaitAll() returns false on cancel
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsTrueWhenFinishedBeforeWait()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            // wait till task actually done
            task.TryWaitAll();

            // now try waiting again on already done task
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsFalseWhenFinishedBeforeWaitWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };
            cxlSource.Cancel();

            // wait till task actually done
            task.TryWaitAll();

            // Due to WaitAll() throwing when cancellation token is cancelled, TryWaitAll() returns false on cancel
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsTrueWhenCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsTrueWhenFaulted()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token) };

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Array.ForEach(task, t => t.TryDispose());

            Assert.IsTrue(result);

        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAllWithIntTimeoutAndTokenReturnsFalseWhenCancelledWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();
            cxlSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));

            // Due to WaitAll() throwing when cancellation token is cancelled, TryWaitAll() returns false on cancel
            var result = task.TryWaitAll(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAnyThrowsOnNullSource()
        {
            Task[] tasks = null;

            tasks.TryWaitAny();
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAnyWithTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task[] tasks = null;

            tasks.TryWaitAny(token.Token);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAnyWithTimeSpanThrowsOnNullSource()
        {
            Task[] tasks = null;

            tasks.TryWaitAny(TimeSpan.FromSeconds(5.0));
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAnyWithTimeSpanAndTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task[] tasks = null;

            tasks.TryWaitAny(TimeSpan.FromSeconds(5.0), token.Token);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAnyWithTimeoutThrowsOnNullSource()
        {
            Task[] tasks = null;

            tasks.TryWaitAny(5000);
        }

        /// <summary>
        /// Test method for null source
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryWaitAnyWithTimeoutAndTokenThrowsOnNullSource()
        {
            var token = new CancellationTokenSource();
            Task[] tasks = null;

            tasks.TryWaitAny(5000, token.Token);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithTimeSpanReturnsFalseWhenNotStarted()
        {
            var task = new[] { new Task(() => Thread.Sleep(1000)) };

            var result = task.TryWaitAny(TimeSpan.FromSeconds(2.0));

            Assert.IsFalse(result);
        }


        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithTimeSpanReturnsTrueWhenNotStartedButCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            var result = task.TryWaitAny(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithTimeSpanReturnsTrueWhenStartedAfterCancel()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            var result = task.TryWaitAny(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithTimeSpanReturnsTrueWhenFinished()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            var result = task.TryWaitAny(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithTimeSpanReturnsTrueWhenFinishedBeforeWait()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            // wait till task actually done
            task.TryWaitAny();

            // now try waiting again on already done task
            var result = task.TryWaitAny(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithTimeSpanReturnsTrueWhenCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAny(TimeSpan.FromSeconds(2.0));

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithTimeSpanReturnsTrueWhenFalted()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token) };

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAny(TimeSpan.FromSeconds(2.0));

            Array.ForEach(task, t => t.TryDispose());

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutReturnsFalseWhenNotStarted()
        {
            var task = new[] { new Task(() => Thread.Sleep(1000)) };

            var result = task.TryWaitAny(2000);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutReturnsTrueWhenNotStartedButCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            var result = task.TryWaitAny(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutReturnsTrueWhenStartedAfterCancel()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            var result = task.TryWaitAny(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutReturnsTrueWhenFinished()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            var result = task.TryWaitAny(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutReturnsTrueWhenFinishedBeforeWait()
        {
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            // wait till task actually done
            task.TryWaitAny();

            // now try waiting again on already done task
            var result = task.TryWaitAny(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutReturnsTrueWhenCancelled()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAny(2000);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutReturnsTrueWhenFaulted()
        {
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token) };

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAny(2000);

            Array.ForEach(task, t => t.TryDispose());

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsFalseWhenNotStarted()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000)) };

            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsFalseWhenNotStartedWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000)) };
            cxlSource.Cancel();

            // Due to WaitAny() throwing when cancellation token is cancelled, TryWaitAny() returns false on cancel
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnTrueWhenNotStartedButCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnFalseWhenNotStartedButCancelledWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();
            cxlSource.Cancel();

            // Due to WaitAny() throwing when cancellation token is cancelled, TryWaitAny() returns false on cancel
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsTrueWhenStartedAfterCancel()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsFalseWhenStartedAfterCancelWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { new Task(() => Thread.Sleep(1000), taskSource.Token) };
            taskSource.Cancel();
            cxlSource.Cancel();

            try
            {
                Array.ForEach(task, t => t.Start());
            }
            catch (Exception)
            {
            }

            // Due to WaitAny() throwing when cancellation token is cancelled, TryWaitAny() returns false on cancel
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsTrueWhenFinished()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsFalseWhenFinishedWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };
            cxlSource.Cancel();

            // Due to WaitAny() throwing when cancellation token is cancelled, TryWaitAny() returns false on cancel
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsTrueWhenFinishedBeforeWait()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };

            // wait till task actually done
            task.TryWaitAny();

            // now try waiting again on already done task
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsFalseWhenFinishedBeforeWaitWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10)) };
            cxlSource.Cancel();

            // wait till task actually done
            task.TryWaitAny();

            // Due to WaitAny() throwing when cancellation token is cancelled, TryWaitAny() returns false on cancel
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsTrueWhenCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsTrueWhenFaulted()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => { throw new Exception(); }, taskSource.Token) };

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Array.ForEach(task, t => t.TryDispose());

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test method for the Task extension method
        /// </summary>
        [TestMethod]
        public void TryWaitAnyWithIntTimeoutAndTokenReturnsFalseWhenCancelledWithWaitCancelled()
        {
            var cxlSource = new CancellationTokenSource();
            var taskSource = new CancellationTokenSource();
            var task = new[] { Task.Factory.StartNew(() => Thread.Sleep(10), taskSource.Token) };
            taskSource.Cancel();
            cxlSource.Cancel();

            // wait for task to cancel out, then wait for it.
            Thread.Sleep(TimeSpan.FromSeconds(1.0));

            // Due to WaitAny() throwing when cancellation token is cancelled, TryWaitAny() returns false on cancel
            var result = task.TryWaitAny(2000, cxlSource.Token);

            Assert.IsFalse(result);
        }
    }
}
