using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SharedAssemblies.Core.Timers;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;

namespace SharedAssemblies.Monitoring.AutoCounters
{
	/// <summary>
	/// A cache of AutoCounters and collections.  This cache allows you to examine all the 
	/// auto counters registered and query them for instrumentation blocks.
	/// </summary>
	internal class AutoCounterCache : ICounterCache
	{
		/// <summary>The registry of available auto counters we can construct</summary>
		private readonly AutoCounterRegistry _registry;

		/// <summary>Timer to control auto-resetting all counters at midnight server time. </summary>
		private readonly MidnightTimer _autoResetTimer;

		/// <summary>
		/// Timer or Thread to flush out the data in non-locking counters that is not directly
		/// written to the underlying counter values.
		/// </summary>
		private readonly object _flushCounterTimer;

		/// <summary>
		/// Lock to prevent re-entrant calls in the flush counter timer.
		/// </summary>
		private readonly object _flushCounterLock;

		/// <summary>
		/// Whether or not the counters are using a non-locking strategy. See 
		/// <see cref="NonLockingAutoCountersAttribute"/> for more information.
		/// </summary>
		public bool UseNonLocking { get; private set; }

		/// <summary>
		/// Gets or sets whether to auto-reset all counters at midnight (server time) to initial values.
		/// </summary>
		public bool ShouldAutoResetAllDaily
		{
			get { return _autoResetTimer.IsEnabled; }
			set { _autoResetTimer.IsEnabled = value; }
		}

		/// <summary>
		/// Gets an IEnumerable of string that contains all the unique names that can be used to key
		/// the AutoCounterCache.
		/// </summary>
		public IEnumerable<string> UniqueNames
		{
			get { return _registry.UniqueNames.Keys; }
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
		InstrumentationBlock ICounterCache.this[string uniqueName]
		{
			get { return InstrumentBlock(uniqueName); }
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
		InstrumentationBlock ICounterCache.this[string uniqueName, string instanceName]
		{
			get { return InstrumentBlock(uniqueName, instanceName); }
		}

		/// <summary>
		/// Constructs a new auto counter cache using the given registry
		/// </summary>
		/// <param name="registry">Registry of available auto counters to construct.</param>
		/// <param name="interval">Time in milliseconds between flushes of non-locking
		/// auto counters.  A value of null indicates that the counters should use locks instead.
		/// </param>
		internal AutoCounterCache(AutoCounterRegistry registry, NonLockingAutoCountersAttribute interval)
		{
			_registry = registry;
			UseNonLocking = interval != null;

			// start auto-heartbeats in registry
			_registry.StartHeartbeats();

			// set up auto-rest timer but leave it disabled until it is explicitly enabled.
			_autoResetTimer = new MidnightTimer();
			_autoResetTimer.Elapsed += OnAutoResetTimerElapsed;

			if(UseNonLocking)
			{
				_flushCounterLock = new object();
				_flushCounterTimer = CreateFlushThread(interval);
			}
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
		public InstrumentationBlock InstrumentBlock(string uniqueName)
		{
			var counter = Get(uniqueName);
			return (counter != null) ? counter.GetBlock() : default(InstrumentationBlock);
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
		public InstrumentationBlock InstrumentBlock(string uniqueName, string instanceName)
		{
			var counter = Get(uniqueName, instanceName);
			return (counter != null) ? counter.GetBlock() : default(InstrumentationBlock);
		}

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
		public ICounter Get(string uniqueName)
		{
			var registration = _registry.Get(uniqueName);

			if (registration != null)
			{
				if (registration.InstanceType == InstanceType.SingleInstance)
				{
					return registration.GetCounter();
				}

				throw new AutoCounterException(
					string.Format("An attempt was made to create a single-instance " +
								  "counter {0} that was declared as multi-instance.",
								  uniqueName));
			}

			throw new AutoCounterException(
				string.Format("An attempt was made to create a single-instance counter {0} which was not " +
							  "registered with an assembly-level AutoCounterAttribute.",
							  uniqueName));
		}

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
		public ICounter Get(string uniqueName, string instanceName)
		{
			var registration = _registry.Get(uniqueName);

			if (registration != null)
			{
				if (registration.InstanceType == InstanceType.MultiInstance)
				{
					return registration.GetCounterInstance(instanceName);
				}

				throw new AutoCounterException(
					string.Format("An attempt was made to create a mutli-instance " +
								  "counter {0} that was declared as single-instance.",
								  uniqueName));
			}

			throw new AutoCounterException(
				string.Format("An attempt was made to create a multi-instance counter {0} which was not " +
							  "registered with an assembly-level AutoCounterAttribute.",
							  uniqueName));
		}

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
		/// <param name="uniqueName">The unique name that identifies the counter or collection.</param>
		/// <typeparam name="T">The type to cast the auto counter.</typeparam>
		/// <returns>An ICounter reference to the counter or collection.</returns>
		public T Get<T>(string uniqueName) where T : class, ICounter
		{
			return Get(uniqueName) as T;
		}

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
		/// <param name="uniqueName">The unique name that identifies the counter or collection.</param>
		/// <param name="instanceName">An instance name that identifies the specific instance.</param>
		/// <typeparam name="T">The type to cast the AutoCounter as</typeparam>
		/// <returns>An ICounter reference to the counter or collection.</returns>
		public T Get<T>(string uniqueName, string instanceName) where T : class, ICounter
		{
			return Get(uniqueName, instanceName) as T;
		}

		/// <summary>
		/// Resets all AutoCounters held in the cache.  This effectively will also initialize every counter
		/// for first use if the counter has not already been created.  So if you are taking advantage of lazy loading
		/// this will somewhat negate it.  It is recommended you create categories instead, but this is an easy brute-force
		/// method for resetting all counters.
		/// </summary>
		public void ResetAll()
		{
			try
			{
				foreach (var registration in _registry.Counters)
				{
					var counterRegistration = registration.Value;
					var category = counterRegistration.Category;

					if (category.InstanceType == InstanceType.MultiInstance)
					{
						ProcessMultiInstanceCounter(category, counterRegistration, c => c.Reset());
					}
					else
					{
						var counter = Get<AutoCounter>(counterRegistration.UniqueName);
						if (counter != null)
						{
							counter.Reset();
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new AutoCounterException("Could not perform ResetAll() for all counters.", ex);
			}
		}


		/// <summary>
		/// Queries the OS for all counter instances and then cycles through each instance to execute 
		/// the supplied operation.
		/// </summary>
		/// <param name="categoryRegistration">The multi-instance category the counter belongs to.</param>
		/// <param name="counterRegistration">The multi-instance counter.</param>
		/// <param name="action">Action to perform.</param>
		private void ProcessMultiInstanceCounter(AutoCounterCategoryRegistration categoryRegistration, 
			AutoCounterRegistration counterRegistration, Action<AutoCounter> action)
		{
			var perfCounterCategory = new PerformanceCounterCategory(categoryRegistration.UniqueName);
			var instances = perfCounterCategory.GetInstanceNames();

			if (instances.Length > 0)
			{
				foreach (var instance in instances)
				{
					var counter = Get<AutoCounter>(counterRegistration.UniqueName, instance);

					if (counter != null)
					{
						action(counter);
					}
				}
			}
		}

		/// <summary>
		/// When the auto reset timer interval has elapsed, we should go ahead and reset all auto-counters and then set the timer
		/// again for midnight of next day.
		/// </summary>
		/// <param name="sender">Timer that generated the event.</param>
		/// <param name="e">Timer elapsed event arguments.</param>
		private void OnAutoResetTimerElapsed(object sender, ElapsedEventArgs e)
		{
			// reset all auto counters
			ResetAll();
		}

		/// <summary>
		/// Creates the background task or thread that flushes out the auto counter
		/// values on a periodic basis.  
		/// </summary>
		/// <param name="interval">Non locking attribute information from the assembly</param>
		/// <returns>The task or thread</returns>
		private object CreateFlushThread(NonLockingAutoCountersAttribute interval)
		{
			object timerObject;	

			if(interval.UseDedicatedThread)
			{
				var flushCounterThread = new Thread(FlushCounterBackgroundThread)
				{
					IsBackground = true, // won't stop the program from exiting.
					Priority = interval.DedicatedThreadPriority,
					Name = "SA Counter Flush Thread",
				};
				flushCounterThread.Start(interval.UpdateInterval);
				timerObject = flushCounterThread;
			}
			else
			{
				var flushCounterTimer = new System.Timers.Timer
				{
					AutoReset = true,
					Interval = interval.UpdateInterval,
					Enabled = true,
				};
				flushCounterTimer.Elapsed += OnFlushCounterTimerElapsed;
				flushCounterTimer.Start();
				timerObject = flushCounterTimer;
			}

			return timerObject;
		}


		/// <summary>
		/// Flushes all non-locking auto counter values so that they are written into
		/// the system performance counter objects.
		/// </summary>
		/// <param name="sender">Timer that generated the event.</param>
		/// <param name="e">Timer elapsed event arguments.</param>
		private void OnFlushCounterTimerElapsed(object sender, ElapsedEventArgs e)
		{
			// need a lock to prevent multiple tasks from trying to execute at once
			// which can happen when 
			lock(_flushCounterLock)
			{
				foreach(var registration in _registry.Counters)
				{
					FlushCounter(registration.Value);
				}
			}
		}
		
		/// <summary>
		/// Background thread method that will sleep for a specific interval
		/// and then flush all non-locking auto counters that require it.
		/// </summary>
		/// <param name="interval">Sleep interval in milliseconds (integer)</param>
		private void FlushCounterBackgroundThread(object interval)
		{
			var resetEvent = new ManualResetEvent(false);
			int sleepValue = (int)interval;
			while(true)
			{
				resetEvent.WaitOne(sleepValue); // more reliable than Thread.Sleep.
				foreach(var registration in _registry.Counters)
				{
					FlushCounter(registration.Value);
				}
			}
		}

		/// <summary>
		/// Flushes the counter value for the <see cref="AutoCounter"/> associated
		/// with the supplied registration.
		/// </summary>
		/// <param name="registration">Counter registration to process.</param>
		private void FlushCounter(AutoCounterRegistration registration)
		{
			var category = registration.Category;
			try
			{
				if (category.InstanceType == InstanceType.MultiInstance)
				{
					ProcessMultiInstanceCounter(category, 
							registration, c => c.FlushCounter());
				}
				else
				{
					var counter = Get<AutoCounter>(registration.UniqueName);
					if (counter != null)
					{
						counter.FlushCounter();
					}
				}
			}
			catch(Exception)
			{
				// just swallow exceptions, propagating errors out
				// would do more harm than good here.	
			}
		}
	}
}
