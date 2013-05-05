using System.Collections.Generic;
using System.Xml.Serialization;


namespace SharedAssemblies.General.Financial
{
    /// <summary>
    /// Defined as a partial class to allow us to separate the FinanceDateTime.HoursConfiguration 
    /// into its own file for readability.
    /// </summary>
    public partial class FinanceDateTime
    {
        /// <summary>
        /// HoursConfiguration class for FinanceDateTime.  Cleaned up as an inner class so not in the
        /// global scope. Added an XmlRoot so that the new name does not hurt old config files.
        /// </summary>
        [XmlRoot("FinanceDateTimeConfiguration")]
        public partial class HoursConfiguration
        {
			/// <summary>
			/// List of days that the market closes early
			/// </summary>
			public List<Date> HalfDays { get; set; }

			/// <summary>
			/// List of days that the market is closed, that aren't normal holidays
			/// </summary>
			public List<Date> SpecialHolidays { get; set; }

			/// <summary>
			/// Time of market close on half days
			/// </summary>
			public Time HalfDayMarketClose { get; set; }

			/// <summary>
			/// Time of extended market open on half days
			/// </summary>
			public Time HalfDayExtendedMarketOpen { get; set; }

			/// <summary>
			/// Time of extended market close on half days
			/// </summary>
			public Time HalfDayExtendedMarketClose { get; set; }

			/// <summary>
			/// Time of pre-market open
			/// </summary>
			public Time PreMarketOpen { get; set; }

			/// <summary>
			/// Time of pre-market close
			/// </summary>
			public Time PreMarketClose { get; set; }

			/// <summary>
			/// Time of market open
			/// </summary>
			public Time MarketOpen { get; set; }

			/// <summary>
			/// Time of market close on standard days
			/// </summary>
			public Time MarketClose { get; set; }

			/// <summary>
			/// Time of extended market open on standard days
			/// </summary>
			public Time ExtendedMarketOpen { get; set; }

			/// <summary>
			/// Time of extended market close on standard days
			/// </summary>
			public Time ExtendedMarketClose { get; set; }


			/// <summary>
            /// Default constructor - values not initialized
            /// </summary>
            public HoursConfiguration()
            {
                LoadFromDefaults();
            }

            /// <summary>
            /// Default constructor - loads default values if specified
            /// </summary>
            /// <param name="loadFromDefaults">Bool designating whether or not to load defaults</param>
            public HoursConfiguration(bool loadFromDefaults)
            {
                if (loadFromDefaults)
                {
                    LoadFromDefaults();
                }
            }

            /// <summary>
            /// Return the number of minutes the market is open on a half day
            /// </summary>
            /// <returns>The number of minutes market open on half day</returns>
            public double GetMinutesMarketIsOpenOnHalfDay()
            {
                return (HalfDayMarketClose.Hour * 60 + HalfDayMarketClose.Minute) -
                       (MarketOpen.Hour * 60 + MarketOpen.Minute);
            }

            /// <summary>
            /// Return the number of minutes the market is closed on a half day
            /// </summary>
            /// <returns>Number of minutes market closed on half day</returns>
            public double GetMinutesMarketIsClosedOnHalfDay()
            {
                return (MarketClose.Hour * 60 + MarketClose.Minute) -
                       (HalfDayMarketClose.Hour * 60 + HalfDayMarketClose.Minute);
            }


            /// <summary>
            /// Load property values from defaults
            /// </summary>
            private void LoadFromDefaults()
            {
                HalfDayMarketClose = new Time(13, 0);
                HalfDayExtendedMarketOpen = new Time(13, 0);
                HalfDayExtendedMarketClose = new Time(17, 0);
                PreMarketOpen = new Time(7, 0);
                PreMarketClose = new Time(9, 30);
                MarketOpen = new Time(9, 30);
                MarketClose = new Time(16, 0);
                ExtendedMarketOpen = new Time(16, 0);
                ExtendedMarketClose = new Time(20, 0);
                HalfDays = new List<Date>();
                SpecialHolidays = new List<Date>();
            }
        }
    }
}