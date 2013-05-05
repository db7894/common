using System;
using System.Threading;
using SharedAssemblies.General.Threading.Extensions;


namespace SharedAssemblies.General.Threading
{
    /// <summary>
    /// This is the base bucket consumer behavior
    /// </summary>
    /// <typeparam name="T">The type of items in the bucket to consume.</typeparam>
	/// <remarks>Obsolete as warning in 2.0, will be removed in 3.0.</remarks>
	[Obsolete("Use SharedAssemblies.General.Threading.AbstractConsumer instead.", false)]
	public abstract class AbstractBucketConsumer<T> : 
        IBucketConsumer<T>,
        IDisposable
    {
        /// <summary>True if currently consuming items.</summary>
        private volatile bool _isConsuming;                     

        /// <summary>Thread for asynchronous consumption.</summary>
        private Thread _consumeThread;                          

        /// <summary>Object to synchronize on.</summary>
        private readonly object _consumerLock = new object();


		/// <summary>
		/// Delegate you can provide that allows you to specify an action to be performed
		/// on start of the consumption thread.  If you wish to abort consuming, throw exception.
		/// </summary>
		public Action OnConsumerStarted { get; set; }


		/// <summary>
		/// Delegate you can provide that allows you to specify an action to be performed
		/// when the consumption is stopped
		/// </summary>
		public Action OnConsumerStopped { get; set; }


		/// <summary>
		/// Returns true if there is a bucket attached to the consumer
		/// </summary>
		/// <returns>True if attached to a bucket</returns>
		public bool IsAttached
		{
			get { return (Bucket != null); }
		}


		/// <summary>
		/// Returns true if currently in a consuming mode.  
		/// </summary>
		/// <returns>True if consuming</returns>
		public bool IsConsuming
		{
			// did not make this an auto-property because i need it to be volatile.
			get { return _isConsuming; }
			protected set { _isConsuming = value; }
		}


		/// <summary>
		/// Returns the thread priority of the consumer thread
		/// </summary>
		public ThreadPriority ConsumerThreadPriority { get; private set; }


		/// <summary>
		/// Returns the current bucket consumer is attached to
		/// </summary>
		/// <returns></returns>
		public IBucket<T> Bucket { get; private set; }


		/// <summary>
		/// Gets/sets the number of milliseconds to wait for a join on Stop() and Dispose()
		/// Note: the default wait is the default int of zero which means NO wait, not infinite
		/// </summary>
		public int TimeoutInMsToWaitOnStop { get; set; }


		/// <summary>
        /// Constructor which creates a consumer pointed to the given bucket
        /// </summary>
        /// <param name="sourceBucket">The bucket to consume from.</param>
        /// <param name="priority">The thread priority for the consuming thread to run at.</param>
        protected AbstractBucketConsumer(IBucket<T> sourceBucket, ThreadPriority priority)
        {
            ConsumerThreadPriority = priority;
            Bucket = sourceBucket;
        }


        /// <summary>
        /// Begins the consuming process, cannot be called if already consuming
        /// </summary>
        /// <returns>True if start is successful</returns>
        public bool Start()
        {
            lock (_consumerLock)
            {
                if (!IsConsuming)
                {
                    // hook to perform custom start code, if an exception is thrown the thread
                    // is not started and the exception cascades up to caller.
                    if (OnConsumerStarted != null)
                    {
                        OnConsumerStarted();
                    }

                    // create and start new thread
                    _consumeThread = new Thread(ConsumeThreadFunction)
                                         {
                                             Priority = ConsumerThreadPriority,
                                             IsBackground = true
                                         };

                    _consumeThread.Start();

                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Dispose the instance by stopping the thread
        /// </summary>
        public void Dispose()
        {
            Stop();
        }


        /// <summary>
        /// Stops given the default timeout in ms to wait for this instance.  If not altered,
        /// this is a timeout of zero ms which effectively means it will not wait for a join.
        /// </summary>
        /// <returns>True if consumer stopped (joined) within the timeout</returns>
        public bool Stop()
        {
            return Stop(TimeoutInMsToWaitOnStop);
        }


        /// <summary>
        /// Stops the consumer given a specific timeout in ms to wait for the thread to join.
        /// </summary>
        /// <param name="timeoutToWaitInMs">Number of ms to wait for the join of the thread</param>
        /// <returns>True if stop is successful within timeout</returns>
        public bool Stop(int timeoutToWaitInMs)
        {
            bool hasJoined = true;

            lock (_consumerLock)
            {
                if (IsConsuming)
                {
                    IsConsuming = false;
                    hasJoined = _consumeThread.JoinOrAbort(timeoutToWaitInMs);
                }
            }

            return hasJoined;
        }


        /// <summary>
        /// Method to change the bucket a consumer is using, cannot be done
        /// while already in consuming mode.
        /// </summary>
        /// <param name="sourceBucket">New bucket to consume from</param>
        /// <returns>True if new bucket is attached</returns>
        public bool AttachBucket(IBucket<T> sourceBucket)
        {
            lock (_consumerLock)
            {
                if (!IsConsuming)
                {
                    Bucket = sourceBucket;
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Detaches from current bucket it not currently consuming.
        /// </summary>
        /// <returns>True if detach successful</returns>
        public bool DetachBucket()
        {
            lock (_consumerLock)
            {
                if (!IsConsuming)
                {
                    Bucket = null;
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// This private method is the consumer's thread function.
        /// </summary>
        protected abstract void ConsumeThreadFunction();
    }
}