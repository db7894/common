using System;

namespace SharedAssemblies.General.Threading
{
    /// <summary>
    /// Interface for the bucket consumer hierarchy of classes
    /// </summary>
    /// <typeparam name="T">Type of items in bucket from which to consume</typeparam>
	/// <remarks>Obsolete as warning in 2.0, will be removed in 3.0.</remarks>
	[Obsolete("Use SharedAssemblies.General.Threading.ICollectionConsumer instead.", false)]
	public interface IBucketConsumer<T> : IConsumer
    {
		/// <summary>
		/// Property that returns the bucket currently attached
		/// </summary>
		IBucket<T> Bucket { get; }
		
		
		/// <summary>
        /// Method to change the bucket a consumer is using, cannot be done
        /// while already in consuming mode.
        /// </summary>
        /// <param name="sourceBucket">New bucket to consume from</param>
        /// <returns>True if new bucket is attached</returns>
        bool AttachBucket(IBucket<T> sourceBucket);


        /// <summary>
        /// Detaches from current bucket it not currently consuming.
        /// </summary>
        /// <returns>True if detach successful</returns>
        bool DetachBucket();
    }
}