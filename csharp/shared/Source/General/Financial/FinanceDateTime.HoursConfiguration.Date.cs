namespace SharedAssemblies.General.Financial
{
    /// <summary>
    /// Defined as a partial class to allow us to separate the FinanceDateTime.Configuration 
    /// into its own file for readability.
    /// </summary>
    public partial class FinanceDateTime
    {
        /// <summary>
        /// Defined as a partial class to allow us to separate FinanceDateTime.Configuration.Date
        /// into its own file for readability.
        /// </summary>
        public partial class HoursConfiguration
        {
			/// <summary>
            /// Class for year/month/day
            /// </summary>
            public struct Date
            {
				/// <summary>
				/// Year property
				/// </summary>
				public int Year { get; set; }


				/// <summary>
				/// Month property
				/// </summary>
				public int Month { get; set; }


				/// <summary>
				/// Day property
				/// </summary>
				public int Day { get; set; }


				/// <summary>
                /// Constructs a simple date structure
                /// </summary>
                /// <param name="year">Year of the date</param>
                /// <param name="month">Month of the date</param>
                /// <param name="day">Day of the date</param>
				public Date(int year, int month, int day)
					: this()
                {
                	Year = year;
                	Month = month;
                	Day = day;
                }
            }
        }
    }
}