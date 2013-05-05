using System;
using System.Collections.Generic;


namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// Generic bucket interface
	/// </summary>
	/// <typeparam name="T">Type of items bucket holds</typeparam>
	/// <remarks>Obsolete as warning in 2.0, will be removed in 3.0.</remarks>
	[Obsolete("Use System.Collections.Concurrent.BlockingCollection<T> instead.", false)]
	public interface IBucket<T>
	{
		/// <summary>
		/// Returns the current # of items in the bucket	
		/// </summary>
		/// <returns>Number of items in bucket</returns>
		int Depth { get; }


		/// <summary>
		/// Returns true if bucket has zero items
		/// </summary>
		/// <returns>True if bucket empty</returns>
		bool IsEmpty { get; }


		/// <summary>
		/// Removes all items in the bucket
		/// </summary>
		void Clear();


		/// <summary>
		/// Removes one item from the bucket and returns it as a ref param
		/// </summary>
		/// <param name="item">The item removed from the bucket</param>
		/// <returns>True if there exists an item to get</returns>
		bool Get(out T item);


		/// <summary>
		/// Removes all items from the bucket and returns it as a ref param
		/// </summary>
		/// <param name="item">The array of items removed from the bucket</param>
		/// <returns>True if there exists an item to get</returns>
		bool Get(out List<T> item);


		/// <summary>
		/// Adds one item to the bucket
		/// </summary>
		/// <param name="item">Item to add</param>
		/// <returns>True if successful</returns>
		bool Add(T item);
	}
}