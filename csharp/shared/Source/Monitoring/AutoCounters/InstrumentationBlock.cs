using System;
using System.Diagnostics;

namespace SharedAssemblies.Monitoring.AutoCounters
{
    /// <summary>
    /// struct that automatically enters and leaves a configuration block if used in an using context.
    /// Using struct here so that its garbage collection is immediate and deterministic.
    /// </summary>
    public struct InstrumentationBlock : IDisposable
    {
        /// <summary>
        /// Readonly property to get the counter
        /// </summary>
        public ICounter Counter { get; private set; }

		/// <summary>
		/// Start time for counters that track execution time.
		/// </summary>
    	internal long StartTime;

		/// <summary>
		/// Constructs an instrumentation block using a performance counter
		/// and automatically calls leave when disposed.
		/// </summary>
		/// <param name="counter">Performance counter to manage</param>
		public InstrumentationBlock(ICounter counter) :
			this(counter, false)
		{
		}

        /// <summary>
        /// Constructs an instrumentation block using a performance counter
        /// and automatically calls leave when disposed.
        /// </summary>
        /// <param name="counter">Performance counter to manage</param>
        /// <param name="startTimer">If true records the current time in the StartTime field.</param>
        public InstrumentationBlock(ICounter counter, bool startTimer) : 
            this()
        {
            if (counter != null)
            {
            	StartTime = startTimer ? Stopwatch.GetTimestamp() : 0;
                Counter = counter;
				// No longer makes calls to StartBlock().
            }
        }

		/// <summary>
        /// Leaves the instrumentation block immediately.
        /// </summary>
        public void End()
        {
            if (Counter != null)
            {
				Counter.EndBlock(this);
                Counter = null;
            }
        }

        /// <summary>
        /// Automatically exits instrumentation block on exit if not already left
        /// </summary>
        public void Dispose()
        {
            End();
        }
    }
}