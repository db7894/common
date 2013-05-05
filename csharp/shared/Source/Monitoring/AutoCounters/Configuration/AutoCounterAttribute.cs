using System;
using SharedAssemblies.Core.Conversions;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
	/// <summary>
	/// An assembly level attribute that allows you to mark auto counters for installation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class AutoCounterAttribute : Attribute
	{
		// translator for counter type to units.
		private static readonly Translator<AutoCounterType, string> _typeTranslator = new Translator<AutoCounterType, string>("?")
			{
				{ AutoCounterType.AverageTime, "sec" },
				{ AutoCounterType.CountsPerSecond, "rate" },
				{ AutoCounterType.ElapsedTime, "sec" },
				{ AutoCounterType.RollingAverageTime, "sec" },
				{ AutoCounterType.TotalCount, "items" },
			};

		// name used for displaying in a column, should be abbreviated
		private string _shortName;

		/// <summary>
		/// The type of the autocounter to install.
		/// </summary>
		public AutoCounterType AutoCounterType { get; set; }

		/// <summary>
		/// The category to install the auto counter under, this category must exist or also be
		/// installed using the AutoCounterCategoryToInstallAttribute
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// The name of the autocounter.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Can be any unique identifier that separates other counters of similar name,
		/// the default UniqueId is the Category:Name combination, which should be unique.  
		/// </summary>
		public string UniqueName { get; set; }

		/// <summary>
		/// A description for what the autocounter's purpose is.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// A description of the units in which this counter is measured.  This is purely informational and not 
		/// part of the underlying performance counter.  Applications can query this for informational purposes.
		/// </summary>
		public string Units { get; set; }

		/// <summary>
		/// Sets the abbreviated name of the auto counter.  This is a very short-name that can be used for column
		/// views, etc.  If AbbreviatedName is null, then UniqueName is returned.
		/// </summary>
		public string AbbreviatedName
		{
			get { return _shortName ?? UniqueName; }
			set { _shortName = value; }
		}

		/// <summary>
		/// True if the counter is to be used in a read-only manner.
		/// </summary>
		public bool IsReadOnly { get; set; }

		/// <summary>
		/// <para>
		/// Marks an autocounter to be installed with a type, category, name, and description.
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
		public AutoCounterAttribute() : this(AutoCounters.AutoCounterType.Unknown, string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// <para>
		/// Marks an autocounter to be installed with a type, category, name, and description.
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
		/// <param name="type">Type of the autocounter to install.</param>
		/// <param name="category">Category that the autocounter will be installed under.</param>
		/// <param name="name">Name of the autocounter to install.</param>
		public AutoCounterAttribute(AutoCounterType type, string category, string name)
		{
			AutoCounterType = type;
			Category = category;
			Name = name;

			// description defaulted to same as name unless manually changed.
			Description = name;

			// unique name is Category:Name unless manually changed.
			UniqueName = category + ':' + name;

			// gets the appropriate default units for this counter type.
			Units = _typeTranslator[type];
		}
	}
}