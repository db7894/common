using System.Configuration;

namespace SharedAssemblies.General.Financial
{
	/// <summary>
	/// Configuration file section for FinanceDateTime configuration elements.
	/// </summary>
	public sealed class FinanceDateTimeConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// Gets or sets the path where the configuration file will be read from
		/// </summary>
		[ConfigurationProperty("pathOrFileName", DefaultValue = null, IsRequired = false)]
		public string PathOrFileName
		{
			get { return this["pathOrFileName"].ToString(); }
			set { this["pathOrFileName"] = value; }
		}
	}
}
