using System;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
	/// <summary>
	/// An assembly level attribute that allows you to mark auto counter categories for installation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class AutoCounterCollectionAttribute : Attribute
	{
		/// <summary>
		/// The unique name of the collection.
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// A list of the unique names (counter name by default) of the counters to include.  Must
		/// be unique names of counters, not categories.  Also may not contain the name of a multi-
		/// instance counter if this is a single-instance collection.
		/// </summary>
		public string[] AutoCounters { get; set; }


		/// <summary>
		/// The type of instance this collection supports.  Multi-Instance collections can be instantiated
		/// once per instance and typically contain instanced counters but can also contain non-instanced
		/// counters.  However, Single-Instance collections cannot contain multi-instance counters.
		/// </summary>
		public InstanceType InstanceType { get; set; }


		/// <summary>
		/// The unique name of the parent group, null/empty means there is no parent group.  It should
		/// be noted that if this is a single-instance collection, it cannot have a parent that is a
		/// multi-instance collection.
		/// </summary>
		public string ParentCollection { get; set; }


		/// <summary>
		/// <para>
		/// Creates a collection to logically group counters that tend to get incremented together.  For
		/// example, perhaps you want to determine how many orders are placed, how long each order takes 
		/// on average, and how many orders per second you are placing.  The three of these counters could
		/// be grouped together so that only the group needs to be incremented.
		/// </para>
		/// <para>
		/// Multi-Instance should only be used in those places where it makes sense (like tracking a stat per
		/// CPU or per thread).  The vast majority of the time, you only want or need single instances.
		/// The default is a SingleInstance collection, but may be overridden with the named argument.
		/// </para>
		/// </summary>
		public AutoCounterCollectionAttribute() : this(string.Empty, null)
		{
		}

	
		/// <summary>
		/// <para>
		/// Creates a collection to logically group counters that tend to get incremented together.  For
		/// example, perhaps you want to determine how many orders are placed, how long each order takes 
		/// on average, and how many orders per second you are placing.  The three of these counters could
		/// be grouped together so that only the group needs to be incremented.
		/// </para>
		/// <para>
		/// Multi-Instance should only be used in those places where it makes sense (like tracking a stat per
		/// CPU or per thread).  The vast majority of the time, you only want or need single instances.
		/// The default is a SingleInstance collection, but may be overridden with the named argument.
		/// </para>
		/// </summary>
		/// <param name="collectionUniqueName">Unique name for the collection.</param>
		/// <param name="counterUniqueNames">Unique names for the included counters.</param>
		public AutoCounterCollectionAttribute(string collectionUniqueName, params string[] counterUniqueNames)
		{
			Name = collectionUniqueName;
			InstanceType = InstanceType.SingleInstance;
			AutoCounters = counterUniqueNames ?? new string[0];
		}
	}
}