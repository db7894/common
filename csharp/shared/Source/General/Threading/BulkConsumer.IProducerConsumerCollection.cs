using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// <para>
	/// The most efficient of the Collection Consumers, this consumer will consume items 
	/// in bulk up to a bulk size of BlockingCollection.MaxItemsOnGet and then hand them over
	/// in bulk for processing, good if you want to do aggregation of data instead
	/// of handling individual items.  Since you control the consumption, there is no
	/// risk of items being lost if Stop() is called mid-processing.
	/// </para>
	/// <para>
	/// This is a more generic version of BulkConsumer that lets you specify any IProducerConsumerCollection
	/// implementation as the backing store for the BlockingCollection.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The type of items in the collection to consume.</typeparam>
	/// <typeparam name="TCollection">The type of the underlying IProducerConsumer collection.</typeparam>
	public class BulkConsumer<T, TCollection> : AbstractConsumer<T, TCollection>
		where TCollection : IProducerConsumerCollection<T>, new()
	{
		/// <summary>
		/// By default, the number of items to take at a time from the collection.
		/// </summary>
		public const int DefaultBulkTakeSize = 100;

		/// <summary>
		/// Gets the function for consuming an item in the collection.  If you wish to halt consumption
		/// from within the consumeAction, you can throw.
		/// </summary>
		public Action<IEnumerable<T>> ConsumeAction { get; private set; }

		/// <summary>
		/// Gets the size of items to take at a time.
		/// </summary>
		public int BulkTakeSize { get; private set; }

		/// <summary>
		/// Constructor which creates a consumer pointed to the given collection that grabs items in bulk for processing.  The default 
		/// BulkTakeSize is zero, which indicates it should take all items available in the collection.
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="bulkTakeSize">The bulk take size to use.</param>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>
		public BulkConsumer(Action<IEnumerable<T>> consumeAction, int bulkTakeSize = DefaultBulkTakeSize, 
			int consumerCount = 1, Func<T, object> partitionerProjection = null)
			 : base(consumerCount, partitionerProjection)
		{
			if (consumeAction == null)
			{
				throw new ArgumentNullException("consumeAction");
			}

			if (bulkTakeSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bulkTakeSize", "The bulkTakeSize must be a positive number.");
			}

			ConsumeAction = consumeAction;
			BulkTakeSize = bulkTakeSize;
		}

		/// <summary>
		/// Constructor which creates a consumer pointed to the given collection that grabs items in bulk for processing.  The default 
		/// BulkTakeSize is zero, which indicates it should take all items available in the collection.
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="bulkTakeSize">The bulk take size to use.</param>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>
		/// <returns>A new, un-started instance of a consumer.</returns>
		public static IConsumer<T> Create(Action<IEnumerable<T>> consumeAction,
			int bulkTakeSize = BulkConsumer<T>.DefaultBulkTakeSize, int consumerCount = 1, Func<T, object> partitionerProjection = null)
		{
			return new BulkConsumer<T, TCollection>(consumeAction, bulkTakeSize, consumerCount, partitionerProjection);
		}

		/// <summary>
		/// Constructor which creates a consumer pointed to the given collection that grabs items in bulk for processing.  The default 
		/// BulkTakeSize is zero, which indicates it should take all items available in the collection.
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="bulkTakeSize">The bulk take size to use.</param>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>
		/// <returns>A new, un-started instance of a consumer.</returns>
		public static IConsumer<T> StartNew(Action<IEnumerable<T>> consumeAction,
			int bulkTakeSize = BulkConsumer<T>.DefaultBulkTakeSize, int consumerCount = 1, Func<T, object> partitionerProjection = null)
		{
			var consumer = Create(consumeAction, bulkTakeSize, consumerCount, partitionerProjection);

			consumer.Start();

			return consumer;
		}

		/// <summary>
		/// This private method is the consumer's thread function.
		/// </summary>
		/// <param name="collection">The collection to consume from.</param>
		/// <param name="token">The cancellation token to stop consumption.</param>
		protected sealed override void ConsumeUntilCancelled(BlockingCollection<T> collection, CancellationToken token)
		{
			// this is the max we will pre-size the list
			const int defaultPreSize = 1000;

			// as long as not cancelled (due to Stop()) and adding is not completed (due to AddingCompleted())
			while (!token.IsCancellationRequested && !collection.IsCompleted)
			{
				T item;

				// attempt to grab an item, wait for specified timeout or cancellation
				if (collection.TryTake(out item, (int)TimeoutOnConsume.TotalMilliseconds, token))
				{
					// if we have at least one, create the list and add the item (pre-size to bulk or 1000, whichever smaller
					var items = new List<T>(Math.Min(BulkTakeSize, defaultPreSize)) { item };

					// then get up to BulkTakeSize or until collection empty, whichever comes first...
					while (items.Count < BulkTakeSize && collection.TryTake(out item, 0, token))
					{
						items.Add(item);
					}

					// Have at least one at this point, so call.
					ConsumeAction(items);
				}
			}
		}
	}
}