using System;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// This is the simplest of the bucket consumers, it takes one item at a
	/// time from the bucket and consumes it before getting another.  It is not
	/// as efficient as bulk and buffered bucket consumers, but it will never
	/// lose items in the bucket if stop() is called mid-process.
	/// </summary>
	/// <typeparam name="T">The type of items in the bucket to consume.</typeparam>
	/// <remarks>Obsolete as warning in 2.0, will be removed in 3.0.</remarks>
	[Obsolete("Use SharedAssemblies.General.Threading.Consumer instead.", false)]
	public class DiscreteBucketConsumer<T> : AbstractBucketConsumer<T>
	{
		/// <summary>
		/// Function for consuming an item in the bucket.  If you wish to halt consumption
		/// from within the consumeAction, you can throw.
		/// </summary>
		private readonly Action<T> _consumeAction;


		/// <summary>
		/// Default constructor which makes rogue consumer which points to
		/// no bucket
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		public DiscreteBucketConsumer(Action<T> consumeAction)
			: this(consumeAction, null, ThreadPriority.Normal)
		{
		}


		/// <summary>
		/// Default constructor which makes rogue consumer which points to
		/// no bucket
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="priority">The priority at which the consuming thread should run.</param>
		public DiscreteBucketConsumer(Action<T> consumeAction, ThreadPriority priority)
			: this(consumeAction, null, priority)
		{
		}


		/// <summary>
		/// Constructor which creates a consumer pointed to the given bucket
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="sourceBucket">The bucket to consume items from.</param>
		public DiscreteBucketConsumer(Action<T> consumeAction, IBucket<T> sourceBucket)
			: this(consumeAction, sourceBucket, ThreadPriority.Normal)
		{
		}


		/// <summary>
		/// Constructor which creates a consumer pointed to the given bucket
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="sourceBucket">The bucket to consume items from.</param>
		/// <param name="priority">The priority at which the consuming thread should run.</param>
		public DiscreteBucketConsumer(Action<T> consumeAction, IBucket<T> sourceBucket, 
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
					T item;

					if (Bucket.Get(out item))
					{
						_consumeAction(item);
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