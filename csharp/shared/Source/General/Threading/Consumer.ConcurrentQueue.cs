using System;
using System.Collections.Concurrent;


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
	/// This version uses a ConcurrentQueue as the IProducerConsumerCollection backing store.
	/// </para>
	/// </summary>
	/// <typeparam name="T">The type of items in the collection to consume.</typeparam>
	public class Consumer<T> : Consumer<T, ConcurrentQueue<T>>
	{
		/// <summary>
		/// Constructor which creates a consumer pointed to the given collection
		/// </summary>
		/// <param name="consumeAction">The action to perform on each item.</param>
		/// <param name="consumerCount">The number of consumer threads to process data.</param>
		/// <param name="partitionerProjection">A project that provides a field to decide which collection to place data in.</param>
		public Consumer(Action<T> consumeAction, int consumerCount = 1, Func<T, object> partitionerProjection = null)
			: base(consumeAction, consumerCount, partitionerProjection)
		{
		}
	}
}