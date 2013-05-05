using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;


namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// The Bucket class represents a fixed size queue of objects with full read/write protection.
	/// </summary>
	/// <typeparam name="T">The type of items to store in the bucket.</typeparam>
	/// <remarks>Obsolete as warning in 2.0, will be removed in 3.0.</remarks>
	[Obsolete("This class is no longer supported, SharedAssemblies.General.Threading.Collection<T> now has internal bucket.", false)]
	public class Bucket<T> : IBucket<T>
	{
		/// <summary>
		/// Constant for wait time that specifies an infinite wait period
		/// </summary>
		public static readonly int InfiniteWait = -1;

		/// <summary>
		/// The default initial capacity of the bucket queue
		/// </summary>
		public static readonly int DefaultCapacity = 1000;

		/// <summary>
		/// The default maximum number of items to pull on a call to Get()
		/// </summary>
		public static readonly int DefaultMaxItemsPerGet = 100;


		/// <summary>timeout to wait for an item to get</summary>
		private int _timeoutInMsOnGet = 1000;

		/// <summary>number of items to get in one pull</summary>
		private readonly int _getChunkSize = 100;

		/// <summary>queue that holds the items</summary>
		private readonly Queue<T> _queue;

		/// <summary>mutext lock on the queue</summary>
		private readonly object _queueLock = new object();


		/// <summary>
		/// Get/set the timeout in ms for how long to wait for items to get (-1 = infinite).
		/// </summary>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.NamingAnalyzer",
			"ST3010:PropertyMayNotStartWithGetOrSet",
			Justification = "In this case, GetTimeoutInMs refers to the timeout on a get request.")]
		public int GetTimeoutInMs
		{
			get { return _timeoutInMsOnGet; }
			set { _timeoutInMsOnGet = Math.Max(-1, value); }
		}


		/// <summary>
		/// Returns the current # of items in the bucket	
		/// </summary>
		/// <returns>Number of items in bucket</returns>
		public int Depth
		{
			get
			{
				lock (_queueLock)
				{
					return _queue.Count;
				}
			}
		}


		/// <summary>
		/// Number of items at most to pull on Get, must be at least 1.
		/// </summary>
		public int MaxItemsPerGet
		{
			get { return _getChunkSize; }
		}


		/// <summary>
		/// Returns true if bucket has zero items
		/// </summary>
		/// <returns>True if bucket empty</returns>
		public bool IsEmpty
		{
			get
			{
				lock (_queueLock)
				{
					return (_queue.Count == 0);
				}
			}
		}


		/// <summary>
		/// Constructor to create a new bucket with a default size of
		/// Bucket.DefaultCapacity and a default maximum get size of
		/// Bucket.DefaultMaxItemsPerGet.
		/// </summary>
		public Bucket()
			: this(DefaultCapacity, DefaultMaxItemsPerGet)
		{
		}


		/// <summary>
		/// Constructor to create a new bucket of a given size and a default 
		/// maximum get size of Bucket.DefaultMaxItemsPerGet.
		/// </summary>
		/// <param name="initialCapacity">the capacity of the bucket</param>
		public Bucket(int initialCapacity)
			: this(initialCapacity, DefaultMaxItemsPerGet)
		{
		}


		/// <summary>
		/// Constructor to create a new bucket of a given size
		/// </summary>
		/// <param name="initialCapacity">the capacity of the bucket</param>
		/// <param name="maxItemsPerGet">Maximum number of items to pull on a Get()</param>
		public Bucket(int initialCapacity, int maxItemsPerGet)
		{
			// set queue to initial capacity
			_queue = new Queue<T>(initialCapacity);
			_getChunkSize = Math.Max(1, maxItemsPerGet);
		}


		/// <summary>
		/// Restarts processing on the bucket and clears all contents in the 
		/// bucket.
		/// </summary>
		public void Clear()
		{
			lock (_queueLock)
			{
				_queue.Clear();
			}
		}


		/// <summary>
		/// Removes one item from the bucket and returns it as a ref param
		/// </summary>
		/// <param name="item">The item removed from the bucket</param>
		/// <returns>True if successful</returns>
		public bool Get(out T item)
		{
			// set default return values
			bool result = false;
			item = default(T);

			lock (_queueLock)
			{
				// wait for a pulse or timeout if count is empty
				if (_queue.Count == 0)
				{
					Monitor.Wait(_queueLock, _timeoutInMsOnGet);
				}

				// need to recheck because state may have changed while waiting for pulse
				int currentCount = _queue.Count;
				if (currentCount > 0)
				{
					item = _queue.Dequeue();
					result = true;

					if(currentCount > 1)
					{
						Monitor.Pulse(_queueLock);
					}
				}
			}

			return result;
		}


		/// <summary>
		/// Removes all items from the bucket and returns them as an array
		/// </summary>
		/// <param name="items">The items removed from the bucket</param>
		/// <returns>True if successful</returns>
		public bool Get(out List<T> items)
		{
			// set default return values
			bool isSuccessful = false;
			items = null;

			// wait for an item in the queue
			lock (_queueLock)
			{
				if (_queue.Count == 0)
				{
					Monitor.Wait(_queueLock, _timeoutInMsOnGet);
				}

				// get the smaller of the count and the max to get
				int numToGet = Math.Min(_getChunkSize, _queue.Count);

				if (numToGet > 0)
				{
					// allocate the array list and dequeue the items
					items = new List<T>(numToGet);
					for (int i = 0; i < numToGet; ++i)
					{
						items.Add(_queue.Dequeue());
					}
					isSuccessful = true;
				}
			}

			return isSuccessful;
		}


		/// <summary>
		/// Adds one item to the bucket
		/// </summary>
		/// <param name="item">Item to add</param>
		/// <returns>True if successful</returns>
		public bool Add(T item)
		{
			lock (_queueLock)
			{
				_queue.Enqueue(item);

				// tell any in Wait() they can go
				Monitor.Pulse(_queueLock);
			}


			return true;
		}
	}
}
