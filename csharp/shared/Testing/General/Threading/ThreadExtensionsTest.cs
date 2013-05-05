using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Threading.Extensions;


namespace SharedAssemblies.General.Threading.UnitTests
{
	/// <summary>
	/// Unit tests for ThreadExtensions
	/// </summary>
	[TestClass]
	public class ThreadExtensionsTest
	{
		/// <summary>
		/// Unit test for JoinOrAbort()
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void JoinOrAbortOnNullThreadThrows()
		{
			Thread thread = null;

			Assert.IsFalse(thread.JoinOrAbort(5000));
		}

		/// <summary>
		/// Unit test for JoinOrAbort()
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void JoinOrAbortWithTimeSpanOnNullThreadThrows()
		{
			Thread thread = null;

			Assert.IsFalse(thread.JoinOrAbort(TimeSpan.FromSeconds(5.0)));
		}

		/// <summary>
		/// Unit test for JoinOrAbort()
		/// </summary>
		[TestMethod]
		public void JoinOrAbortReturnsTrueOnCompletedThread()
		{
			var thread = new Thread(() => Thread.Sleep(10));
			thread.Start();

			Assert.IsTrue(thread.JoinOrAbort(5000));
		}

		/// <summary>
		/// Unit test for JoinOrAbort()
		/// </summary>
		[TestMethod]
		public void JoinOrAbortWithTimeSpanReturnsTrueOnCompletedThread()
		{
			var thread = new Thread(() => Thread.Sleep(10));
			thread.Start();

			Assert.IsTrue(thread.JoinOrAbort(TimeSpan.FromSeconds(5.0)));
		}

		/// <summary>
		/// Unit test for JoinOrAbort()
		/// </summary>
		[TestMethod]
		public void JoinOrAbortReturnsFalseOnRunningThread()
		{
			var thread = new Thread(() => Thread.Sleep(10000));
			thread.Start();

			Assert.IsFalse(thread.JoinOrAbort(10));
		}

		/// <summary>
		/// Unit test for JoinOrAbort()
		/// </summary>
		[TestMethod]
		public void JoinOrAbortWithTimeSpanReturnsFalseOnRunningThread()
		{
			var thread = new Thread(() => Thread.Sleep(10000));
			thread.Start();

			Assert.IsFalse(thread.JoinOrAbort(TimeSpan.FromMilliseconds(10.0)));
		}
	}
}
