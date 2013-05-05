using System;
using System.Collections.Generic;

namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
	/// <summary>
	/// Strategy for a collection of auto counters.
	/// </summary>
	internal interface ICounterCollectionStrategy : 
		ICounter,
		IDisposable,
		IEnumerable<AutoCounter>
	{
		/// <summary>
		/// Gets the parent collection to be used in blocks
		/// </summary>
		AutoCounterCollection ParentCollection { get; }

		/// <summary>
		/// Add an AutoCounter to the collection
		/// </summary>
		/// <param name="counter">The counter to add to the managed collection.</param>
		void Add(AutoCounter counter);

		/// <summary>
		/// Clean up any system resources held by this collection not including
		/// the parent collection, it must reset itself
		/// </summary>
		void ResetAll();
	}
}
