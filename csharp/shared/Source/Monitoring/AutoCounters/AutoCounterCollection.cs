using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using SharedAssemblies.Monitoring.AutoCounters.Strategies;

namespace SharedAssemblies.Monitoring.AutoCounters
{
    /// <summary>
    /// Class for an easier-to-use performance counter collection
    /// </summary>
    public class AutoCounterCollection : 
        ICounter, 
        IDisposable,
        IEnumerable<AutoCounter>
    {
		/// <summary>
		/// Strategy that implements the desired collection behavior.
		/// </summary>
    	private readonly ICounterCollectionStrategy _strategy;

        /// <summary>
        /// Gets the parent collection to be used in blocks
        /// </summary>
        public AutoCounterCollection ParentCollection 
		{ 
			get { return _strategy.ParentCollection; }
		}
        
        /// <summary>
        /// Initialize empty list of AutoCounter
        /// </summary>
        public AutoCounterCollection()
			: this(null, new List<AutoCounter>(), false)
        {
        }

        /// <summary>
        /// Initialize empty list of AutoCounter
        /// </summary>
        /// <param name="parentCollection">The parent collection for this collection</param>
        public AutoCounterCollection(AutoCounterCollection parentCollection) 
			:this(parentCollection, new List<AutoCounter>(), false)
        {
        }

        /// <summary>
        /// Initialize a collection that manages a list of AutoCounters.
        /// </summary>
        /// <param name="parentCollection">The parent collection for this collection</param>
		/// <param name="counters">The counters to contain in the collection</param>
		/// <param name="useNonLocking">Whether or not to use the non-locking container</param>
        public AutoCounterCollection(AutoCounterCollection parentCollection, 
										IEnumerable<AutoCounter> counters, bool useNonLocking)
        {
        	_strategy = CounterStrategyFactory.CreateCollection(parentCollection, 
																		counters, useNonLocking);
        }
        
        /// <summary>
        /// Enter a block of code to be instrumented.
        /// </summary>
        public void StartBlock()
        {
        	// None of the strategies actually do anything in a start block, so 
			// there is no longer a need for this method.  The method is only kept so that
			// the interface into the class doesn't change.
        }

		/// <summary>
		/// Leave a block of code to be instrumented.
		/// <note>Deprecated, use <see cref="EndBlock(InstrumentationBlock)"/></note>
		/// </summary>
        public void EndBlock()
        {
			_strategy.EndBlock(default(InstrumentationBlock));
        }

		/// <summary>
		/// Leave a block of code to be instrumented.  
		/// </summary>
		/// <param name="block">Internal instrumentation data returned from <see cref="GetBlock"/></param>
		public void EndBlock(InstrumentationBlock block)
		{
			_strategy.EndBlock(block);			
		}

        /// <summary>
        /// Returns a IDisposable InstrumentationBlock to automatically
        /// call Enter and Leave correctly to avoid screw-ups.
        /// </summary>
        /// <returns>An instrumentation block for the counter collection.</returns>
        public InstrumentationBlock GetBlock()
        {
        	return _strategy.GetBlock();
        }

        /// <summary>
        /// Add an AutoCounter to the collection
        /// </summary>
        /// <param name="counter">The counter to add to the managed collection.</param>
        public void Add(AutoCounter counter)
        {
           _strategy.Add(counter);
        }

        /// <summary>
        /// Clean up any system resources held by this collection not including
        /// the parent collection, it must reset itself
        /// </summary>
        public void ResetAll()
        {
			_strategy.ResetAll();
        }

        /// <summary>
        /// Clean up any system resources in this collection, does not dispose of
        /// parent collection, that must dispose of itself
        /// </summary>
		/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
		[Obsolete("Do not explicitly dispose collections, this disposes the underlying counters.", true)]
        public void Dispose()
        {
        	_strategy.Dispose();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An IEnumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<AutoCounter> GetEnumerator()
        {
			return _strategy.GetEnumerator();
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
			return _strategy.GetEnumerator();
        }
    }
}