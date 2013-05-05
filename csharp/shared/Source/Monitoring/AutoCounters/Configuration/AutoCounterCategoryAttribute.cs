using System;

namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
    /// <summary>
    /// An assembly level attribute that allows you to mark auto counter categories for installation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AutoCounterCategoryAttribute : Attribute
    {
        /// <summary>
        /// The name of the category to install
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if there are more than one instance of this category allowed.
        /// </summary>
        public InstanceType InstanceType { get; set; }
        
        /// <summary>
        /// The description of the category of counters to install.
        /// </summary>
        public string Description { get; set; }


		/// <summary>
		/// <para>
		/// Constructs an auto counter category for installation.  If the category already exists, it will 
		/// not be recreated.  If you need to change the multi-instance of a category, you must uninstall it
		/// first and then reinstall it.
		/// </para>
		/// <para>
		/// Multi-Instance should only be true in those places where it makes sense (like tracking a stat per
		/// CPU or per thread).  The vast majority of the time, you only want or need single instances.  The
		/// default is a single instance category, but may be overridden with the named argument.
		/// </para>
		/// </summary>
		public AutoCounterCategoryAttribute() : this(string.Empty)
		{
		}


		/// <summary>
		/// <para>
		/// Constructs an auto counter category for installation.  If the category already exists, it will 
		/// not be recreated.  If you need to change the multi-instance of a category, you must uninstall it
		/// first and then reinstall it.
		/// </para>
		/// <para>
		/// Multi-Instance should only be true in those places where it makes sense (like tracking a stat per
		/// CPU or per thread).  The vast majority of the time, you only want or need single instances.  The
		/// default is a single instance category, but may be overridden with the named argument.
		/// </para>
		/// </summary>
		/// <param name="name">Name of the auto counter category to install.</param>
		public AutoCounterCategoryAttribute(string name)
		{
			InstanceType = InstanceType.SingleInstance;
			Name = name;
			Description = name;
		}
	}
}