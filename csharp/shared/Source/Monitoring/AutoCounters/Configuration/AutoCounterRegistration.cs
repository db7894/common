using System;
using System.Collections.Concurrent;

namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
	/// <summary>
	/// Creation data for installing auto counters
	/// </summary>
	internal class AutoCounterRegistration : ICounterRegistration
	{
		/// <summary>The counter that is registered</summary>
		private readonly Lazy<ICounter> _counter;

		/// <summary>A dictionary of instances of the counter</summary>
		private readonly ConcurrentDictionary<string, ICounter> _counterInstances;

		/// <summary>
		/// A unique name that can be applied to the counter as opposed to the Category:Name combination
		/// </summary>
		public string UniqueName { get; private set; }

		/// <summary>
		/// The instance type of this registration
		/// </summary>
		public InstanceType InstanceType { get; private set; }

		/// <summary>
		/// The name of the auto counter
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// The type of the auto counter
		/// </summary>
		public AutoCounterType Type { get; set; }

		/// <summary>
		/// The counter category name
		/// </summary>
		public AutoCounterCategoryRegistration Category { get; set; }

		/// <summary>
		/// The description of the auto counter
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Attribute for whether this counter is read-only or not
		/// </summary>
		public bool IsReadOnly { get; set; }

		/// <summary>
		/// The failed creation action to use for lazily initialized auto counters.
		/// </summary>
		internal readonly CreateFailedAction _failedAction;

		/// <summary>
		/// Whether or not to use non-locking auto counters.
		/// </summary>
		internal readonly bool _useNonLocking;

		/// <summary>
		/// Constructs the base registration with no sync root
		/// </summary>
		/// <param name="instanceType">The type of instance</param>
		/// <param name="uniqueName">The unique name items are stored in the registry under</param>
		/// <param name="createFailedAction">Action to take if counter does not exist.</param>
		/// <param name="interval">Time in milliseconds between flushes of non-locking
		/// auto counters.  A value of null indicates that the counters should use locks instead.
		/// </param>
		public AutoCounterRegistration(InstanceType instanceType, string uniqueName, 
											CreateFailedAction createFailedAction,
											NonLockingAutoCountersAttribute interval)
		{
			if (uniqueName == null)
			{
				throw new ArgumentNullException("uniqueName");
			}

			InstanceType = instanceType;
			UniqueName = uniqueName;
			_failedAction = createFailedAction;
			_useNonLocking = interval != null;
			_counter = new Lazy<ICounter>(() => CreateCounter(null));

			if (instanceType == InstanceType.MultiInstance)
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
			       	: _counterInstances.GetOrAdd(instanceName, CreateCounter);
		}


		/// <summary>
		/// Creates an AutoCounter.
		/// </summary>
		/// <param name="instanceName">Optional instance name to use for instance counters</param>
		/// <returns>An auto counter.</returns>
		private ICounter CreateCounter(string instanceName)
		{
			return new AutoCounter(Category.UniqueName, 
									Name, instanceName, IsReadOnly,
									_failedAction, _useNonLocking);
		}
	}
}