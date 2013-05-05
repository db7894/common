using System;
using System.Diagnostics;
using System.Threading;

namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
	/// <summary>
	/// Attribute that defines whether or not to use non-locking counters for an assembly.  Adding
	/// this attribute in an assembly ensures that counters returned for the assembly use
	/// non-locking algorithms for improved performance at the cost of some accuracy.
	/// <para>
	/// There are several differences in the behavior of the counters when in the non-locking
	/// mode.  The primary difference is that none of the operations acquire a lock on the counter
	/// which would prevent concurrent operations from using the same counter at exact same time.
	/// However to achieve this some of the auto counters store their data in variables that
	/// are outside of the <see cref="PerformanceCounter"/> object wrapped by this library.  These
	/// values need to be pushed into the performance counter object by a background task/thread
	/// which is automatically started and managed in the <see cref="AutoCounterCache"/>.
	/// </para>
	/// The following is an example usage of this attribute.
	/// <example>
	/// [assembly: NonLockingAutoCounters(500, true, ThreadPriority.Lowest)]
	/// </example>
	/// <note>
	/// Multiple instance counters still acquire a lock when retrieving counters from the
	/// cache (with  <see cref="ICounterCache.Get(string,string)"/>.  To reduce contention 
	/// store the instance counter in a local variable rather than retrieving it from the 
	/// cache for every call.
	/// </note>
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class NonLockingAutoCountersAttribute : Attribute
	{
		/// <summary>
		/// Update interval in milliseconds for the background task or thread that flushes 
		/// the auto counter values.  The default is 500 milliseconds, the value must be
		/// greater than 1 millisecond.
		/// </summary>
		public int UpdateInterval { get; set; }

		/// <summary>
		/// Whether or not to use a dedicated thread for flushing auto counter values.  If false the 
		/// flushing of auto counter values is left to a repeating <see cref="Timer"/> task.  The
		/// default is true.   Set to false if the system has a limited number of threads available
		/// or if the system will rarely have a heavy load.  The timer task may be delayed when
		/// the system is under heavy load, the thread is more robust in that case (but can also 
		/// be delayed in flushing updates when under load).
		/// </summary>
		public bool UseDedicatedThread { get; set; }

		/// <summary>
		/// Only used if UseDedicatedThread is true.  Specifies the priority for the background
		/// thread.  The default is <see cref="ThreadPriority.BelowNormal"/>.
		/// </summary>
		public ThreadPriority DedicatedThreadPriority { get; set; }

		/// <summary>
		/// Constructor that assigns the update interval to the supplied value.
		/// </summary>
		/// <param name="interval">Update interval to use</param>
		/// <param name="useDedicated">Whether or not to use a dedicated thread</param>
		/// <param name="priority">Dedicated thread priority</param>
		public NonLockingAutoCountersAttribute(int interval = 500, 
												bool useDedicated = true, 
												ThreadPriority priority = ThreadPriority.BelowNormal)
		{
			if(interval < 1)
			{
				throw new ArgumentOutOfRangeException("interval",
											 "Update interval must be greater than 1 millisecond.");
			}
			UpdateInterval = interval;
			UseDedicatedThread = useDedicated;
			DedicatedThreadPriority = priority;
		}
	}
}
