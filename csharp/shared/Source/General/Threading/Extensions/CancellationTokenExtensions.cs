using System;
using System.Threading;

namespace SharedAssemblies.General.Threading.Extensions
{
	/// <summary>
	/// Extension methods on cancellation token to make it easier to wait for a cancel
	/// </summary>
	public static class CancellationTokenExtensions
	{
		/// <summary>
		/// Waits for the cancellation token to be set, or until timeout, whichever happens first.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		/// <param name="timeout">The TimeSpan representing a timeout.</param>
		/// <returns>True if the token was set, false if timeout occurred.</returns>
		public static bool Wait(this CancellationToken token, TimeSpan timeout)
		{
			return token.WaitHandle.WaitOne(timeout);
		}

		/// <summary>
		/// Waits for the cancellation token to be set, or until timeout, whichever happens first.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		/// <param name="timeoutInMs">An int representing a timeout in milliseconds.</param>
		/// <returns>True if the token was set, false if timeout occurred.</returns>
		public static bool Wait(this CancellationToken token, int timeoutInMs)
		{
			return token.WaitHandle.WaitOne(timeoutInMs);
		}

		/// <summary>
		/// Waits for the cancellation token to be set, no timeout.
		/// </summary>
		/// <param name="token">The cancellation token.</param>
		/// <returns>Always returns true.</returns>
		public static bool Wait(this CancellationToken token)
		{
			return token.WaitHandle.WaitOne();
		}
	}
}
