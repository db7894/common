using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using SharedAssemblies.Core.Conversions;


namespace SharedAssemblies.Monitoring.AutoCounters.Strategies
{
    /// <summary>
    /// Collection of factories to help instantiate the correct
    /// counter strategy
    /// </summary>
    internal static class CounterStrategyFactory
    {
        /// <summary>
        /// The suffix added to a counter name for a subordinate counter
        /// </summary>
        internal const string BaseCounterSuffix = "-Base";


        /// <summary>
        /// Translator from a performance counter tyep to an auto counter type
        /// </summary>
        private static readonly Translator<PerformanceCounterType, AutoCounterType?> 
			_performanceCounterTypeTranslator = new Translator<PerformanceCounterType, AutoCounterType?>(null)
                {
                    { PerformanceCounterType.RawFraction, AutoCounterType.RollingAverageTime },
                    { PerformanceCounterType.AverageTimer32, AutoCounterType.AverageTime },
                    { PerformanceCounterType.RateOfCountsPerSecond32, AutoCounterType.CountsPerSecond },
                    { PerformanceCounterType.RateOfCountsPerSecond64, AutoCounterType.CountsPerSecond },
                    { PerformanceCounterType.NumberOfItems32, AutoCounterType.TotalCount },
                    { PerformanceCounterType.NumberOfItems64, AutoCounterType.TotalCount },
                    { PerformanceCounterType.ElapsedTime, AutoCounterType.ElapsedTime },
                };


        /// <summary>
        /// Internal factory method to create a new counter strategy
        /// </summary>
        /// <param name="category">The category for the counter</param>
        /// <param name="name">The name of the counter</param>
        /// <param name="instance">The name of the instance</param>
        /// <param name="readOnly">True if the counter is read-only</param>
        /// <param name="actionIfCreateFails">Action to take if creating the counter fails</param>
		/// <param name="useNonLocking">If true then uses non-locking functionality</param>
        /// <returns>The strategy appropriate for the counter</returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException", 
			Justification = "If creation fails, wrap and rethrow to abstract details.")]
		internal static ICounterStrategy Create(string category, string name, string instance,
                           bool readOnly, CreateFailedAction actionIfCreateFails, bool useNonLocking)
        {
            ICounterStrategy result = null;

            try
            {
                var counter = new PerformanceCounter(category, name, instance, readOnly);
                result = DetermineStrategy(counter, readOnly, useNonLocking);
            }
            catch(Exception ex)
            {
                // if the failed action is to throw, throw exception, otherwise our result is null and
                // we will just return a null strategy
                if(actionIfCreateFails == CreateFailedAction.ThrowException)
                {
                    throw new AutoCounterException(
						"Could not create performance counter on this machine.", ex);
                }
            }

            return result ?? new StubCounterStrategy();
        }

		/// <summary>
		/// Internal factory method to create a new counter collection strategy
		/// </summary>
		/// <param name="parentCollection">Parent collection (may be null)</param>
		/// <param name="counters">Counters to store in the counter collection.</param>
		/// <param name="useNonLocking">If true then uses the newer non-locking collection</param>
		/// <returns>The strategy appropriate for the counter</returns>
		internal static ICounterCollectionStrategy CreateCollection(AutoCounterCollection parentCollection,
											IEnumerable<AutoCounter> counters, bool useNonLocking)
		{
			ICounterCollectionStrategy result = null;

			// use the new fixed size counter collections
			if(useNonLocking)
			{
				result = new FixedSizeCounterCollectionStrategy(parentCollection, counters);
			}
			// return the old counter collection type.
			else
			{
				result = new CounterCollectionStrategy(parentCollection, counters);
			}
			return result;
		}

        /// <summary>
        /// Factory to create a system performance counter
        /// </summary>
        /// <param name="counter">The counter to determine the strategy of</param>
        /// <param name="isReadOnly">True if counter should be read-only</param>
		/// <param name="useNonLocking">If true then uses non-locking functionality</param>
        /// <returns>The strategy for the counter</returns>
        private static ICounterStrategy DetermineStrategy(PerformanceCounter counter, 
																bool isReadOnly, bool useNonLocking)
        {
            AutoCounterType? type = _performanceCounterTypeTranslator[counter.CounterType];

            // if not in our translator map, we don't know how to build it
            if(!type.HasValue)
            {
                throw new AutoCounterException("AutoCounter has unsupported underlying " 
                    + "PerformanceCounter type.");                
            }

        	ICounterStrategy result;

            // build the underlying strategy
            switch (type.Value)
            {
                case AutoCounterType.RollingAverageTime:
                    result = !useNonLocking	? new RollingAverageStrategy(counter) as ICounterStrategy
											: new NonLockingRollingAverageStrategy(counter);
					break;
                case AutoCounterType.AverageTime:
                    result = !useNonLocking	? new AverageTimeStrategy(counter) as ICounterStrategy
											: new NonLockingAverageTimeStrategy(counter);
            		break;
                case AutoCounterType.CountsPerSecond:
                    result = !useNonLocking	? new RateOfCountStrategy(counter) as ICounterStrategy
											: new NonLockingRateOfCountStrategy(counter);
					break;
                case AutoCounterType.TotalCount:
                    result = !useNonLocking	? new NumberOfItemsStrategy(counter) as ICounterStrategy
											: new NonLockingNumberOfItemsStrategy(counter);
					break;
                case AutoCounterType.ElapsedTime:
                    result = !useNonLocking	? new ElapsedTimeStrategy(counter, isReadOnly) as ICounterStrategy
											: new NonLockingElapsedTimeStrategy(counter, isReadOnly);
					break;
                default:
                    result = new StubCounterStrategy();
					break;
            }

			// wrap counters marked as read-only in a strategy that turns
			// modifying calls (Increment, Reset, etc) into no-ops.  
			if(isReadOnly)
			{
				result = new ReadOnlyStrategy(result);
			}
        	return result;
        }
    }
}