using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedAssemblies.General.Threading.Extensions
{
	/// <summary>
	/// Extension methods for the TPL Task class
	/// </summary>
	public static class TaskExtensions
	{
		/// <summary>
		/// <para>
		/// Attempts to dispose of a Task, but will not propagate the exception.  Returns false instead if
		/// the Task could not be disposed.
		/// </para>
		/// <para>
		/// A task may only be disposed if it is in a Cancelled, Faulted, or RunToCompletion.  Calling Dispose() on a task
		/// in these three states will return <b>true</b>.  If the task is Running or NotStarted, this will return <b>false</b>.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of Dispose() throwing if the task is Running or NotStarted, TryDispose() will fail (return false)
		/// in these cases.  No extraordinary measures should be taken to call Dispose() again, according to MSDN.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
		/// <returns>True if disposed successfully.</returns>
		public static bool TryDispose(this Task source, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");				
			}

            try
            {
                // no sense attempting to dispose unless we are completed, otherwise we know we'll throw
                // and why add the overhead.
                if (source.IsCompleted)
                {
                    if (shouldMarkExceptionsHandled && source.Exception != null)
                    {
                        // just accessing the getter so the exception is marked as handled.
                        source.Exception.Flatten().Handle(x => true);
                    }

                    source.Dispose();
                    return true;
                }
            }

            catch (Exception)
            {                
                // consume any other possible exception on dispose so dispose is as safe as possible
            }

            // return false if any exception occurred or because task has not yet completed.
            return false;
		}

		/// <summary>
		/// Attempts to wait on a task.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
		public static bool TryWait(this Task source, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				source.Wait();
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }
            }

            return true;
        }
	
		/// <summary>
		/// <para>
		/// Attempts to wait on a task.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this behaves as the underlying Wait() does, which is to return <b>true</b> automatically.  This is in contrast to 
		/// WaitAll() and WaitAny() that throw OperationCancelledException.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of Wait() returning <b>true</b> if the task is already run to completion,
		/// even if the CancellationToken is set, this will return <b>true</b> even if the CancellationToken is already set.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
		public static bool TryWait(this Task source, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				source.Wait(token);
				return true;
			}

			catch (OperationCanceledException)
			{
				// if the token was cancelled, we didn't wait till task ended, so return false.
				return false;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// Attempts to wait on a task.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeout">The time to wait for the task.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
		public static bool TryWait(this Task source, TimeSpan timeout, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return source.Wait(timeout);
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }

		}

		/// <summary>
		/// Attempts to wait on a task.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeoutInMs">The time to wait for the task.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWait(this Task source, int timeoutInMs, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return source.Wait(timeoutInMs);
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }

		}

		/// <summary>
		/// <para>
		/// Attempts to wait on a task.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this behaves as the underlying Wait() does, which is to return <b>true</b> automatically.  This is in contrast to 
		/// WaitAll() and WaitAny() that throw OperationCancelledException.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of Wait() returning <b>true</b> if the task is already run to completion,
		/// even if the CancellationToken is set, this will return <b>true</b> even if the CancellationToken is already set.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeout">The time to wait for the task to complete.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWait(this Task source, TimeSpan timeout, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			return TryWait(source, (int)timeout.TotalMilliseconds, token);
		}

        /// <summary>
        /// Wait on a task, this method is an extension method added since the .NET 4.0 TPL doesn't have a 
        /// task.Wait(TimeSpan, CancellationToken).
        /// </summary>
        /// <param name="source">The source task, must be non-null.</param>
        /// <param name="timeout">The time to wait for the task to complete.</param>
        /// <param name="token">A cancellation token to cancel the wait.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool Wait(this Task source, TimeSpan timeout, CancellationToken token)
        {
            return source.Wait((int)timeout.TotalMilliseconds, token);
        }

        /// <summary>
		/// <para>
		/// Attempts to wait on a task.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this behaves as the underlying Wait() does, which is to return <b>true</b> automatically.  This is in contrast to 
		/// WaitAll() and WaitAny() that throw OperationCancelledException.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of Wait() returning <b>true</b> if the task is already run to completion,
		/// even if the CancellationToken is set, this will return <b>true</b> even if the CancellationToken is already set.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeoutInMs">The time to wait for the task to complete.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
		/// <param name="shouldMarkExceptionsHandled">True if should mark all exceptions as having been handled.</param>
		/// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWait(this Task source, int timeoutInMs, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return source.Wait(timeoutInMs, token);
			}

			catch (OperationCanceledException)
			{
				// if the token was cancelled, we didn't wait till task ended, so return false.
				return false;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAll(this Task[] source, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				Task.WaitAll(source);
				return true;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// <para>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this returns <b>false</b> because the underlying WaitAll() call throws OperationCancelledException.  This is in contrast to 
		/// Wait() that returns true immediately even if the CancellationToken is set.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of WaitAll() throwing OperationCancelledException if the token is already cancelled even if
		/// all of the underlying tasks are completed, this method will return <b>false</b> if the token is already cancelled.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAll(this Task[] source, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				Task.WaitAll(source, token);
				return true;
			}

			catch (OperationCanceledException)
			{
				// if the token was cancelled, we didn't wait till task ended, so return false.
				return false;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeout">The time to wait for the task.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAll(this Task[] source, TimeSpan timeout, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAll(source, timeout);
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeoutInMs">The time to wait for the task.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAll(this Task[] source, int timeoutInMs, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAll(source, timeoutInMs);
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// <para>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this returns <b>false</b> because the underlying WaitAll() call throws OperationCancelledException.  This is in contrast to 
		/// Wait() that returns true immediately even if the CancellationToken is set.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of WaitAll() throwing OperationCancelledException if the token is already cancelled even if
		/// all of the underlying tasks are completed, this method will return <b>false</b> if the token is already cancelled.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeout">The time to wait for the task to complete.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAll(this Task[] source, TimeSpan timeout, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			return TryWaitAll(source, (int)timeout.TotalMilliseconds, token, shouldMarkExceptionsHandled);
		}

        /// <summary>
		/// <para>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this returns <b>false</b> because the underlying WaitAll() call throws OperationCancelledException.  This is in contrast to 
		/// Wait() that returns true immediately even if the CancellationToken is set.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of WaitAll() throwing OperationCancelledException if the token is already cancelled even if
		/// all of the underlying tasks are completed, this method will return <b>false</b> if the token is already cancelled.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeoutInMs">The time to wait for the task to complete.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAll(this Task[] source, int timeoutInMs, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAll(source, timeoutInMs, token);
			}

			catch (OperationCanceledException)
			{
				// if the token was cancelled, we didn't wait till task ended, so return false.
				return false;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAny(this Task[] source, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAny(source) != -1;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// <para>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this returns <b>false</b> because the underlying WaitAny() call throws OperationCancelledException.  This is in contrast to 
		/// Wait() that returns true immediately even if the CancellationToken is set.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of WaitAny() throwing OperationCancelledException if the token is already cancelled even if
		/// all of the underlying tasks are completed, this method will return <b>false</b> if the token is already cancelled.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAny(this Task[] source, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAny(source, token) != -1;
			}

			catch (OperationCanceledException)
			{
				// if the token was cancelled, we didn't wait till task ended, so return false.
				return false;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeout">The time to wait for the task.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAny(this Task[] source, TimeSpan timeout, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAny(source, timeout) != -1;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </summary>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeoutInMs">The time to wait for the task.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAny(this Task[] source, int timeoutInMs, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAny(source, timeoutInMs) != -1;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}

		/// <summary>
		/// <para>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this returns <b>false</b> because the underlying WaitAny() call throws OperationCancelledException.  This is in contrast to 
		/// Wait() that returns true immediately even if the CancellationToken is set.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of WaitAny() throwing OperationCancelledException if the token is already cancelled even if
		/// all of the underlying tasks are completed, this method will return <b>false</b> if the token is already cancelled.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeout">The time to wait for the task to complete.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAny(this Task[] source, TimeSpan timeout, CancellationToken token, 
            bool shouldMarkExceptionsHandled = true)
		{
			return TryWaitAny(source, (int)timeout.TotalMilliseconds, token);
		}

		/// <summary>
		/// <para>
		/// Attempts to wait on an array of tasks.  If the task threw an exception or is in a cancelled or disposed state, 
		/// the exception is consumed and discarded.  This will return <b>true</b> if the task ended within the
		/// timeout period (normally or by exception).
		/// </para>
		/// <para>
		/// <b>Note:</b> If the task is already Completed, Faulted, or Cancelled and CancellationToken is Cancelled
		/// that this returns <b>false</b> because the underlying WaitAny() call throws OperationCancelledException.  This is in contrast to 
		/// Wait() that returns true immediately even if the CancellationToken is set.  The decision was made to keep the behavior here
		/// consistent with the underlying calls instead of normalizing them.
		/// </para>
		/// </summary>
		/// <remarks>
		/// Note: Due to the underlying nature of WaitAny() throwing OperationCancelledException if the token is already cancelled even if
		/// all of the underlying tasks are completed, this method will return <b>false</b> if the token is already cancelled.
		/// </remarks>
		/// <param name="source">The source task, must be non-null.</param>
		/// <param name="timeoutInMs">The time to wait for the task to complete.</param>
		/// <param name="token">A cancellation token to cancel the wait.</param>
        /// <param name="shouldMarkExceptionsHandled">True if should mark exceptions as handled so won't re-throw.</param>
        /// <returns>True if the task has ended by any means within the timeout.</returns>
        public static bool TryWaitAny(this Task[] source, int timeoutInMs, CancellationToken token, bool shouldMarkExceptionsHandled = true)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			try
			{
				return Task.WaitAny(source, timeoutInMs, token) != -1;
			}

			catch (OperationCanceledException)
			{
				// if the token was cancelled, we didn't wait till task ended, so return false.
				return false;
			}

            catch (AggregateException aggregateEx)
            {
                // mark all exceptions as handled
                if (shouldMarkExceptionsHandled)
                {
                    aggregateEx.Flatten().Handle(ex => true);
                }

                return true;
            }
		}
	}
}
