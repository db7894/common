using System;
using System.Collections.Concurrent;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// <para>
	/// This is the simplest of the collection consumers, it takes one item at a
	/// time from the collection and consumes it before getting another.  
	/// </para>
	/// <para>
	/// In the earlier Bucket implementations, the consumers and buckets were separate, in the new implementation,
	/// the buckets are self contained and the consumer is the only thing visible.
	/// </para>
	/// <para>
	/// This is a more generic version of BulkConsumer that lets you specify any IProducerConsumerCollection
	/// implementation as the backing store for the BlockingCollection.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The type of items in the collection to consume.</typeparam>
	/// <typeparam name="TCollection">The type of the underlying IProducerConsumer collection.</typeparam>
	public class Consumer<T, TCollection> : AbstractConsumer<T, TCollection>
		where TCollection : IProducerConsumerCollection<T>, new()
	{
		/// <summary>
		/// Function for consuming an item in the collection.  If you wish to halt consumption
		/// from within the consumeAction, you can throw.
		/// </summary>
		public Action<T> ConsumeAction { get; private set; }

		/// <summary>
		/// Constructor which creates a consumer pointed to the given collection
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>
		public Consumer(Action<T> consumeAction, int consumerCount = 1, Func<T, object> partitionerProjection = null)
			: base(consumerCount, partitionerProjection)
		{
			if (consumeAction == null)
			{
				throw new ArgumentNullException("consumeAction");
			}

			ConsumeAction = consumeAction;
		}

		/// <summary>
		/// Factory method which creates a consumer pointed to the given collection
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>		
		/// <returns>A new, un-started instance of a consumer.</returns>
		public static IConsumer<T> Create(Action<T> consumeAction, int consumerCount = 1,
			Func<T, object> partitionerProjection = null)
		{
			return new Consumer<T, TCollection>(consumeAction, consumerCount, partitionerProjection);
		}

		/// <summary>
		/// Factory method which creates a consumer pointed to the given collection
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>		
		/// <returns>A new, un-started instance of a consumer.</returns>
		public static IConsumer<T> StartNew(Action<T> consumeAction, int consumerCount = 1,
			Func<T, object> partitionerProjection = null)
		{
			var consumer = Create(consumeAction, consumerCount, partitionerProjection);

			consumer.Start();

			return consumer;
		}

		/// <summary>
		/// This private method is the consumer's thread function.
		/// </summary>
		/// <param name="collection">The collection to consume from.</param>
		/// <param name="token">The cancellation token to listen to.</param>
		protected sealed override void ConsumeUntilCancelled(BlockingCollection<T> collection, CancellationToken token)
		{
			// as long as not cancelled (due to Stop()) and adding is not completed (due to AddingCompleted())
			while (!token.IsCancellationRequested && !collection.IsCompleted)
			{
				T item;

				// perform consume action until timeout occurs or cancellation token cancel is set.
				if (collection.TryTake(out item, (int)TimeoutOnConsume.TotalMilliseconds, token))
				{
					ConsumeAction(item);
				}
			}
		}
	}
}