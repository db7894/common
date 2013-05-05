using System.Collections.Generic;

namespace SharedAssemblies.Monitoring.AutoCounters
{	
	/// <summary>
	/// A cache of AutoCounters and collections.  This cache allows you to examine all the 
	/// auto counters registered and query them for instrumentation blocks.
	/// </summary>
	public interface ICounterCache
	{
		/// <summary>
		/// Gets an IEnumerable of string that contains all the unique names that can be used to key
		/// the AutoCounterCache.
		/// </summary>
		IEnumerable<string> UniqueNames { get; }

		/// <summary>
		/// Gets or sets whether to auto-reset all counters at midnight (server time) to initial values.
		/// </summary>
		bool ShouldAutoResetAllDaily { get; set; }

		/// <summary>
		/// <para>
		/// Gets an instrumentation block from a single-instance counter or collection as 
		/// specified by its unique name.  
		/// </para>
		/// <para>
		/// If the counter or collection specified by the unique name is a multi-instance 
		/// counter or collection, this is an error and will always throw regardless of 
		/// the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// The counter could be an AutoCounter or an AutoCounterCollection, regardless
		/// it will return the result of GetBlock() on that ICounter instance.
		/// </para>
		/// </summary>
		/// <param name="uniqueName">A unique name that identifies the counter.</param>
		/// <returns>An instrumentation block for the counter.</returns>
		InstrumentationBlock this[string uniqueName]
		{
			get;
		}

		/// <summary>
		/// <para>
		/// Gets an instrumentation block from a multi-instance counter or collection as specified 
		/// by its unique name and instance name.  
		/// </para>
		/// <para>
		/// If the counter or collection specified by the unique name is a multi-instance 
		/// counter or collection, this is an error and will always throw regardless of 
		/// the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// The counter could be an AutoCounter or an AutoCounterCollection, regardless
		/// it will return the result of GetBlock() on that ICounter instance.
		/// </para>
		/// </summary>
		/// <param name="uniqueName">A unique name that identifies the counter or collection.</param>
		/// <param name="instanceName">An instance name that identifies the specific instance.</param>
		/// <returns>An instrumentation block for the counter.</returns>
		InstrumentationBlock this[string uniqueName, string instanceName]
		{
			get;
		}

		/// <summary>
		/// <para>
		/// Gets an instrumentation block from a single-instance counter or collection as 
		/// specified by its unique name.  
		/// </para>
		/// <para>
		/// If the counter or collection specified by the unique name is a multi-instance 
		/// counter or collection, this is an error and will always throw regardless of 
		/// the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// The counter could be an AutoCounter or an AutoCounterCollection, regardless
		/// it will return the result of GetBlock() on that ICounter instance.
		/// </para>
		/// </summary>
		/// <param name="uniqueName">A unique name that identifies the counter.</param>
		/// <returns>An instrumentation block for the counter.</returns>
		InstrumentationBlock InstrumentBlock(string uniqueName);

		/// <summary>
		/// <para>
		/// Gets an instrumentation block from a multi-instance counter or collection as specified 
		/// by its unique name and instance name.  
		/// </para>
		/// <para>
		/// If the counter or collection specified by the unique name is a multi-instance 
		/// counter or collection, this is an error and will always throw regardless of 
		/// the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// The counter could be an AutoCounter or an AutoCounterCollection, regardless
		/// it will return the result of GetBlock() on that ICounter instance.
		/// </para>
		/// </summary>
		/// <param name="uniqueName">A unique name that identifies the counter or collection.</param>
		/// <param name="instanceName">An instance name that identifies the specific instance.</param>
		/// <returns>An instrumentation block for the counter.</returns>
		InstrumentationBlock InstrumentBlock(string uniqueName, string instanceName);

		/// <summary>
		/// <para>
		/// Gets the specified single-instance counter or collection and returns the generic ICounter 
		/// interface to it.  Be careful when you get an underlying counter, if you dispose the counter 
		/// that is in the cache, it will invalidate it for any future calls.
		/// </para>
		/// <para>
		/// If the counter or counter collection is a multi-instance counter or collection, 
		/// this is an error and will always throw regardless of the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// </summary>
		/// <param name="uniqueName">The unique name that identifies the counter or collection.</param>
		/// <returns>An ICounter reference to the counter or collection.</returns>
		ICounter Get(string uniqueName);

		/// <summary>
		/// <para>
		/// Gets the specified multi-instance counter or collection and returns the generic ICounter 
		/// interface to it.  Be careful when you get an underlying counter, if you dispose the counter 
		/// that is in the cache, it will invalidate it for any future calls.
		/// </para>
		/// <para>
		/// If the counter or counter collection is a single-instance counter or collection, 
		/// this is an error and will always throw regardless of the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// </summary>
		/// <param name="uniqueName">The unique name that identifies the counter or collection.</param>
		/// <param name="instanceName">An instance name that identifies the specific instance.</param>
		/// <returns>An ICounter reference to the counter or collection.</returns>
		ICounter Get(string uniqueName, string instanceName);

		/// <summary>
		/// <para>
		/// Gets the specified single-instance counter or collection and returns the specific  
		/// reference to it.  Be careful when you get an underlying counter, if you dispose the counter 
		/// that is in the cache, it will invalidate it for any future calls.
		/// </para>
		/// <para>
		/// If the counter or counter collection is a single-instance counter or collection, 
		/// this is an error and will always throw regardless of the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type of the counter</typeparam>
		/// <param name="uniqueName">The unique name that identifies the counter or collection.</param>
		/// <returns>An ICounter reference to the counter or collection.</returns>
		T Get<T>(string uniqueName) where T : class, ICounter;

		/// <summary>
		/// <para>
		/// Gets the specified multi-instance counter or collection and returns the specific  
		/// reference to it.  Be careful when you get an underlying counter, if you dispose the counter 
		/// that is in the cache, it will invalidate it for any future calls.
		/// </para>
		/// <para>
		/// If the counter or counter collection is a multi-instance counter or collection, 
		/// this is an error and will always throw regardless of the value of CreateFailedAction.
		/// </para>
		/// <para>
		/// If the counter or collection does not exist at all, then the result will either
		/// be a null block or will throw depending on the value of CreateFailedAction.
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type of the counter</typeparam>
		/// <param name="uniqueName">The unique name that identifies the counter or collection.</param>
		/// <param name="instanceName">An instance name that identifies the specific instance.</param>
		/// <returns>An ICounter reference to the counter or collection.</returns>
		T Get<T>(string uniqueName, string instanceName) where T : class, ICounter;

		/// <summary>
		/// Resets all AutoCounters held in the cache.  This effectively will also initialize every counter
		/// for first use if the counter has not already been created.  So if you are taking advantage of lazy loading
		/// this will somewhat negate it.  It is recommended you create categories instead, but this is an easy brute-force
		/// method for resetting all counters.
		/// </summary>
		void ResetAll();
	}
}
