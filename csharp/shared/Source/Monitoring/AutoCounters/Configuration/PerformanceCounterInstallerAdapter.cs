using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharedAssemblies.Monitoring.AutoCounters.Strategies;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// Internal adapter that converts a list of AutoCounters to the underlying PerformanceCounter
    /// objects.  Some AutoCounters equate to more than one PerformanceCounter due to underlying base counters.
    /// </summary>
    internal static class PerformanceCounterInstallerAdapter
    {
        /// <summary>
        /// Converts the enumeration of auto counter categories to a set of performance counter installers.
        /// </summary>
        /// <param name="categories">The categories to create installers for</param>
        /// <returns>Returns the collection of installers</returns>
        internal static IEnumerable<PerformanceCounterInstaller> GetInstallers(
			IEnumerable<AutoCounterCategoryRegistration> categories)
        {
            var results = new List<PerformanceCounterInstaller>();

            categories.ForEach(category => CreateInstallers(category, results));

            return results;
        }


        /// <summary>
        /// Convert one category to its associated performance coutner installers.
        /// </summary>
        /// <param name="category">The category information for the counters</param>
        /// <param name="installers">The collection of installers to run</param>
        private static void CreateInstallers(AutoCounterCategoryRegistration category, 
            ICollection<PerformanceCounterInstaller> installers)
        {
            // convert the category
            var installer = new PerformanceCounterInstaller
                                {
                                    CategoryName = category.UniqueName,
                                    CategoryHelp = category.Description,
                                    CategoryType = category.InstanceType == InstanceType.MultiInstance
                                                       ? PerformanceCounterCategoryType.MultiInstance
                                                       : PerformanceCounterCategoryType.SingleInstance
                                };

            // now convert all the categories under it
            category.AutoCounters.ForEach(counter => CreateCounters(counter.Value, installer));

            installers.Add(installer);
        }


        /// <summary>
        /// Converts the auto counter to its underlying performance counters
        /// </summary>
        /// <param name="counter">The counter to install</param>
        /// <param name="installer">The performance counter installer</param>
        private static void CreateCounters(AutoCounterRegistration counter, 
			PerformanceCounterInstaller installer)
        {
            // generate the correct PerformanceCounter(s) for the given AutoCounter type.
            switch(counter.Type)
            {
                case AutoCounterType.RollingAverageTime:
                    installer.Counters.Add(
                        new CounterCreationData(counter.Name, counter.Description,
                                                PerformanceCounterType.RawFraction));
                    installer.Counters.Add(
                        new CounterCreationData(counter.Name + CounterStrategyFactory.BaseCounterSuffix,
                                                counter.Description,
                                                PerformanceCounterType.RawBase));
                    break;

                case AutoCounterType.AverageTime:
                    installer.Counters.Add(
                        new CounterCreationData(counter.Name, counter.Description,
                                                PerformanceCounterType.AverageTimer32));
                    installer.Counters.Add(
                        new CounterCreationData(counter.Name + CounterStrategyFactory.BaseCounterSuffix,
                                                counter.Description,
                                                PerformanceCounterType.AverageBase));
                    break;

                case AutoCounterType.CountsPerSecond:
                    installer.Counters.Add(
                        new CounterCreationData(counter.Name, counter.Description,
                                                PerformanceCounterType.RateOfCountsPerSecond32));
                    break;

                case AutoCounterType.TotalCount:
                    installer.Counters.Add(
                        new CounterCreationData(counter.Name, counter.Description,
                                                PerformanceCounterType.NumberOfItems32));
                    break;

                case AutoCounterType.ElapsedTime:
                    installer.Counters.Add(
                        new CounterCreationData(counter.Name, counter.Description,
                                                PerformanceCounterType.ElapsedTime));
                    break;

                default:
                    throw new ArgumentOutOfRangeException("counter", 
                        "Invalid Type property in parameter counter.");
            }
        }
    }
}
