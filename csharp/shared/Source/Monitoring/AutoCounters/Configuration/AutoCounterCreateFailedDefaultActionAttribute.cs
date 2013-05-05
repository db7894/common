using System;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// A custom assembly-level attribute that defines the default logger for the generic logger
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class AutoCounterCreateFailedDefaultActionAttribute : Attribute
    {
		/// <summary>
		/// Property to get/set the default logger
		/// </summary>
		public CreateFailedAction DefaultCreateFailedAction { get; set; }


		/// <summary>
		/// Constructs the attribute given the default logger type
		/// </summary>
		public AutoCounterCreateFailedDefaultActionAttribute() : this(CreateFailedAction.ThrowException)
		{
		}


		/// <summary>
        /// Constructs the attribute given the default logger type
        /// </summary>
        /// <param name="defaultLoggerType">The type of logger to use as a default</param>
        public AutoCounterCreateFailedDefaultActionAttribute(CreateFailedAction defaultLoggerType)
        {
            DefaultCreateFailedAction = defaultLoggerType;
        }
    }
}