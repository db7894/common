using System;
using System.Reflection;


namespace SharedAssemblies.Monitoring.AutoCounters
{
	/// <summary>
	/// Class for an easier-to-use performance counter
	/// </summary>
	public class AutoCounter : ICounter, IDisposable
	{
		/// <summary>
		/// The name used to identify that a counter has a single instance
		/// </summary>
		public static readonly string SingleInstanceName = string.Empty;

		/// <summary>The auto counter strategy</summary>
		private readonly Strategies.ICounterStrategy _strategy;

		/// <summary>
		/// Whether or not this counter needs to be flushed by the background process
		/// that updates non-locking counters. 
		/// </summary>
		private readonly bool _requiresFlush;

		/// <summary>
		/// Whether or not the counter requires a start time in the
		/// InsturmentationBlock.
		/// </summary>
		private readonly bool _requiresTimer;

		/// <summary>
		/// Gets the name of the performance counter
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the description of the performance counter
		/// </summary>
		public string Category { get; private set; }

		/// <summary>
		/// Gets the instance of the performance counter
		/// </summary>
		public string Instance { get; private set; }

		/// <summary>
		/// Allows you to specify an alternate name to this counter instead of relying on the
		/// category:name convention.  This property is only used by the reporter and the
		/// cache and has no effect on the underlying PerformanceCounter(s) at all.
		/// </summary>
		public string UniqueName { get; set; }

		/// <summary>
		/// Whether or not the auto counter requires a start time.
		/// </summary>
		internal bool RequiresStartTimer { get { return _requiresTimer; } }

		/// <summary>
		/// Initialize the performance counter based on its name and description
		/// </summary>
		/// <param name="category">Name of performance counter category</param>
		/// <param name="name">Name of performance counter instance</param>
		/// <param name="readOnly">True if counter is read-only</param>
		public AutoCounter(string category, string name, bool readOnly)
			: this(category, name, null, readOnly,
				AutoCounterCacheFactory.GetDefaultCreateFailedAction(Assembly.GetCallingAssembly()))
		{
		}

		/// <summary>
		/// Initialize the performance counter based on its name and description
		/// </summary>
		/// <param name="category">Name of performance counter category</param>
		/// <param name="name">Name of performance counter instance</param>
		/// <param name="readOnly">True if counter is read-only</param>
		/// <param name="actionIfCreateFails">The action to perform if the create fails</param>
		public AutoCounter(string category, string name, bool readOnly,
			CreateFailedAction actionIfCreateFails)
			: this(category, name, null, readOnly, actionIfCreateFails)
		{
		}

		/// <summary>
		/// Initialize the performance counter based on its name and description
		/// </summary>
		/// <param name="category">Name of performance counter category</param>
		/// <param name="name">Name of performance counter instance</param>
		/// <param name="readOnly">True if counter is read-only</param>
		/// <param name="actionIfCreateFails">The action to perform if the create fails</param>
		/// <param name="useNonLocking">If true then uses non-locking functionality</param>
		public AutoCounter(string category, string name, bool readOnly,
			CreateFailedAction actionIfCreateFails, bool useNonLocking)
			: this(category, name, null, readOnly, actionIfCreateFails, useNonLocking)
		{
		}

		/// <summary>
		/// Initialize the performance counter based on its name,
		/// description, and instance
		/// </summary>
		/// <param name="category">
		/// Name of performance counter category</param>
		/// <param name="name">
		/// Name of performance counter instance</param>
		/// <param name="instance">Instance of category counter 
		/// lives in</param>
		/// <param name="readOnly">True if counter is read-only</param>
		public AutoCounter(string category, string name, string instance, bool readOnly)
			: this(category, name, instance, readOnly,
				AutoCounterCacheFactory.GetDefaultCreateFailedAction(Assembly.GetCallingAssembly()))
		{
		}

		/// <summary>
		/// Initialize the performance counter based on its name, description, and instance
		/// </summary>
		/// <param name="category">Name of performance counter category</param>
		/// <param name="name">Name of performance counter instance</param>
		/// <param name="instance">Instance of category counter lives in</param>
		/// <param name="readOnly">True if counter is read-only</param>
		/// <param name="actionIfCreateFails">The action to perform if the create fails</param>
		public AutoCounter(string category, string name, string instance,
						   bool readOnly, CreateFailedAction actionIfCreateFails)
			: this(category, name, instance, readOnly, actionIfCreateFails, false)
		{
		}

		/// <summary>
		/// Initialize the performance counter based on its name, description, and instance
		/// </summary>
		/// <param name="category">Name of performance counter category</param>
		/// <param name="name">Name of performance counter instance</param>
		/// <param name="instance">Instance of category counter lives in</param>
		/// <param name="readOnly">True if counter is read-only</param>
		/// <param name="actionIfCreateFails">The action to perform if the create fails</param>
		/// <param name="useNonLocking">If true then uses non-locking functionality</param>
		public AutoCounter(string category, string name, string instance,
						   bool readOnly, CreateFailedAction actionIfCreateFails, bool useNonLocking)
		{
			// set all pc fields
			Name = name;
			Category = category;
			Instance = instance ?? SingleInstanceName;

			UniqueName = category + ':' + name;

			_strategy = Strategies.CounterStrategyFactory.Create(Category, Name, Instance, readOnly, 
																 actionIfCreateFails, useNonLocking);

			// Read this only once instead of every time we want to do a flush as the assumption
			// is that the counter either always needs to be manually copied down to the performance
			// counter or it never needs to be copied.  
			_requiresFlush = _strategy.RequiresFlush;
			_requiresTimer = _strategy.RequireStartTimer;
		}

		/// <summary>
		/// Resets the raw value of the counters to zero
		/// </summary>
		public void Reset()
		{
			_strategy.Reset();
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
		/// Increments the counter by the supplied value.
		/// </summary>
		/// <param name="value">The value to increment the counter by</param>
		public void IncrementBy(long value)
		{
			_strategy.IncrementBy(value);
		}

		/// <summary>
		/// Return the value of the current counter
		/// </summary>
		/// <returns>Returns the next calculated value from the counter</returns>
		public double NextValue()
		{
			return _strategy.NextValue();
		}

		/// <summary>
		/// Sets the raw value of the performance counter to the value indicated.
		/// </summary>
		/// <param name="rawValue">The raw value to set the counter to.</param>
		public void SetValue(long rawValue)
		{
			_strategy.SetValue(rawValue);
		}

		/// <summary>
		/// Returns a IDisposable InstrumentationBlock to automatically
		/// call Enter and Leave correctly to avoid screw-ups.
		/// </summary>
		/// <returns>A new instrumentation block that can be used in a using statement</returns>
		public InstrumentationBlock GetBlock()
		{
			return new InstrumentationBlock(this, _requiresTimer);
		}

		/// <summary>
		/// Clean up any system resources
		/// </summary>
		public void Dispose()
		{
			_strategy.Dispose();
		}

		/// <summary>
		/// Flushes any cached data in the counter down to the PerformanceCounter objects
		/// owned by this strategy.
		/// </summary>
		public void FlushCounter()
		{
			if(_requiresFlush)
			{
				_strategy.FlushCounter();
			}
		}
	}
}