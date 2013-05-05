using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SharedAssemblies.Core.Xml;


namespace SharedAssemblies.General.Financial
{
	/// <summary>
	/// The FinanceDateTime class is a collection of business logic surrounding
	/// the Bashwork market days and hours.
	/// </summary>
	public partial class FinanceDateTime
	{
		/// <summary>The configuration file name to load for the hours and holidays</summary>
		private const string _defaultConfigurationFile = "FinanceDateTime.config.xml";

		/// <summary>The configuration section name for the FinanceDateTime configuration information.</summary>
		private const string _configurationSectionName = "financeDateTime";


		/// <summary>
		/// Property to contain DateTime
		/// </summary>
		public DateTime DateTimeStamp { get; set; }


		/// <summary>
		/// Property to contain Configuration
		/// </summary>
		public static HoursConfiguration Configuration { get; private set; }


		/// <summary>
		/// Static constructor to load configuration
		/// </summary>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException", 
			Justification = "We do not want any failure to configure to affect using logic.")]
		static FinanceDateTime()
		{
			try
			{
				var configFile = GetConfigurationFile();

				Configuration = XmlUtility.TypeFromXmlFile<HoursConfiguration>(configFile);
			}
			catch(Exception)
			{
				// if there were any problems, just load defaults
				Configuration = new HoursConfiguration(true);
			}
		}


		/// <summary>
		/// Constructs a FinanceDateTime using the current date and time for the 
		/// DateTimeStamp property.
		/// </summary>
		public FinanceDateTime()
		{
			DateTimeStamp = DateTime.Now;
		}


		/// <summary>
		/// Constructs a FinanceDateTime with DateTimeStamp property set to 
		/// the date-time stamp passed in.
		/// </summary>
		/// <param name="dateTime">DateTime of the object</param>
		public FinanceDateTime(DateTime dateTime)
		{
			DateTimeStamp = dateTime;
		}


		/// <summary>
		/// Constructor that sets the time to market open/close plus the deltaHours and deltaMinutes
		/// </summary>
		/// <param name="deltaHours">Hours to add</param>
		/// <param name="deltaMinutes">Minutes to add</param>
		/// <param name="type">Type: market open or market close</param>
		public FinanceDateTime(int deltaHours, int deltaMinutes, MarketTimeType type)
		{
			DateTimeStamp = DateTime.Now;
			if (type == MarketTimeType.MarketOpen)
			{
				SetToMarketOpen(deltaHours, deltaMinutes);
			}
			else
			{
				SetToMarketClose(deltaHours, deltaMinutes);
			}
		}


		/// <summary>
		/// Constructor taht sets time to market close plus the deltaHours and deltaMinutes
		/// </summary>
		/// <param name="deltaHours">Hours to add to market close.</param>
		/// <param name="deltaMinutes">Minutes to add to market close.</param>
		public FinanceDateTime(int deltaHours, int deltaMinutes)
			: this(deltaHours, deltaMinutes, MarketTimeType.MarketClose)
		{
		}


		/// <summary>
		/// Get date of New Years for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetNewYearsDay(int year)
		{
			var newYears = new DateTime(year, 1, 1);

			if (newYears.DayOfWeek == DayOfWeek.Sunday)
			{
				// if it falls on a Sunday, the market holiday will be on the second
				newYears = newYears.AddDays(1);
			}

			return newYears;
		}


		/// <summary>
		/// Get date of Martin Luther King Day for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetMartinLutherKingDay(int year)
		{
			// 3rd Monday of January - must be between 1/15 and 1/21
			return GetNextOccuranceOfWeekday(new DateTime(year, 1, 15), DayOfWeek.Monday);
		}


		/// <summary>
		/// Get date of Presidents Day for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetPresidentsDay(int year)
		{
			// 3rd Monday of February - must be between 2/15 and 2/21
			return GetNextOccuranceOfWeekday(new DateTime(year, 2, 15), DayOfWeek.Monday);
		}


		/// <summary>
		/// Get date of Good Friday for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetGoodFriday(int year)
		{
			// get Easter
			int c = year / 100;
			int n = year - 19 * (year / 19);
			int k = (c - 17) / 25;
			int i = c - c / 4 - (c - k) / 3 + 19 * n + 15;
			i = i - 30 * (i / 30);
			i = i - (i / 28) * (1 - (i / 28) * (29 / (i + 1)) * ((21 - n) / 11));
			int j = year + year / 4 + i + 2 - c + c / 4;
			j = j - 7 * (j / 7);
			int m = i - j;
			int easterMonth = 3 + (m + 40) / 44;
			int easterDay = m + 28 - 31 * (easterMonth / 4);

			var goodFriday = new DateTime(year, easterMonth, easterDay, 0, 0, 0).AddDays(-2);

			return goodFriday;
		}


		/// <summary>
		/// Get date of Memorial Day for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetMemorialDay(int year)
		{
			// 4th Monday in May - must be between 25 and 31
			return GetNextOccuranceOfWeekday(new DateTime(year, 5, 25), DayOfWeek.Monday);
		}


		/// <summary>
		/// Get date of Independence Day for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetIndependenceDay(int year)
		{
			return GetObservedHolidayDay(new DateTime(year, 7, 4));
		}


		/// <summary>
		/// Get date of Labor Day for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetLaborDay(int year)
		{
			// 1st Monday of September - must be between 1 and 7
			return GetNextOccuranceOfWeekday(new DateTime(year, 9, 1), DayOfWeek.Monday);
		}


		/// <summary>
		/// Get date of Thanksgiving for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetThanksgiving(int year)
		{
			// 3rd Thursday of November - must be between 22 and 28
			return GetNextOccuranceOfWeekday(new DateTime(year, 11, 22), DayOfWeek.Thursday);
		}


		/// <summary>
		/// Get date of Christmas for the given year
		/// </summary>
		/// <param name="year">Year to query</param>
		/// <returns>A DateTime that represents the holiday in the given year.</returns>
		public static DateTime GetChristmas(int year)
		{
			return GetObservedHolidayDay(new DateTime(year, 12, 25));
		}


		/// <summary>
		/// Returns the next market open from today
		/// </summary>
		/// <returns>A DateTime the next day and time that the market is open.</returns>
		public static DateTime GetNextMarketOpen()
		{
			return GetNextMarketOpen(DateTime.Now);
		}


		/// <summary>
		/// Returns the next market open from today
		/// </summary>
		/// <param name="fromDateTime">The DateTime to calculate next market open from.</param>
		/// <returns>A DateTime the next day and time that the market is open.</returns>
		public static DateTime GetNextMarketOpen(DateTime fromDateTime)
		{
			var time = new FinanceDateTime(fromDateTime);

			// if market has already opened today, we want to cycle forward and look to tomorrow.
			if (time.HasMarketOpened())
			{
				time.DateTimeStamp = time.DateTimeStamp.AddDays(1);
			}

			// set to market open of the specified day
			time.SetToMarketOpen(0, 0);

			// while we're not open, keep adding a day
			while (!time.IsMarketOpen())
			{
				time.DateTimeStamp = time.DateTimeStamp.AddDays(1);
			}

			return time.DateTimeStamp;
		}


		/// <summary>
		/// Returns days since last market day in relation to the passed date
		/// </summary>
		/// <param name="date">Date to count back from</param>
		/// <returns>Days since last market day from passed date</returns>
		public static int GetDaysSinceLastMarketDay(DateTime date)
		{
			var now = new FinanceDateTime(date);
			int days = 0;

			if (!now.IsMarketDay() || !now.HasMarketOpened())
			{
				now.DateTimeStamp = now.DateTimeStamp.AddDays(-1);
				days++;

				while (!now.IsMarketDay())
				{
					now.DateTimeStamp = now.DateTimeStamp.AddDays(-1);
					days++;
				}
			}

			return days;
		}


		/// <summary>
		/// Calculate and return the GTC expire date and time from the order date
		/// </summary>
		/// <param name="orderDate">Date of order</param>
		/// <param name="securityType">MarketCloseType of security</param>
		/// <returns>GTC expire date and time</returns>
		public static DateTime GetGtcExpirationFromOrderDate(DateTime orderDate, MarketCloseType securityType)
		{
			var nextMonth = orderDate.AddMonths(1);
			int numDaysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);

			// Starting with the last day of the month following the order date month,
			// decrement the day until we find the last market day of the month
			var gtcDateTime = new FinanceDateTime(
				new DateTime(nextMonth.Year, nextMonth.Month, numDaysInNextMonth));

			while (!gtcDateTime.IsMarketDay())
			{
				gtcDateTime.DateTimeStamp = gtcDateTime.DateTimeStamp.AddDays(-1);
			}

			// We have the expiration date, now set the expiration time
			gtcDateTime.DateTimeStamp = GetMarketCloseForDate(gtcDateTime.DateTimeStamp, securityType);

			return gtcDateTime.DateTimeStamp;
		}


		/// <summary>
		/// Calculate and return the market close time for the given date with the given security type
		/// </summary>
		/// <param name="dateTime">Date to use</param>
		/// <param name="securityType">MarketClosetype to use</param>
		/// <returns>The DateTime containing the proper market close time for given date and securityType</returns>
		public static DateTime GetMarketCloseForDate(DateTime dateTime, MarketCloseType securityType)
		{
			var closeTime = IsHalfDayHoliday(dateTime)
				 ? Configuration.HalfDayMarketClose
				 : Configuration.MarketClose;
			var close = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 
				closeTime.Hour, closeTime.Minute, 0);

			if (securityType == MarketCloseType.IndexSecurity)
			{
				close = close.AddMinutes(15);
			}

			return close;
		}


		/// <summary>
		/// Returns days since last market day in relation to the stored Date
		/// </summary>
		/// <returns>Days since last market day from stored date</returns>
		public int GetDaysSinceLastMarketDay()
		{
			return GetDaysSinceLastMarketDay(DateTimeStamp);
		}


		/// <summary>
		/// Returns true if then is the previous market day to the stored DateTime
		/// </summary>
		/// <param name="then">DateTime to query</param>
		/// <returns>True if the date given is the previous market day from the specified day.</returns>
		public bool IsLastMarketDay(DateTime then)
		{
			var lastMarketDay = new FinanceDateTime(DateTimeStamp);

			do
			{
				lastMarketDay.DateTimeStamp = lastMarketDay.DateTimeStamp.AddDays(-1);
			} 
			while (!lastMarketDay.IsMarketDay());

			return (then.Date == lastMarketDay.DateTimeStamp.Date);
		}


		/// <summary>
		/// Set the time to market close, plus the passed hours and minutes
		/// </summary>
		/// <param name="deltaHours">Hours to add</param>
		/// <param name="deltaMinutes">Minutes to add</param>
		public void SetToMarketClose(int deltaHours, int deltaMinutes)
		{
			var closeTime = IsHalfDayHoliday()
								? Configuration.HalfDayMarketClose
								: Configuration.MarketClose;

			DateTimeStamp = new DateTime(DateTimeStamp.Year, DateTimeStamp.Month, DateTimeStamp.Day, 
				closeTime.Hour, closeTime.Minute, 0)
					.AddMinutes(deltaMinutes)
					.AddHours(deltaHours);
		}


		/// <summary>
		/// Set the time to market open, plus the passed hours and minutes
		/// </summary>
		/// <param name="deltaHours">Hours to add</param>
		/// <param name="deltaMinutes">Minutes to add</param>
		public void SetToMarketOpen(int deltaHours, int deltaMinutes)
		{
			var openTime = Configuration.MarketOpen;

			DateTimeStamp = new DateTime(DateTimeStamp.Year, DateTimeStamp.Month, DateTimeStamp.Day, 
				openTime.Hour, openTime.Minute, 0)
					.AddMinutes(deltaMinutes)
					.AddHours(deltaHours);
		}


		/// <summary>
		/// Returns true if the date is a market day
		/// </summary>
		/// <returns>True if the instance DateTimeStamp is a market day.</returns>
		public bool IsMarketDay()
		{
			return DateTimeStamp.DayOfWeek != DayOfWeek.Sunday
				&& DateTimeStamp.DayOfWeek != DayOfWeek.Saturday
				&& !IsHoliday();
		}


		/// <summary>
		/// Returns true if within 3 seconds (high or low) of standard market close.
		/// This form that takes no parameters assumes the equity market.
		/// </summary>
		/// <returns>True if the property DateTimeStamp is near the market close.</returns>
		public bool IsNearMarketClose()
		{
			return IsNearMarketClose(MarketCloseType.Equity);
		}


		/// <summary>
		/// Returns true if within 3 seconds (high or low) of the market close time 
		/// associated with the MarketCloseType.
		/// </summary>
		/// <param name="closeType">The type of market to check for closing time.</param>
		/// <returns>True if the property DateTimeStamp is near the market close.</returns>
		public bool IsNearMarketClose(MarketCloseType closeType)
		{
			const int lagPeriod = 3;

			if (IsMarketDay())
			{
				var close = IsHalfDayHoliday()
								? Configuration.HalfDayMarketClose
								: Configuration.MarketClose;

				var highTime = new DateTime(DateTimeStamp.Year, DateTimeStamp.Month, DateTimeStamp.Day,
											close.Hour, close.Minute, 0);

				// Index securities (including index options) close 15 minutes later
				if (closeType == MarketCloseType.IndexSecurity)
				{
					highTime = highTime.AddMinutes(15);
				}

				// low time is 3 seconds before close
				var lowTime = highTime.AddSeconds(-lagPeriod);

				// high time is 3 seconds after close
				highTime = highTime.AddSeconds(lagPeriod);

				// check if in low-high period
				if (DateTimeStamp >= lowTime && DateTimeStamp <= highTime)
				{
					return true;
				}
			}

			// not a market day or not in low-high period
			return false;
		}


		/// <summary>
		/// Returns true if after market open and before market close for the equity market close.
		/// </summary>
		/// <returns>True if the property DateTimeStamp is during market open hours.</returns>
		public bool IsMarketOpen()
		{
			return IsMarketOpen(MarketCloseType.Equity);
		}


		/// <summary>
		/// Returns true if after market open and before market close for the given market type.
		/// </summary>
		/// <param name="closeType">The type of market to check for closing hours.</param>
		/// <returns>True if the property DateTimeStamp is during market open hours.</returns>
		public bool IsMarketOpen(MarketCloseType closeType)
		{
			return (IsMarketDay() && HasMarketOpened() && !HasMarketClosed(closeType));
		}


		/// <summary>
		/// Returns true if the time is at least the passed number of minutes past market open
		/// </summary>
		/// <param name="minutesPast">Minutes past market open</param>
		/// <returns>True if the property DateTimeStamp is the specified number of minutes past open.</returns>
		public bool IsPastMarketOpen(int minutesPast)
		{
			var timeToCompare = new FinanceDateTime(DateTimeStamp);
			timeToCompare.SetToMarketOpen(0, minutesPast);

			if (HasMarketOpened())
			{
				return DateTimeStamp >= timeToCompare.DateTimeStamp;
			}

			return false;
		}


		/// <summary>
		/// Returns true if the extended hours market is open
		/// </summary>
		/// <returns>True if the property DateTimeStamp is in the extended hours.</returns>
		public bool IsExtendedHoursMarketOpen()
		{
			return (IsMarketDay() && HasExtendedHoursOpened() && !HasExtendedHoursClosed());
		}


		/// <summary>
		/// Returns true if the stored date is a market holiday
		/// </summary>
		/// <returns>True if the property DateTimeStamp is on a holiday.</returns>
		public bool IsHoliday()
		{
			return IsHoliday(DateTimeStamp);
		}


		/// <summary>
		/// Returns true if the stored date matches the next holiday half day
		/// </summary>
		/// <returns>True if the property DateTimeStamp is a market half-day.</returns>
		public bool IsHalfDayHoliday()
		{
			return IsHalfDayHoliday(DateTimeStamp);
		}


		/// <summary>
		/// Returns true if during the timeframe that does not include pre-market.
		/// </summary>
		/// <returns>True if the property DateTimeStamp is during the wide-spread time.</returns>
		public bool IsDuringWideSpread()
		{
			// if the market is not open either due to hours or it's a non market day
			if (!IsMarketOpen())
			{
				// if this IS a market day and we're in between 15 minutes prior to market open 
				// and the market open
				if (IsMarketDay() &&
					DateTimeStamp.Hour == Configuration.MarketOpen.Hour &&
					DateTimeStamp.Minute >= Configuration.MarketOpen.Minute - 15) 					
				{
					// Changing this because wide spread isn't really tied to premarket 
					// but rather a business rule that decides wide spread ends 15 minutes before market
					// open these times used to coincide but no longer is this the case
					return false;
				}
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if the date and time meet all GTD criteria in respect to the current date and time.
		/// </summary>
		/// <returns>True if the property DateTimeStamp is a valid GTD expiration date and time</returns>
		public bool IsValidGtdExpiration()
		{
			bool doesMeet = true;

			// check to see if the GTD has passed
			if (DateTimeStamp <= DateTime.Now)
			{
				doesMeet = false;
			}

			// make sure <= 90 days from now
			if (DateTimeStamp > DateTime.Now.AddDays(90))
			{
				doesMeet = false;
			}

			// make sure it's a market day/time
			if (!IsValidGtdTime())
			{
				doesMeet = false;
			}

			return doesMeet;
		}


		/// <summary>
		/// Returns true if the market has past market open but does not care if the market has closed.
		/// This is in contrast to IsMarketOpen which check to see if market is currently open.
		/// </summary>
		/// <returns>True if the property DateTimeStamp is past market open.</returns>
		public bool HasMarketOpened()
		{
			var openTime = Configuration.MarketOpen;

			// use configurable settings instead of hard-coded 9:30
			return (DateTimeStamp.Hour > openTime.Hour) ||
				   (DateTimeStamp.Hour == openTime.Hour && DateTimeStamp.Minute >= openTime.Minute);
		}


		/// <summary>
		/// Returns true if the market has closed for standard securities.
		/// </summary>
		/// <returns>Returns true if the property DateTimeStamp is part the market close time.</returns>
		public bool HasMarketClosed()
		{
			return HasMarketClosed(MarketCloseType.Equity);
		}


		/// <summary>
		/// Returns true if the market has closed for the given MarketCloseType
		/// </summary>
		/// <param name="closeType">The type of market to check for closing hours.</param>
		/// <returns>Returns true if the property DateTimeStamp is past the market close.</returns>
		public bool HasMarketClosed(MarketCloseType closeType)
		{
			DateTime marketCloseTime = GetMarketCloseForDate(DateTimeStamp, closeType);

			return DateTimeStamp >= marketCloseTime;
		}


		/// <summary>
		/// Returns true if the extended hours market has closed
		/// </summary>
		/// <returns>True if the property DateTimeStamp is past the extended hours close.</returns>
		public bool HasExtendedHoursClosed()
		{
			var extClose = IsHalfDayHoliday()
							   ? Configuration.HalfDayExtendedMarketClose
							   : Configuration.ExtendedMarketClose;

			return (DateTimeStamp.Hour > extClose.Hour) ||
				   (DateTimeStamp.Hour == extClose.Hour && DateTimeStamp.Minute >= extClose.Minute);
		}


		/// <summary>
		/// Returns true if the extended hours market has already opened but is irrespective of 
		/// whether it has closed for the day.  This is in contrast to the IsExtendedHoursOpen()
		/// method that checks to see if extended hours are currently open.
		/// </summary>
		/// <returns>True if the DateTimeStamp property is past extended hours open.</returns>
		public bool HasExtendedHoursOpened()
		{
			var extOpen = IsHalfDayHoliday()
							  ? Configuration.HalfDayExtendedMarketOpen
							  : Configuration.ExtendedMarketOpen;

			return (DateTimeStamp.Hour > extOpen.Hour) ||
				   (DateTimeStamp.Hour == extOpen.Hour && DateTimeStamp.Minute >= extOpen.Minute);
		}


		/// <summary>
		/// Returns true if the pre-market hours have closed
		/// </summary>
		/// <returns>True if the property DateTimeStamp is past the pre-market close.</returns>
		public bool HasPreMarketClosed()
		{
			var preMarketClose = Configuration.PreMarketClose;

			return (DateTimeStamp.Hour > preMarketClose.Hour) 
				|| (DateTimeStamp.Hour == preMarketClose.Hour 
				   && DateTimeStamp.Minute >= preMarketClose.Minute);
		}


		/// <summary>
		/// Returns true if the pre-market hours have opened irrespective of the pre-market close.  This
		/// is in contrast to the IsPreMarketOpen() method which checks to see if the pre-market
		/// is currently open.
		/// </summary>
		/// <returns>True if the DateTimeStamp property is past the pre-market open.</returns>
		public bool HasPreMarketOpened()
		{
			var preMarketOpen = Configuration.PreMarketOpen;

			return (DateTimeStamp.Hour > preMarketOpen.Hour) 
				|| (DateTimeStamp.Hour == preMarketOpen.Hour 
					&& DateTimeStamp.Minute >= preMarketOpen.Minute);
		}


		/// <summary>
		/// Returns true if the pre-market hours are currently open.
		/// </summary>
		/// <returns>True if the DateTimeStamp property is during the pre-market hours.</returns>
		public bool IsPreMarketOpen()
		{
			return (IsMarketDay() && HasPreMarketOpened() && !HasPreMarketClosed());
		}


		/// <summary>
		/// Returns true if the time is 15 minutes or less to market open
		/// </summary>
		/// <returns>True if the DateTimeStamp property is within 15 minutes of market open.</returns>
		public bool IsWithinFifteenMinutesToOpen()
		{
			if (IsMarketDay() && !HasMarketOpened())
			{
				int? minutesToOpen = GetMinutesToMarketOpen();

				if (minutesToOpen.HasValue && minutesToOpen.Value < 15 && minutesToOpen.Value > 0)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Return minutes to market open in comparison to stored time or null if the market has opened
		/// or it is not a market day.
		/// </summary>
		/// <returns>Returns the number of minutes from DateTimeStamp property to market open.</returns>
		public int? GetMinutesToMarketOpen()
		{
			if (IsMarketDay() && !HasMarketOpened() && !HasMarketClosed())
			{
				var openTime = Configuration.MarketOpen;

				return (60 * openTime.Hour + openTime.Minute) 
					- (DateTimeStamp.Hour * 60 + DateTimeStamp.Minute);
			}

			return null;
		}


		/// <summary>
		/// Return minutes to market close in comparison to stored time or null if the market has closed or
		/// is not a market day.
		/// </summary>
		/// <returns>Returns the number of minutes from the DateTimeStamp property to the market close.</returns>
		public int? GetMinutesToMarketClose()
		{
			if (IsMarketDay() && HasMarketOpened() && !HasMarketClosed())
			{
				var closeTime = IsHalfDayHoliday()
									? Configuration.HalfDayMarketClose
									: Configuration.MarketClose;

				return (60 * closeTime.Hour + closeTime.Minute) 
					- (DateTimeStamp.Hour * 60 + DateTimeStamp.Minute);
			}

			return null;
		}

		/// <summary>
		/// Returns true if the the date and time are valid for GTD
		/// </summary>
		/// <returns>Determines if the time component of DateTimeStamp is valid for GTDs.</returns>
		private bool IsValidGtdTime()
		{
			bool isValid = false;

			var closeTime = IsHalfDayHoliday()
								? Configuration.HalfDayMarketClose
								: Configuration.MarketClose;

			// if it's a market day during open hours, check closing,
			// we can't use !HasMarketClosed() because of the strict < operator
			if (IsMarketDay() && HasMarketOpened())
			{
				isValid = (DateTimeStamp.Hour < closeTime.Hour 
					|| (DateTimeStamp.Hour == closeTime.Hour && DateTimeStamp.Minute <= closeTime.Minute));

				if (DateTime.Now > DateTimeStamp)
				{
					isValid = false;
				}

				// check to make sure time minutes on the hour or half hour
				if (DateTimeStamp.Minute != 0 && DateTimeStamp.Minute != 30)
				{
					isValid = false;
				}
			}

			return isValid;
		}


		/// <summary>
		/// Returns true if the passed parameters correspond to New Years Day
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsNewYearsDay(DateTime dt)
		{
			return (dt.Date == GetNewYearsDay(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Martin Luther King Day
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsMartinLutherKingDay(DateTime dt)
		{
			return (dt.Date == GetMartinLutherKingDay(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Presidents Day
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsPresidentsDay(DateTime dt)
		{
			return (dt.Date == GetPresidentsDay(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Good Friday
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsGoodFriday(DateTime dt)
		{
			return (dt.Date == GetGoodFriday(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Memorial Day
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsMemorialDay(DateTime dt)
		{
			return (dt.Date == GetMemorialDay(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Independence Day
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsIndependenceDay(DateTime dt)
		{
			return (dt.Date == GetIndependenceDay(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Labor Day
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsLaborDay(DateTime dt)
		{
			return (dt.Date == GetLaborDay(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Thanksgiving
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsThanksgiving(DateTime dt)
		{
			return (dt.Date == GetThanksgiving(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to Christmas
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsChristmas(DateTime dt)
		{
			return (dt.Date == GetChristmas(dt.Year).Date);
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to a market holiday
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsHoliday(DateTime dt)
		{
			return IsNewYearsDay(dt) ||
				   IsMartinLutherKingDay(dt) ||
				   IsPresidentsDay(dt) ||
				   IsGoodFriday(dt) ||
				   IsMemorialDay(dt) ||
				   IsIndependenceDay(dt) ||
				   IsLaborDay(dt) ||
				   IsThanksgiving(dt) ||
				   IsChristmas(dt) ||
				   IsSpecialHoliday(dt);
		}

		/// <summary>
		/// Returns true if the passed parameters match the next holiday half day
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsHalfDayHoliday(DateTime dt)
		{
			foreach (HoursConfiguration.Date halfDay in Configuration.HalfDays)
			{
				if (halfDay.Year == dt.Year && halfDay.Month == dt.Month && halfDay.Day == dt.Day)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if the passed parameters correspond to a special holiday
		/// </summary>
		/// <param name="dt">The date to check</param>
		/// <returns>True if the date specified is the specified holiday</returns>
		private static bool IsSpecialHoliday(DateTime dt)
		{
			foreach (HoursConfiguration.Date specialDay in Configuration.SpecialHolidays)
			{
				if (specialDay.Year == dt.Year && specialDay.Month == dt.Month && specialDay.Day == dt.Day)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns the observed date of the passed holiday
		/// </summary>
		/// <param name="holiday">The date to check</param>
		/// <returns>True if the date specified is an observed holiday</returns>
		private static DateTime GetObservedHolidayDay(DateTime holiday)
		{
			DateTime observed = holiday;
			if (observed.DayOfWeek == DayOfWeek.Sunday)
			{
				observed = observed.AddDays(1);
			}
			else if (observed.DayOfWeek == DayOfWeek.Saturday)
			{
				observed = observed.AddDays(-1);
			}
			return observed;
		}

		/// <summary>
		/// Returns the next occurrence of the weekday on or after the start date
		/// </summary>
		/// <param name="start">The date to start from</param>
		/// <param name="weekday">The day of week to find next occurrence of</param>
		/// <returns>The next instance of the day of the week from the start date</returns>
		private static DateTime GetNextOccuranceOfWeekday(DateTime start, DayOfWeek weekday)
		{
			DateTime desired = start;

			while (desired.DayOfWeek != weekday)
			{
				desired = desired.AddDays(1);
			}

			return desired;
		}

		/// <summary>
		/// Helper method to get the configuration file information from the configuration section.
		/// </summary>
		/// <returns>Path to the configuration file.</returns>
		private static string GetConfigurationFile()
		{
			try
			{
				// attempt to get the section and treat it as a FinanceDateTimeConfigurationSection
				var section = ConfigurationManager.GetSection(_configurationSectionName) as FinanceDateTimeConfigurationSection;

				if (section != null)
				{
					var path = section.PathOrFileName;

					// if the path given is a directory, then append the config file to the path.
					if (Directory.Exists(path))
					{
						return Path.Combine(path, _defaultConfigurationFile);
					}

					// otherwise, just return the path since it's not a directory, it should be a path to a full file.
					return path;
				}
			}

			catch (Exception)
			{
				// if any exception occurs due to configuration or file IO, just consume and return basic config file.
			}

			// if anything fails, return the default.
			return _defaultConfigurationFile;
		}
	}
}