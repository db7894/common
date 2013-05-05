using System;
using System.Diagnostics;
using System.Threading;
using SharedAssemblies.General.Threading.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Threading.UnitTests
{
	/// <summary>
	/// This is a test class for BucketTest and is intended
	/// to contain all BucketTest Unit Tests
	/// </summary>
	[TestClass]
	public class CancellationTokenExtensionsTest
	{
		/// <summary>
		/// The test method for Wait() extension method.
		/// </summary>
		[TestMethod]
		public void WaitWithTimeSpanWhenNotCancelled()
		{
			var source = new CancellationTokenSource();
			var timer = Stopwatch.StartNew();

			var result = source.Token.Wait(TimeSpan.FromSeconds(1.0));

			timer.Stop();

			Assert.IsFalse(result);
			Assert.IsTrue(timer.Elapsed.TotalSeconds >= 0.9);
		}

		/// <summary>
		/// The test method for Wait() extension method.
		/// </summary>
		[TestMethod]
		public void WaitWithIntegerWhenNotCancelled()
		{
			var source = new CancellationTokenSource();
			var timer = Stopwatch.StartNew();

			var result = source.Token.Wait(1000);

			timer.Stop();

			Assert.IsFalse(result);
			Assert.IsTrue(timer.Elapsed.TotalSeconds >= 0.9);
		}

		/// <summary>
		/// The test method for Wait() extension method.
		/// </summary>
		[TestMethod]
		public void WaitWithTimeSpanWhenCancelled()
		{
			var source = new CancellationTokenSource();
			source.Cancel();

			var timer = Stopwatch.StartNew();

			var result = source.Token.Wait(TimeSpan.FromSeconds(1.0));

			timer.Stop();

			Assert.IsTrue(result);
			Assert.IsTrue(timer.Elapsed.TotalSeconds < 1.0);
		}

		/// <summary>
		/// The test method for Wait() extension method.
		/// </summary>
		[TestMethod]
		public void WaitWithIntegerWhenCancelled()
		{
			var source = new CancellationTokenSource();
			source.Cancel();

			var timer = Stopwatch.StartNew();

			var result = source.Token.Wait(1000);

			timer.Stop();

			Assert.IsTrue(result);
			Assert.IsTrue(timer.Elapsed.TotalSeconds < 1.0);
		}

		/// <summary>
		/// The test method for Wait() extension method.
		/// </summary>
		[TestMethod]
		public void WaitWhenCancelled()
		{
			var source = new CancellationTokenSource();
			source.Cancel();

			var result = source.Token.Wait();

			Assert.IsTrue(result);
		}
	}
}
