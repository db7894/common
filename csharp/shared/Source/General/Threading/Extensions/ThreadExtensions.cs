using System;
using System.Threading;

namespace SharedAssemblies.General.Threading.Extensions
{
    /// <summary>
    /// Extension methods for the thread class to help clean-up common thread operations
    /// </summary>
    public static class ThreadExtensions
    {
        /// <summary>
        /// This helper method attempts to join the thread first and then aborts if the thread
        /// does not join within the time indicated.  It should be noted that you should not
        /// rely on the bool result to decide if the thread is stopped, all this means is that
        /// the join was successful within the timeout indicated
        /// </summary>
        /// <param name="thread">Thread to join and possibly abort if timeout passes</param>
        /// <param name="timeToWaitInMs">Time in ms to wait for a join</param>
        /// <returns>True if the thread joins in the timeout indicated</returns>
        public static bool JoinOrAbort(this Thread thread, int timeToWaitInMs)
        {
			if (thread == null)
			{
				throw new ArgumentNullException("thread");
			}

            bool isJoined = thread.Join(timeToWaitInMs);

            if (!isJoined)
            {
                thread.Abort();
            }

            return isJoined;
        }

	
		/// <summary>
		/// This helper method attempts to join the thread first and then aborts if the thread
		/// does not join within the time indicated.  It should be noted that you should not
		/// rely on the bool result to decide if the thread is stopped, all this means is that
		/// the join was successful within the timeout indicated
		/// </summary>
		/// <param name="thread">Thread to join and possibly abort if timeout passes</param>
		/// <param name="timeToWait">TimeSpan duration to wait for a join</param>
		/// <returns>True if the thread joins in the timeout indicated</returns>
		public static bool JoinOrAbort(this Thread thread, TimeSpan timeToWait)
		{
			if (thread == null)
			{
				throw new ArgumentNullException("thread");
			}

			bool isJoined = thread.Join(timeToWait);

			if (!isJoined)
			{
				thread.Abort();
			}

			return isJoined;
		}
	}
}
