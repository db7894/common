using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Auto counter collection strategy that maintains the SharedAssemblies v1.7 and
	/// earlier behavior. 
	/// </summary>
	internal class CounterCollectionStrategy : ICounterCollectionStrategy
	{
		/// <summary>The list of counters that are part of the collection</summary>
        private readonly List<AutoCounter> _counters;

        /// <summary>The parent collection of this collection</summary>
        private readonly AutoCounterCollection _parentCollection;

		/// <summary>
		/// True if any of the auto counters in the collection require a start timer.
		/// </summary>
		private bool _requiresStartTimer;

        /// <summary>
        /// Gets the parent collection to be used in blocks
        /// </summary>
        public AutoCounterCollection ParentCollection
        {
            get { return _parentCollection; }
        }
        
        /// <summary>
        /// Initialize empty list of AutoCounter
        /// </summary>
        /// <param name="parentCollection">The parent collection for this collection</param>
		/// <param name="counters">Initial set of counters to include</param>
        public CounterCollectionStrategy(AutoCounterCollection parentCollection, 
														IEnumerable<AutoCounter> counters)
        {
            _parentCollection = parentCollection;
            _counters = new List<AutoCounter>(counters);
        	_requiresStartTimer = _counters.Any(c => c.RequiresStartTimer);
        }


		/// <summary>
		/// Whether or not the counter owned by this strategy must be flushed
		/// using the <see cref="FlushCounter"/> method.
		/// </summary>
		public bool RequiresFlush { get { return false; } }	// counter is always up to date.

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
			foreach (var counter in _counters)
			{
				counter.EndBlock(block);
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
            _counters.Add(counter);
			if(counter.RequiresStartTimer)
			{
				_requiresStartTimer = true;
			}
        }

        /// <summary>
        /// Clean up any system resources held by this collection not including
        /// the parent collection, it must reset itself
        /// </summary>
        public void ResetAll()
        {
            foreach (var counter in _counters)
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
            foreach (var counter in _counters)
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
            return _counters.GetEnumerator();
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
