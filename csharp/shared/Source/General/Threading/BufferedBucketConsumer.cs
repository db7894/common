using System;
using System.Collections.Generic;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
    /// <summary>
    /// This is a more efficient bucket consumer that grabs items of the bucket in a bulk
    /// (which is specified by the MaxItemsOnGet property of IBucket), and then loops through
    /// them in memory for processing.  This is more efficient as it does not hit critical 
    /// section contention as much, however it does lose unprocessed items if Stop() is called
    /// while processing a chunk.
    /// </summary>
    /// <typeparam name="T">The type of items in the bucket to consume.</typeparam>
	/// <remarks>Obsolete as warning in 2.0, will be removed in 3.0.</remarks>
	[Obsolete("Use SharedAssemblies.General.Threading.Consumer instead.", false)]
	public class BufferedBucketConsumer<T> : AbstractBucketConsumer<T>
    {
        /// <summary>
        /// Function for consuming an item in the bucket.  If you wish to halt consumption
        /// from within the consumeAction, you can throw.
        /// </summary>
        private readonly Action<T> _consumeOneAction;        

        
        /// <summary>
        /// Default constructor which makes rogue consumer which points to
        /// no bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        public BufferedBucketConsumer(Action<T> consumeAction)
            : this(consumeAction, null, ThreadPriority.Normal)
        {
        }


        /// <summary>
        /// Default constructor which makes rogue consumer which points to
        /// no bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        /// <param name="priority">The priority at which the consuming thread should run.</param>
        public BufferedBucketConsumer(Action<T> consumeAction, ThreadPriority priority)
            : this(consumeAction, null, priority)
        {
        }


        /// <summary>
        /// Constructor which creates a consumer pointed to the given bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        /// <param name="sourceBucket">The bucket to consume items from.</param>
        public BufferedBucketConsumer(Action<T> consumeAction, IBucket<T> sourceBucket)
            : this(consumeAction, sourceBucket, ThreadPriority.Normal)
        {
        }


        /// <summary>
        /// Constructor which creates a consumer pointed to the given bucket
        /// </summary>
        /// <param name="consumeAction">The action to perform on each item.</param>
        /// <param name="sourceBucket">The bucket to consume items from.</param>
        /// <param name="priority">The priority at which the consuming thread should run.</param>
        public BufferedBucketConsumer(Action<T> consumeAction, IBucket<T> sourceBucket, 
                                      ThreadPriority priority) :
            base(sourceBucket, priority)
        {
            if (consumeAction != null)
            {
                _consumeOneAction = consumeAction;
            }
            else
            {
                throw new ArgumentNullException("consumeAction",
                                                "You must specify a consume action.");
            }
        }


        /// <summary>
        /// The thread function that will handle consumption of items from thread
        /// </summary>
        protected override void ConsumeThreadFunction()
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
                        ConsumeItems(items);
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

        
        /// <summary>
        /// Abstract method for consuming an item in the bucket.  Your class
        /// will override this method to provide the consumer functionality.
        /// </summary>
        /// <param name="items">The array of items to be consumed</param>
        protected void ConsumeItems(List<T> items)
        {
            // if consumeItem returns FALSE, will break cycle
            if (IsConsuming && items != null)
            {
                foreach (var item in items)
                {
                    // if consumeItem returns FALSE, will break cycle
                    if (IsConsuming)
                    {
                        _consumeOneAction(item);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}