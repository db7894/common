using System;

namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Extension class for DateTime
	/// </summary>
	public static class DateTimeExtensions
	{
		// constant for one day,
		private static readonly TimeSpan _oneDay = TimeSpan.FromDays(1.0);

		/// <summary>
		/// Formats the DateTime into the specified serialized form.
		/// </summary>
		/// <param name="date">DateTime parameter for extension</param>
		/// <param name="format">The format to serialize the DateTime to</param>
		/// <returns>DateTime serialized in the specified format</returns>
		/// <exception>throws ArgumentOutOfRangeException if DateTimeFormat is not in range.</exception>
		public static string ToString(this DateTime date, DateTimeFormat format)
		{
			switch (format)
			{
				case DateTimeFormat.DateTime:
					return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
				case DateTimeFormat.Date:
					return date.ToString("yyyy-MM-dd");
				case DateTimeFormat.Time:
					return date.ToString("HH:mm:ss.fff");
				case DateTimeFormat.HtmlDateTime:
					return date.ToString("MM/dd/yy hh:mm:ss tt");
				case DateTimeFormat.HtmlDate:
					return date.ToString("MM/dd/yy");
				case DateTimeFormat.HtmlTime:
					return date.ToString("hh:mm:ss tt");
				case DateTimeFormat.HtmlTimeNoSeconds:
					return date.ToString("hh:mm tt");
				case DateTimeFormat.NumericDate:
					return date.ToString("yyyyMMdd");
				case DateTimeFormat.NumericTime:
					return date.ToString("HHmmssfff");
				case DateTimeFormat.NumericTimeNoMilliseconds:
					return date.ToString("HHmmss");
				case DateTimeFormat.DisplayableDate:
					return date.ToString("MMMM dd, yyyy");
				case DateTimeFormat.MonthName:
					return date.ToString("MMMM");
				case DateTimeFormat.AbreviatedMonthName:
					return date.ToString("MMM");
				case DateTimeFormat.Month:
					return date.ToString("MM");
				case DateTimeFormat.Day:
					return date.ToString("dd");
				case DateTimeFormat.Year:
					return date.ToString("yyyy");
				default:
					throw new ArgumentOutOfRangeException("format");
			}
		}

		/// <summary>
		/// Gets the TimeOfDay from a null-able DateTime, or returns null if the null-able DateTime is null.
		/// </summary>
		/// <param name="nullableDate">The null-able DateTime to check.</param>
		/// <returns>TimeSpan for TimeOfDay if nullableDate is not null.</returns>
		public static TimeSpan? NullableTimeOfDay(this DateTime? nullableDate)
		{
			TimeSpan? result = null;

			if (nullableDate.HasValue)
			{
				result = nullableDate.Value.TimeOfDay;
			}

			return result;
		}

		/// <summary>
		/// Get day of week N days ago
		/// </summary>
		/// <param name="originalDate">DateTime parameter for extension</param>
		/// <param name="numDaysAgo">Number of days ago</param>
		/// <returns>Day of week</returns>
		public static DayOfWeek GetDayOfWeekNDaysAgo(this DateTime originalDate, int numDaysAgo)
		{
			var date = new DateTime(originalDate.Year, originalDate.Month, originalDate.Day);
			var daysAgo = new TimeSpan(numDaysAgo, 0, 0, 0);
			date -= daysAgo;
			return date.DayOfWeek;
		}

		/// <summary>
		/// Returns the minutes since midnight.
		/// </summary>
		/// <param name="dt">DateTime parameter for extension</param>
		/// <returns>Minutes since midnight</returns>
		public static double GetMinutesSinceMidnight(this DateTime dt)
		{
			var minutesSince = dt - dt.Date;
			return minutesSince.TotalMinutes;
		}

		/// <summary>
		/// Get how many days ago the given date was from .
		/// </summary>
		/// <param name="dt">DateTime parameter for extension</param>
		/// <param name="then">Previous date</param>
		/// <returns>Number of days ago</returns>
		public static double GetDaysSince(this DateTime dt, DateTime then)
		{
			// Give whole days
			var difference = dt.Date - then.Date;
			return difference.TotalDays;
		}

		/// <summary>
		/// Returns the minutes since the passed DateTime.
		/// </summary>
		/// <param name="dt">DateTime parameter for extension</param>
		/// <param name="then">Time to measure minutes since</param>
		/// <returns>Minutes since then</returns>
		public static double GetMinutesSince(this DateTime dt, DateTime then)
		{
			var minutesSince = dt - then;
			return minutesSince.TotalMinutes;
		}

		/// <summary>
		/// Returns the seconds since the passed DateTime.
		/// </summary>
		/// <param name="dt">DateTime parameter for extension</param>
		/// <param name="then">Time to measure seconds since</param>
		/// <returns>Seconds since then</returns>
		public static double GetSecondsSince(this DateTime dt, DateTime then)
		{
			var secondsSince = dt - then;
			return secondsSince.TotalSeconds;
		}

		/// <summary>
		/// Returns the hours since the passed DateTime.
		/// </summary>
		/// <param name="dt">DateTime parameter for extension</param>
		/// <param name="then">Time to measure hours since</param>
		/// <returns>Hours since then</returns>
		public static double GetHoursSince(this DateTime dt, DateTime then)
		{
			var hoursSince = dt - then;
			return hoursSince.TotalHours;
		}

		/// <summary>
		/// Calculates the time till midnight of the date-time passed in.
		/// </summary>
		/// <param name="dt">The DateTime parameter for the extension method.</param>
		/// <returns>TimeSpan from DateTime passed in until midnight.</returns>
		public static TimeSpan TimeUntilMidnight(this DateTime dt)
		{
			// use Date to get rid of time component, so it's midnight next day.
			return dt.AddDays(1.0).Date - dt;
		}

		/// <summary>
		/// Calculates the amount of time till a given time of day.  For example, if it's 2:00 pm and the time of day you
		/// want to get the interval till is 5:35 pm then the time till time of day will be 3 hrs, 35 min, 0 sec.
		/// </summary>
		/// <param name="dt">The current date and time.</param>
		/// <param name="timeOfDay">The time of day to measure time till.</param>
		/// <returns>The TimeSpan from DateTime till the next occurrence of TimeOfDay.</returns>
		public static TimeSpan TimeUntil(this DateTime dt, TimeSpan timeOfDay)
		{
			var currentTime = dt.TimeOfDay;

			return (currentTime < timeOfDay)
			       	? timeOfDay - currentTime
			       	: timeOfDay.Add(_oneDay) - currentTime;
		}
	}
}