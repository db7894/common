using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
	/// <summary>
	/// A registration for an auto counter collection that groups together common
	/// counters into one collection with a possible parent group.
	/// </summary>
	internal class AutoCounterCollectionRegistration : ICounterRegistration
	{
		/// <summary>The counter tied to the registration</summary>
		private readonly Lazy<ICounter> _counter;

		/// <summary>The instances of the counter in this collection</summary>
		private readonly ConcurrentDictionary<string, ICounter> _counterInstances;

		/// <summary>
		/// List of AutoCounters to install, these are the counters contained within
		/// this counter collection.
		/// </summary>
		private readonly IEnumerable<AutoCounterRegistration> _autoCounters;

		/// <summary>
		/// Collection name
		/// </summary>
		public string UniqueName { get; private set; }

		/// <summary>
		/// True if the collection has multiple instances, usually best kept false
		/// </summary>
		public InstanceType InstanceType { get; private set; }

		/// <summary>
		/// Gets the parent collection of this collection.  If the parent is null, it is 
		/// assumed that there is no parent collection.  Never make a circular references!
		/// </summary>
		public AutoCounterCollectionRegistration ParentCollection { get; set; }

		/// <summary>
		/// The failed creation action to use for lazily initialized auto counters.
		/// </summary>
		internal readonly CreateFailedAction _failedAction;

		/// <summary>
		/// Whether or not to use non-locking auto counters.
		/// </summary>
		internal readonly bool _useNonLocking;

		/// <summary>
		/// Default constructor for the collection registration
		/// </summary>
		/// <param name="instanceType">The type of the counter instance</param>
		/// <param name="uniqueName">The unique name to store the counter under</param>
		/// <param name="counters">List of counters to contain within the collection.</param>
		/// <param name="createFailedAction">Action to take if counter does not exist.</param>
		/// <param name="interval">Time in milliseconds between flushes of non-locking
		/// auto counters.  A value of null indicates that the counters should use locks instead.
		/// </param>
		public AutoCounterCollectionRegistration(InstanceType instanceType, string uniqueName,
													IEnumerable<AutoCounterRegistration> counters,
													CreateFailedAction createFailedAction,
													NonLockingAutoCountersAttribute interval)
		{
			if (uniqueName == null)
			{
				throw new ArgumentNullException("uniqueName");
			}

			InstanceType = instanceType;
			UniqueName = uniqueName;
			_autoCounters = counters;
			_failedAction = createFailedAction;
			_useNonLocking = interval != null;
			_counter = new Lazy<ICounter>(() => CreateCounterCollection(null));

			if(instanceType == InstanceType.MultiInstance)
			{
				_counterInstances = new ConcurrentDictionary<string, ICounter>();
			}
		}

		/// <summary>
		/// gets the AutoCounter that backs this registration if it already exists,
		/// or attempts to create it if it does not currently exist.
		/// </summary>
		/// <returns>An ICounter interface reference to the AutoCounter.</returns>
		public ICounter GetCounter()
		{
			return _counter.Value;
		}
		
		/// <summary>
		/// gets the AutoCounter that backs this registration if it already exists,
		/// or attempts to create it if it does not currently exist.
		/// </summary>
		/// <param name="instanceName">The name of the instance of the counter to get.</param>
		/// <returns>An ICounter interface reference to the AutoCounter.</returns>
		public ICounter GetCounterInstance(string instanceName)
		{
			return _counterInstances == null 
						? null 
						: _counterInstances.GetOrAdd(instanceName, CreateCounterCollection);
		}

		/// <summary>
		/// Helper method to construct the parent instance depending on whether it exists,
		/// and whether it is single-instance, or is multi-instance
		/// </summary>
		/// <param name="instanceName">The name of the instance</param>
		/// <returns>The parent collection of auto counters</returns>
		private AutoCounterCollection CreateParentCollection(string instanceName)
		{
			ICounter parentCollection = null;

			if (ParentCollection != null)
			{
				if (instanceName == null
					|| (ParentCollection.InstanceType == InstanceType.SingleInstance))
				{
					parentCollection = ParentCollection.GetCounter();
				}
				else
				{
					parentCollection = ParentCollection.GetCounterInstance(instanceName);
				}
			}

			return (AutoCounterCollection)parentCollection;
		}

		/// <summary>
		/// Creates an auto-counter collection.
		/// </summary>
		/// <param name="instanceName">Optional instance name for determining a parent counter
		///	collection.</param>
		/// <returns>The counter collection</returns>
		private ICounter CreateCounterCollection(string instanceName)
		{
			var counters = _autoCounters.Select(c => (AutoCounter)c.GetCounter());

			// construct the collection and construct the counters if they haven't already been
			return new AutoCounterCollection(CreateParentCollection(instanceName), counters, _useNonLocking);
		}

	}
}
