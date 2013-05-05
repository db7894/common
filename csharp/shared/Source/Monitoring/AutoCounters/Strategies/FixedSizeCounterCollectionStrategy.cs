using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;

namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Auto counter collection strategy that enforces the collection is of a fixed
	/// size to maximize performance for the most common use cases (collection created once, but
	/// read many times).  This strategy is used in conjunction with the non-locking
	/// version of the auto counters <see cref="NonLockingAutoCountersAttribute"/>.
	/// <para>
	/// The primary external difference between this strategy and the 
	/// <see cref="CounterCollectionStrategy"/> is that the <see cref="Add"/> method
	/// will always throw an exception.
	/// </para>
	/// </summary>
	internal class FixedSizeCounterCollectionStrategy : ICounterCollectionStrategy
	{
		/// <summary>The array of counters that are part of the collection</summary>
        private readonly AutoCounter[] _counters;

        /// <summary>The parent collection of this collection</summary>
        private readonly AutoCounterCollection _parentCollection;

		/// <summary>
		/// Whether or not one or more timers in the collection require a 
		/// start timer.
		/// </summary>
		private readonly bool _requiresStartTimer;

        /// <summary>
        /// Gets the parent collection to be used in blocks
        /// </summary>
        public AutoCounterCollection ParentCollection
        {
            get { return _parentCollection; }
        }
        
        /// <summary>
        /// Initialize a fixed size collection of auto counters.
        /// </summary>
        /// <param name="parentCollection">The parent collection for this collection</param>
		/// <param name="counters">Initial set of counters to include</param>
		public FixedSizeCounterCollectionStrategy(AutoCounterCollection parentCollection, 
														IEnumerable<AutoCounter> counters)
        {
            _parentCollection = parentCollection;
        	_counters = counters.ToArray();
        	_requiresStartTimer = _counters.Any(c => c.RequiresStartTimer);
        }

		/// <summary>
		/// Whether or not the counter owned by this strategy must be flushed
		/// using the <see cref="FlushCounter"/> method.
		/// </summary>
		// Counters in the collection are already handled by the background task.
		public bool RequiresFlush { get { return false; } }	

		/// <summary>
		/// Flushes any cached data in the counter down to the PerformanceCounter objects
		/// owned by this strategy.
		/// </summary>
		public void FlushCounter()
		{
			// do nothing.
		}

		/// <summary>
		/// Enter a block of code to be instrumented.
		/// </summary>
		public void StartBlock()
		{
			// do nothing.
		}

		/// <summary>
		/// Leave a block of code to be instrumented.
		/// <note>Deprecated, use <see cref="EndBlock(InstrumentationBlock)"/></note>
		/// </summary>
        public void EndBlock()
        {
			EndBlock(default(InstrumentationBlock));
        }

		/// <summary>
		/// Leave a block of code to be instrumented.  
		/// </summary>
		/// <param name="block">Internal instrumentation data returned from <see cref="GetBlock"/></param>
		public void EndBlock(InstrumentationBlock block)
		{
			// Avoid iterators, use an integer as the array size will not change. This
			// voids a temporary object and gives the runtime the best chance for unrolling
			// these loops.
			for(int i = 0; i < _counters.Length; ++i)
			{
				_counters[i].EndBlock(block);
			}

			if (_parentCollection != null)
			{
				_parentCollection.EndBlock(block);
			}	
		}

        /// <summary>
        /// Returns a IDisposable InstrumentationBlock to automatically
        /// call Enter and Leave correctly to avoid screw-ups.
        /// </summary>
        /// <returns>An instrumentation block for the counter collection.</returns>
        public InstrumentationBlock GetBlock()
        {
            return new InstrumentationBlock(this, _requiresStartTimer);
        }

        /// <summary>
        /// Add an AutoCounter to the collection
        /// </summary>
        /// <param name="counter">The counter to add to the managed collection.</param>
        public void Add(AutoCounter counter)
        {
			throw new AutoCounterException(
						"Cannot add a new counter to a FixedSizeCounterCollectionStrategy");
        }

        /// <summary>
        /// Clean up any system resources held by this collection not including
        /// the parent collection, it must reset itself
        /// </summary>
        public void ResetAll()
        {
			foreach(var counter in _counters)
            {
                counter.Reset();
            }
        }

        /// <summary>
        /// Clean up any system resources in this collection, does not dispose of
        /// parent collection, that must dispose of itself
        /// </summary>
        public void Dispose()
        {
			foreach(var counter in _counters)
            {
                counter.Dispose();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An IEnumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<AutoCounter> GetEnumerator()
        {
        	return _counters.Cast<AutoCounter>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
	}
}
