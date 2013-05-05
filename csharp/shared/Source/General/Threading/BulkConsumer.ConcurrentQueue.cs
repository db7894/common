using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


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
	/// This version uses a ConcurrentQueue as the IProducerConsumerCollection backing store.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The type of items in the collection to consume.</typeparam>
	public class BulkConsumer<T> : BulkConsumer<T, ConcurrentQueue<T>>
	{
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
			: base(consumeAction, bulkTakeSize, consumerCount, partitionerProjection)
		{
		}
	}
}