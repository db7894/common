using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// This is a test class for DateTimeExtensionsTest and is intended
	/// to contain all DateTimeExtensionsTest Unit Tests
	/// </summary>
	[TestClass]
	public class DateTimeExtensionsTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// A test for SerializeYear
		/// </summary>
		[TestMethod]
		public void SerializeYearTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "2011";
			string actual = dt.ToString(DateTimeFormat.Year);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeTime
		/// </summary>
		[TestMethod]
		public void SerializeTimeTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "08:07:00.000";
			string actual = dt.ToString(DateTimeFormat.Time);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeShortNumericTime
		/// </summary>
		[TestMethod]
		public void SerializeShortNumericTimeTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "080700";
			string actual = dt.ToString(DateTimeFormat.NumericTimeNoMilliseconds);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeNumericTime
		/// </summary>
		[TestMethod]
		public void SerializeNumericTimeTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "080700000";
			string actual = dt.ToString(DateTimeFormat.NumericTime);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeNumericMonth
		/// </summary>
		[TestMethod]
		public void SerializeNumericMonthTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "10";
			string actual = dt.ToString(DateTimeFormat.Month);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeNumericDay
		/// </summary>
		[TestMethod]
		public void SerializeNumericDayTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "30";
			string actual = dt.ToString(DateTimeFormat.Day);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeNumericDate
		/// </summary>
		[TestMethod]
		public void SerializeNumericDateTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "20111030";
			string actual = dt.ToString(DateTimeFormat.NumericDate);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeMonth
		/// </summary>
		[TestMethod]
		public void SerializeMonthTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "October";
			string actual = dt.ToString(DateTimeFormat.MonthName);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeHtmlTimeNoSeconds
		/// </summary>
		[TestMethod]
		public void SerializeHtmlTimeNoSecondsTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "08:07 AM";
			string actual = dt.ToString(DateTimeFormat.HtmlTimeNoSeconds);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeHtmlTime
		/// </summary>
		[TestMethod]
		public void SerializeHtmlTimeTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "08:07:00 AM";
			string actual = dt.ToString(DateTimeFormat.HtmlTime);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeHtmlDateTime
		/// </summary>
		[TestMethod]
		public void SerializeHtmlDateTimeTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "10/30/11 08:07:00 AM";
			string actual = dt.ToString(DateTimeFormat.HtmlDateTime);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeHtmlDate
		/// </summary>
		[TestMethod]
		public void SerializeHtmlDateTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "10/30/11";
			string actual = dt.ToString(DateTimeFormat.HtmlDate);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeDate
		/// </summary>
		[TestMethod]
		public void SerializeDateTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "2011-10-30";
			string actual = dt.ToString(DateTimeFormat.Date);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeCurrentTimeNoSeconds
		/// </summary>
		[TestMethod]
		public void SerializeCurrentTimeNoSecondsTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = DateTime.Now.ToString("hh:mm tt");
			string actual = DateTime.Now.ToString(DateTimeFormat.HtmlTimeNoSeconds);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeCurrentTime
		/// </summary>
		[TestMethod]
		public void SerializeCurrentTimeTest()
		{
			DateTime dt = DateTime.Now;
			string expected = DateTime.Now.ToString("hh:mm:ss tt");
			string actual = DateTime.Now.ToString(DateTimeFormat.HtmlTime);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeCurrentDate
		/// </summary>
		[TestMethod]
		public void SerializeCurrentDateTest()
		{
			DateTime dt = DateTime.Now;
			string expected = DateTime.Now.ToString("yyyyMMdd");
			string actual = dt.ToString(DateTimeFormat.NumericDate);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SerializeAbbrMonth
		/// </summary>
		[TestMethod]
		public void SerializeAbbrMonthTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "Oct";
			string actual = dt.ToString(DateTimeFormat.AbreviatedMonthName);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for Serialize
		/// </summary>
		[TestMethod]
		public void SerializeTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "2011-10-30 08:07:00.000";
			string actual = dt.ToString(DateTimeFormat.DateTime);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for SecondsSince
		/// </summary>
		[TestMethod]
		public void SecondsSinceTest()
		{
			DateTime dt = new DateTime(2008, 10, 30, 8, 7, 0);
			DateTime then = new DateTime(2008, 10, 30, 8, 6, 0);

			Assert.AreEqual<double>(60, dt.GetSecondsSince(then));
		}

		/// <summary>
		/// A test for MinutesSinceMidnight
		/// </summary>
		[TestMethod]
		public void MinutesSinceMidnightTest()
		{
			DateTime dt = new DateTime(2008, 10, 30, 0, 7, 0);

			Assert.AreEqual<double>(7, dt.GetMinutesSinceMidnight());
		}

		/// <summary>
		/// A test for MinutesSince
		/// </summary>
		[TestMethod]
		public void MinutesSinceTest()
		{
			DateTime dt = new DateTime(2008, 10, 30, 8, 7, 0);
			DateTime then = new DateTime(2008, 10, 30, 8, 6, 0);

			Assert.AreEqual<double>(1, dt.GetMinutesSince(then));
		}

		/// <summary>
		/// A test for HoursSince
		/// </summary>
		[TestMethod]
		public void HoursSinceTest()
		{
			DateTime dt = new DateTime(2008, 10, 30, 8, 7, 0);
			DateTime then = new DateTime(2008, 10, 30, 6, 7, 0);

			Assert.AreEqual<double>(2, dt.GetHoursSince(then));
		}

		/// <summary>
		/// A test for GetNameOfMonth
		/// </summary>
		[TestMethod]
		public void GetNameOfMonthTest()
		{
			Assert.AreEqual("January", new DateTime(2008, 1, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("February", new DateTime(2008, 2, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("March", new DateTime(2008, 3, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("April", new DateTime(2008, 4, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("May", new DateTime(2008, 5, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("June", new DateTime(2008, 6, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("July", new DateTime(2008, 7, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("August", new DateTime(2008, 8, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("September", new DateTime(2008, 9, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("October", new DateTime(2008, 10, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("November", new DateTime(2008, 11, 15).ToString(DateTimeFormat.MonthName));

			Assert.AreEqual("December", new DateTime(2008, 12, 15).ToString(DateTimeFormat.MonthName));
		}

		/// <summary>
		/// A test for GetDayOfWeekNDaysAgo
		/// </summary>
		[TestMethod]
		public void GetDayOfWeekNDaysAgoTest()
		{
			DateTime dt = new DateTime(2008, 10, 30, 8, 7, 0);

			Assert.AreEqual(DayOfWeek.Monday, dt.GetDayOfWeekNDaysAgo(3));
		}

		/// <summary>
		/// A test for DisplayDate
		/// </summary>
		[TestMethod]
		public void DisplayDateTest()
		{
			DateTime dt = new DateTime(2011, 10, 30, 8, 7, 00);
			string expected = "October 30, 2011";
			string actual = dt.ToString(DateTimeFormat.DisplayableDate);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// A test for DaysSince
		/// </summary>
		[TestMethod]
		public void DaysSinceTest()
		{
			DateTime dt = new DateTime(2008, 10, 30, 8, 7, 0);
			DateTime then = new DateTime(2008, 10, 29, 6, 7, 0);

			Assert.AreEqual<double>(1, dt.GetDaysSince(then));
		}

		/// <summary>
		/// A test for TimeTillMidnight
		/// </summary>
		[TestMethod]
		public void TimeTillMidnight_GivesCorrectInterval_GivenDateTime()
		{
			// five hours, thirteen minutes, twenty seconds, and 998 milliseconds
			TimeSpan someInterval = new TimeSpan(0, 5, 13, 20, 998);

			// midnight on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 0, 0, 0, 0);

			// back up 5 hrs, 13 min, etc from midnight
			DateTime valueToTest = someDate - someInterval;

			// since valueToTest is midnight - interval, TimeTillMidnight of valueToTest should = interval
			Assert.AreEqual(someInterval, valueToTest.TimeUntilMidnight());
		}
	
		/// <summary>
		/// A test for TimeTillMidnight
		/// </summary>
		[TestMethod]
		public void TimeTillMidnight_GivesTwentyFourHourInterval_GivenMidnight()
		{
			// midnight on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 0, 0, 0, 0);

			// since valueToTest is midnight - interval, TimeTillMidnight of valueToTest should = interval
			Assert.AreEqual(TimeSpan.FromHours(24.0), someDate.TimeUntilMidnight());
		}

		/// <summary>
		/// A test for TimeTillMidnight
		/// </summary>
		[TestMethod]
		public void TimeTillMidnight_GivesOneMsInterval_GivenMidnightMinusOneMs()
		{
			// midnight on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 23, 59, 59, 999);

			// since valueToTest is midnight - interval, TimeTillMidnight of valueToTest should = interval
			Assert.AreEqual(TimeSpan.FromMilliseconds(1), someDate.TimeUntilMidnight());
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesCorrectIntervalGivenMidnight()
		{
			// five hours, thirteen minutes, twenty seconds, and 998 milliseconds
			TimeSpan someInterval = new TimeSpan(0, 5, 13, 20, 998);

			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 0, 0, 0, 0);

			// back up 5 hrs, 13 min, etc from time of day
			DateTime valueToTest = someDate - someInterval;

			// since valueToTest is time of day - interval, TimeTillTime of Day of valueToTest should = interval
			Assert.AreEqual(someInterval, valueToTest.TimeUntil(TimeSpan.FromTicks(0)));
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesTwentyFourHourIntervalGivenMidnight()
		{
			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 0, 0, 0, 0);

			// since valueToTest is time of day - interval, TimeTillTime of Day of valueToTest should = interval
			Assert.AreEqual(TimeSpan.FromHours(24.0), someDate.TimeUntil(TimeSpan.FromTicks(0)));
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesOneMsIntervalGivenMidnightMinusOneMs()
		{
			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 23, 59, 59, 999);

			// since valueToTest is time of day - interval, TimeTillTime of Day of valueToTest should = interval
			Assert.AreEqual(TimeSpan.FromMilliseconds(1), someDate.TimeUntil(TimeSpan.FromTicks(0)));
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesCorrectIntervalGivenDateTime()
		{
			// five hours, thirteen minutes, twenty seconds, and 998 milliseconds
			TimeSpan someInterval = new TimeSpan(0, 5, 13, 20, 998);

			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 20, 30, 0, 0);

			// back up 5 hrs, 13 min, etc from time of day
			DateTime valueToTest = someDate - someInterval;

			// since valueToTest is time of day - interval, TimeTillTime of Day of valueToTest should = interval
			Assert.AreEqual(someInterval, valueToTest.TimeUntil(new TimeSpan(20, 30, 0)));
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesTwentyFourHourIntervalGivenTimeOfDay()
		{
			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 20, 30, 0, 0);

			// since valueToTest is time of day - interval, TimeTillTime of Day of valueToTest should = interval
			Assert.AreEqual(TimeSpan.FromHours(24.0), someDate.TimeUntil(new TimeSpan(20, 30, 0)));
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesOneMsIntervalGivenTimeOfDayMinusOneMs()
		{
			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 20, 29, 59, 999);

			// since valueToTest is time of day - interval, TimeTillTime of Day of valueToTest should = interval
			Assert.AreEqual(TimeSpan.FromMilliseconds(1), someDate.TimeUntil(new TimeSpan(20, 30, 0)));
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesTwentyFourHourIntervalGivenTimeOfDayAfter()
		{
			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 20, 30, 0, 0);

			// since loops around to next day, should be 9 hours
			Assert.AreEqual(TimeSpan.FromHours(9.0), someDate.TimeUntil(new TimeSpan(5, 30, 0)));
		}

		/// <summary>
		/// A test for TimeTillTime of Day
		/// </summary>
		[TestMethod]
		public void TimeUntilTimeOfDayGivesOneMsIntervalGivenTimeOfDayOneMsAfter()
		{
			// time of day on 10/1/2010
			DateTime someDate = new DateTime(2010, 10, 1, 20, 30, 0, 001);

			// since valueToTest is time of day - interval, TimeTillTime of Day of valueToTest should = interval
			Assert.AreEqual(new TimeSpan(0, 23, 59, 59, 999), someDate.TimeUntil(new TimeSpan(20, 30, 0)));
		}
	}
}