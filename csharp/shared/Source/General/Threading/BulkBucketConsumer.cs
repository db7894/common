using System;
using System.Collections.Generic;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
    /// <summary>
    /// The most efficient of the Bucket Consumers, this consumer will consume items 
    /// in bulk up to a bulk size of IBucket.MaxItemsOnGet and then hand them over
    /// in bulk for processing, good if you want to do aggregation of data instead
    /// of handling individual items.  Since you control the consumption, there is no
    /// risk of items being lost if Stop() is called mid-processing.
    /// </summary>
    /// <typeparam name="T">The type of items in the bucket to consume.</typeparam>
	/// <remarks>Obsolete as warning in 2.0, will be removed in 3.0.</remarks>
	[Obsolete("Use SharedAssemblies.General.Threading.BulkConsumer instead.", false)]
	public class BulkBucketConsumer<T> : AbstractBucketConsumer<T>
    {
        /// <summary>
        /// Function for consuming an item in the bucket.  If you wish to halt consumption
        /// from within the consumeAction, you can throw.
        /// </summary>
        private readonly Action<List<T>> _consumeAction;


        /// <summary>
        /// Default constructor which makes rogue consumer which points to
        /// no bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        public BulkBucketConsumer(Action<List<T>> consumeAction)
            : this(consumeAction, null, ThreadPriority.Normal)
        {
        }


        /// <summary>
        /// Default constructor which makes rogue consumer which points to
        /// no bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        /// <param name="priority">The priority at which the consuming thread should run.</param>
        public BulkBucketConsumer(Action<List<T>> consumeAction, ThreadPriority priority)
            : this(consumeAction, null, priority)
        {
        }


        /// <summary>
        /// Constructor which creates a consumer pointed to the given bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        /// <param name="sourceBucket">The bucket to consume items from.</param>
        public BulkBucketConsumer(Action<List<T>> consumeAction, IBucket<T> sourceBucket)
            : this(consumeAction, sourceBucket, ThreadPriority.Normal)
        {
        }


        /// <summary>
        /// Constructor which creates a consumer pointed to the given bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        /// <param name="sourceBucket">The bucket to consume items from.</param>
        /// <param name="priority">The priority at which the consuming thread should run.</param>
        public BulkBucketConsumer(Action<List<T>> consumeAction, IBucket<T> sourceBucket, 
                                  ThreadPriority priority)
            : base(sourceBucket, priority)
        {
            if (consumeAction != null)
            {
                _consumeAction = consumeAction;
            }
            else
            {
                throw new ArgumentNullException("consumeAction",
                                                "You must specify a consume action.");
            }
        }


        /// <summary>
        /// This private method is the consumer's thread function.
        /// </summary>
        protected sealed override void ConsumeThreadFunction()
        {
            IsConsuming = true;

            try
            {
                // as long as item remove okay, process it
                while (IsConsuming)
                {
                    List<T> items;

                    if (Bucket.Get(out items))
                    {
                        _consumeAction(items);
                    }
                }
            }
            finally
            {
                IsConsuming = false;

                // hook to perform custom finishing code
                if (OnConsumerStopped != null)
                {
                    OnConsumerStopped();
                }
            }
        }
    }
}