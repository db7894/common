using System;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// An assembly level attribute that allows you to mark a heartbeat auto counter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AutoHeartbeatAttribute : AutoCounterAttribute
    {
		/// <summary>
		/// The number of milliseconds to update the last update heartbeat, if positive
		/// the heartbeat will be sent automatically, if negative, no heartbeat is sent.
		/// </summary>
		public long HeartbeatIntervalInMs { get; set; }


		/// <summary>
		/// <para>
		/// Marks a heartbeat autocounter to be installed with a category, name, and description.
		/// This auto counter will always be of type ElapsedTime.
		/// </para>
		/// <para>
		/// It should be noted that the category must either already exist or must be installed 
		/// using the AutoCounterCategoryToInstallAttribute or this installer attribute will fail.
		/// </para>
		/// <para>
		/// Each counter will be given a unique id that by default will be the Category:Name, however
		/// you can override this default name by setting the named argument UniqueName.
		/// </para>
		/// </summary>
		public AutoHeartbeatAttribute() : this(string.Empty, string.Empty, 5000)
		{
		}

	
		/// <summary>
        /// <para>
        /// Marks a heartbeat autocounter to be installed with a category, name, and description.
        /// This auto counter will always be of type ElapsedTime.
        /// </para>
        /// <para>
        /// It should be noted that the category must either already exist or must be installed 
        /// using the AutoCounterCategoryToInstallAttribute or this installer attribute will fail.
        /// </para>
        /// <para>
        /// Each counter will be given a unique id that by default will be the Category:Name, however
        /// you can override this default name by setting the named argument UniqueName.
        /// </para>
        /// </summary>
        /// <param name="category">Category that the autocounter will be installed under.</param>
        /// <param name="name">Name of the autocounter to install.</param>
        /// <param name="heartbeatIntervalInMs">The number of ms to update the heartbeat.</param>
        public AutoHeartbeatAttribute(string category, string name, long heartbeatIntervalInMs) :
            base(AutoCounterType.ElapsedTime, category, name)
        {
            HeartbeatIntervalInMs = heartbeatIntervalInMs;
        }
    }
}