using System;
using System.Threading;
using System.Threading.Tasks;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.General.Threading.Extensions;


namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// A repeater class that continues to repeat an action for a given duration or count.
	/// </summary>
	public static class Repeat
	{
		/// <summary>
		/// Creates a new task that calls a delegate which performs some work, the delegate returns true
		/// if there is more work that immediately needs to be done, or false if should wait for more work.
		/// </summary>
		/// <param name="workDelegate">Delegate to perform work, should return true if more work ready, false if wait and try again.</param>
		/// <param name="waitTimeoutOnNoWork">The time to wait if no work immediately available.</param>
		/// <param name="token">The cancellation token to interrupt the interval task.</param>
		/// <returns>The task that was started.</returns>
		public static Task Interval(Func<bool> workDelegate, TimeSpan waitTimeoutOnNoWork, CancellationToken token)
		{
			if (workDelegate == null)
			{
				throw new ArgumentNullException("workDelegate");
			}

			if (waitTimeoutOnNoWork.TotalMilliseconds <= 0)
			{
				throw new ArgumentOutOfRangeException("waitTimeoutOnNoWork", "Timeout on no work must be positive number of milliseconds");
			}

			// create a task that performs the work delegate, waiting for the timeout on the cancellation token 
			// if no work, until cancellation.
			return Task.Factory.StartNew(() => PerformUntilCancelled(workDelegate, waitTimeoutOnNoWork, token),
			                             token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		/// <summary>
		/// Creates a new task that calls a delegate which performs some work, the delegate returns true
		/// if there is more work that immediately needs to be done, or false if should wait for more work.
		/// </summary>
		/// <param name="workDelegate">Delegate to perform work, should return true if more work ready, false if wait and try again.</param>
		/// <param name="waitTimeoutAfterWork">The time to wait if no work immediately available.</param>
		/// <param name="token">The cancellation token to interrupt the interval task.</param>
		/// <returns>The task that was started.</returns>
		public static Task Interval(Action workDelegate, TimeSpan waitTimeoutAfterWork, CancellationToken token)
		{
			if (workDelegate == null)
			{
				throw new ArgumentNullException("workDelegate");
			}

			if (waitTimeoutAfterWork.TotalMilliseconds <= 0)
			{
				throw new ArgumentOutOfRangeException("waitTimeoutAfterWork", "Timeout on no work must be positive number of milliseconds");
			}

			return Interval(workDelegate.AsPredicate(), waitTimeoutAfterWork, token);
		}

		/// <summary>
		/// Creates a new task that calls a delegate which performs some work, the delegate returns true
		/// if there is more work that immediately needs to be done, or false if should wait for more work.
		/// </summary>
		/// <param name="workDelegate">Delegate to perform work, should return true if more work ready, false if wait and try again.</param>
		/// <param name="timeout">The timeout to wait.</param>
		/// <param name="token">The cancellation token.</param>
		private static void PerformUntilCancelled(Func<bool> workDelegate, TimeSpan timeout, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				// perform the work, returns true if should skip wait because more work ready now.
				bool isMoreWorkReadyNow = workDelegate();

				// then wait for the timeout before trying again.
				if (!isMoreWorkReadyNow)
				{
					token.Wait(timeout);
				}
			}
		}
	}
}
