using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Financial.UnitTests
{
    /// <summary>
    /// This is a test class for FinanceDateTimeTest and is intended
    /// to contain all FinanceDateTimeTest Unit Tests
    /// </summary>
    [TestClass]
    public class FinanceDateTimeTest
    {

        /// <summary>
        /// Common setup tasks for the tests
        /// </summary>
        /// <param name="a">The test context</param>
        [ClassInitialize]
        public static void ClassSetup(TestContext a)
        {
            FinanceDateTime.Configuration.HalfDays.Add(
				new FinanceDateTime.HoursConfiguration.Date(2008, 12, 24));
            FinanceDateTime.Configuration.HalfDays.Add(
				new FinanceDateTime.HoursConfiguration.Date(2008, 12, 31));
            FinanceDateTime.Configuration.SpecialHolidays.Add(
				new FinanceDateTime.HoursConfiguration.Date(2008, 12, 11));
        }

        
        /// <summary>
        /// A test for SetToMarketOpen
        /// </summary>
        [TestMethod]
        public void SetToMarketOpenTest()
        {
            FinanceDateTime fdt = new FinanceDateTime();
            var expected = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0);
            fdt.SetToMarketOpen(0, 0);
            Assert.AreEqual(expected, fdt.DateTimeStamp);

            var expected2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 45, 0);
            fdt.SetToMarketOpen(0, 15);
            Assert.AreEqual(expected2, fdt.DateTimeStamp);

            var expected3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 15, 0);
            fdt.SetToMarketOpen(0, -15);
            Assert.AreEqual(expected3, fdt.DateTimeStamp);

            var expected4 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 45, 0);
            fdt.SetToMarketOpen(0, 75);
            Assert.AreEqual(expected4, fdt.DateTimeStamp);

            var expected5 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 45, 0);
            expected5 = expected5.AddDays(1);
            fdt.SetToMarketOpen(25, 15);
            Assert.AreEqual(expected5, fdt.DateTimeStamp);
        }

        /// <summary>
        /// A test for SetToMarketClose
        /// </summary>
        [TestMethod]
        public void SetToMarketCloseTest_HalfDay()
        {
            // half day based on config file
            FinanceDateTime fdt = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 0, 0));
            var expected = new DateTime(2008, 12, 24, 13, 0, 0);
            fdt.SetToMarketClose(0, 0);
            Assert.AreEqual(expected, fdt.DateTimeStamp);

            var expected2 = new DateTime(2008, 12, 24, 13, 15, 0);
            fdt.SetToMarketClose(0, 15);
            Assert.AreEqual(expected2, fdt.DateTimeStamp);

            var expected3 = new DateTime(2008, 12, 24, 12, 45, 0);
            fdt.SetToMarketClose(0, -15);
            Assert.AreEqual(expected3, fdt.DateTimeStamp);

            var expected4 = new DateTime(2008, 12, 24, 14, 15, 0);
            fdt.SetToMarketClose(0, 75);
            Assert.AreEqual(expected4, fdt.DateTimeStamp);

            var expected5 = new DateTime(2008, 12, 24, 14, 15, 0);
            expected5 = expected5.AddDays(1);
            fdt.SetToMarketClose(25, 15);
            Assert.AreEqual(expected5, fdt.DateTimeStamp);
        }

        /// <summary>
        /// A test for SetToMarketClose
        /// </summary>
        [TestMethod]
        public void SetToMarketCloseTest_FullDay()
        {
            FinanceDateTime fdt = new FinanceDateTime(new DateTime(2008, 11, 13, 12, 0, 0));
            var expected = new DateTime(2008, 11, 13, 16, 0, 0);
            fdt.SetToMarketClose(0, 0);
            Assert.AreEqual(expected, fdt.DateTimeStamp);

            var expected2 = new DateTime(2008, 11, 13, 16, 15, 0);
            fdt.SetToMarketClose(0, 15);
            Assert.AreEqual(expected2, fdt.DateTimeStamp);

            var expected3 = new DateTime(2008, 11, 13, 15, 45, 0);
            fdt.SetToMarketClose(0, -15);
            Assert.AreEqual(expected3, fdt.DateTimeStamp);

            var expected4 = new DateTime(2008, 11, 13, 17, 15, 0);
            fdt.SetToMarketClose(0, 75);
            Assert.AreEqual(expected4, fdt.DateTimeStamp);

            var expected5 = new DateTime(2008, 11, 13, 17, 15, 0);
            expected5 = expected5.AddDays(1);
            fdt.SetToMarketClose(25, 15);
            Assert.AreEqual(expected5, fdt.DateTimeStamp);
        }

        /// <summary>
        /// A test for HasPreMarketOpened
        /// </summary>
        [TestMethod]
        public void HasPreMarketOpenedTest()
        {
            FinanceDateTime beforePre = new FinanceDateTime(new DateTime(2008, 11, 13, 6, 30, 0));
            Assert.IsFalse(beforePre.HasPreMarketOpened());

            FinanceDateTime duringPre = new FinanceDateTime(new DateTime(2008, 11, 13, 8, 30, 0));
            Assert.IsTrue(duringPre.HasPreMarketOpened());

            FinanceDateTime afterPre = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 30, 0));
            Assert.IsTrue(afterPre.HasPreMarketOpened());
        }

        /// <summary>
        /// A test for HasPreMarketClosed
        /// </summary>
        [TestMethod]
        public void HasPreMarketClosedTest()
        {
            FinanceDateTime beforePre = new FinanceDateTime(new DateTime(2008, 11, 13, 7, 0, 0));
            Assert.IsFalse(beforePre.HasPreMarketClosed());

            FinanceDateTime duringPre = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 1, 0));
            Assert.IsFalse(duringPre.HasPreMarketClosed());

            FinanceDateTime afterPre = new FinanceDateTime(new DateTime(2008, 11, 13, 10, 16, 0));
            Assert.IsTrue(afterPre.HasPreMarketClosed());
        }

        /// <summary>
        /// A test for GetMinutesToMarketOpen
        /// </summary>
        [TestMethod]
        public void GetMinutesToMarketOpenTest()
        {
            FinanceDateTime fdt = new FinanceDateTime(new DateTime(2008, 11, 13, 8, 30, 0));
            Assert.AreEqual(60, fdt.GetMinutesToMarketOpen());

            FinanceDateTime fdt2 = new FinanceDateTime(new DateTime(2008, 11, 13, 10, 0, 0));
            Assert.AreEqual(null, fdt2.GetMinutesToMarketOpen());
        }

        /// <summary>
        /// A test for GetMinutesToMarketClose
        /// </summary>
        [TestMethod]
        public void GetMinutesToMarketCloseTest()
        {
            FinanceDateTime fdt = new FinanceDateTime(new DateTime(2008, 11, 13, 15, 0, 0));
            Assert.AreEqual(60, fdt.GetMinutesToMarketClose());

            FinanceDateTime fdt2 = new FinanceDateTime(new DateTime(2008, 11, 13, 17, 0, 0));
            Assert.AreEqual(null, fdt2.GetMinutesToMarketClose());

            FinanceDateTime fdt3 = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 0, 0));
            Assert.AreEqual(60, fdt3.GetMinutesToMarketClose());

            FinanceDateTime fdt4 = new FinanceDateTime(new DateTime(2008, 11, 15, 12, 0, 0));
            Assert.AreEqual(null, fdt4.GetMinutesToMarketClose());
        }

        /// <summary>
        /// A test for HasMarketOpened
        /// </summary>
        [TestMethod]
        public void HasMarketOpenedTest()
        {
            FinanceDateTime beforeOpen = new FinanceDateTime(-2, 0, MarketTimeType.MarketOpen);
            Assert.IsFalse(beforeOpen.HasMarketOpened());

            FinanceDateTime duringOpen = new FinanceDateTime(3, 0, MarketTimeType.MarketOpen);
            Assert.IsTrue(duringOpen.HasMarketOpened());

            FinanceDateTime afterClose = new FinanceDateTime(1, 0, MarketTimeType.MarketClose);
            Assert.IsTrue(afterClose.HasMarketOpened());
        }

        /// <summary>
        /// A test for HasMarketClosed
        /// </summary>
        [TestMethod]
        public void HasMarketClosedTest_HalfDayWithTypeSpecified()
        {
            // half days depend on what is in config
            FinanceDateTime beforeHalfClose = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 0, 0));
            FinanceDateTime beforeHalfIndexClose = new FinanceDateTime(new DateTime(2008, 12, 24, 13, 1, 0));
            FinanceDateTime afterHalfClose = new FinanceDateTime(new DateTime(2008, 12, 24, 13, 16, 0));

            Assert.IsFalse(beforeHalfClose.HasMarketClosed(MarketCloseType.Equity));
            Assert.IsFalse(beforeHalfClose.HasMarketClosed(MarketCloseType.Option));
            Assert.IsFalse(beforeHalfClose.HasMarketClosed(MarketCloseType.IndexSecurity));

            Assert.IsTrue(beforeHalfIndexClose.HasMarketClosed(MarketCloseType.Equity));
            Assert.IsTrue(beforeHalfIndexClose.HasMarketClosed(MarketCloseType.Option));
            Assert.IsFalse(beforeHalfIndexClose.HasMarketClosed(MarketCloseType.IndexSecurity));

            Assert.IsTrue(afterHalfClose.HasMarketClosed(MarketCloseType.Equity));
            Assert.IsTrue(afterHalfClose.HasMarketClosed(MarketCloseType.Option));
            Assert.IsTrue(afterHalfClose.HasMarketClosed(MarketCloseType.IndexSecurity));
        }

        /// <summary>
        /// A test for HasMarketClosed
        /// </summary>
        [TestMethod]
        public void HasMarketClosedTest_FullDayWithDefaultTypeEquity()
        {
            FinanceDateTime beforeOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 8, 0, 0));
            Assert.IsFalse(beforeOpen.HasMarketClosed());

            FinanceDateTime duringOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 12, 0, 0));
            Assert.IsFalse(duringOpen.HasMarketClosed());

            FinanceDateTime afterClose = new FinanceDateTime(new DateTime(2008, 11, 13, 17, 0, 0));
            Assert.IsTrue(afterClose.HasMarketClosed());
        }

        /// <summary>
        /// A test for HasMarketClosed
        /// </summary>
        [TestMethod]
        public void HasMarketClosedTest_FullDayWithTypeSpecified()
        {
            FinanceDateTime beforeClose = new FinanceDateTime(new DateTime(2008, 11, 13, 12, 0, 0));
            FinanceDateTime beforeIndexClose = new FinanceDateTime(new DateTime(2008, 11, 13, 16, 1, 0));
            FinanceDateTime afterClose = new FinanceDateTime(new DateTime(2008, 11, 13, 16, 16, 0));

            Assert.IsFalse(beforeClose.HasMarketClosed(MarketCloseType.Equity));
            Assert.IsFalse(beforeClose.HasMarketClosed(MarketCloseType.Option));
            Assert.IsFalse(beforeClose.HasMarketClosed(MarketCloseType.IndexSecurity));

            Assert.IsTrue(beforeIndexClose.HasMarketClosed(MarketCloseType.Equity));
            Assert.IsTrue(beforeIndexClose.HasMarketClosed(MarketCloseType.Option));
            Assert.IsFalse(beforeIndexClose.HasMarketClosed(MarketCloseType.IndexSecurity));

            Assert.IsTrue(afterClose.HasMarketClosed(MarketCloseType.Equity));
            Assert.IsTrue(afterClose.HasMarketClosed(MarketCloseType.Option));
            Assert.IsTrue(afterClose.HasMarketClosed(MarketCloseType.IndexSecurity));
        }

        /// <summary>
        /// A test for IsValidGtd
        /// </summary>
        [TestMethod]
        public void IsValidGtdTest()
        {
            FinanceDateTime validGtd = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            FinanceDateTime invalidGtd = new FinanceDateTime(new DateTime(2008, 11, 1, 13, 0, 0));
            FinanceDateTime invalidGtd1 = new FinanceDateTime(new DateTime(3009, 12, 22, 12, 0, 0));
            FinanceDateTime invalidGtd2 = new FinanceDateTime(new DateTime(2008, 10, 31, 19, 0, 0));

            Assert.IsTrue(validGtd.IsValidGtdExpiration());
            Assert.IsFalse(invalidGtd.IsValidGtdExpiration());
            Assert.IsFalse(invalidGtd1.IsValidGtdExpiration());
            Assert.IsFalse(invalidGtd2.IsValidGtdExpiration());
        }

        /// <summary>
        /// A test for IsPreMarketOpen
        /// </summary>
        [TestMethod]
        public void IsPreMarketOpenTest()
        {
            FinanceDateTime beforePre = new FinanceDateTime(new DateTime(2008, 12, 24, 6, 15, 0)); // 6:30
            Assert.IsFalse(beforePre.IsPreMarketOpen());

            FinanceDateTime duringPre = new FinanceDateTime(new DateTime(2008, 12, 24, 8, 30, 0)); // 8:30
            Assert.IsTrue(duringPre.IsPreMarketOpen());

            FinanceDateTime afterPre = new FinanceDateTime(new DateTime(2008, 12, 24, 9, 30, 0)); // 9:30
            Assert.IsFalse(afterPre.IsPreMarketOpen());
        }

        /// <summary>
        /// A test for IsPastMarketOpen
        /// </summary>
        [TestMethod]
        public void IsPastMarketOpenTest()
        {
            FinanceDateTime afterOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 33, 0));
            FinanceDateTime beforeOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 25, 0));

            Assert.IsFalse(afterOpen.IsPastMarketOpen(4));
            Assert.IsTrue(afterOpen.IsPastMarketOpen(3));
            Assert.IsTrue(afterOpen.IsPastMarketOpen(2));

            Assert.IsFalse(beforeOpen.IsPastMarketOpen(1));
        }


        /// <summary>
        /// A test for IsNearMarketClose
        /// </summary>
        [TestMethod]
        public void IsNearMarketCloseTest_FullDay()
        {
            FinanceDateTime near = new FinanceDateTime(new DateTime(2008, 11, 13, 15, 59, 58));
            FinanceDateTime near1 = new FinanceDateTime(new DateTime(2008, 11, 13, 16, 0, 2));
            FinanceDateTime before = new FinanceDateTime(new DateTime(2008, 11, 13, 15, 0, 0));
            FinanceDateTime after = new FinanceDateTime(new DateTime(2008, 11, 13, 17, 0, 0));

            Assert.IsTrue(near.IsNearMarketClose());
            Assert.IsTrue(near1.IsNearMarketClose());
            Assert.IsFalse(before.IsNearMarketClose());
            Assert.IsFalse(after.IsNearMarketClose());
        }


        /// <summary>
        /// A test for IsNearMarketClose
        /// </summary>
        [TestMethod]
        public void IsNearMarketCloseTest_HalfDay()
        {
            FinanceDateTime near = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 59, 58));
            FinanceDateTime before = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 0, 0));
            FinanceDateTime after = new FinanceDateTime(new DateTime(2008, 12, 24, 14, 0, 0));

            Assert.IsTrue(near.IsNearMarketClose());
            Assert.IsFalse(before.IsNearMarketClose());
            Assert.IsFalse(after.IsNearMarketClose());
        }

        /// <summary>
        /// A test for IsNearMarketClose
        /// </summary>
        [TestMethod]
        public void IsNearMarketCloseTest_HalfDayWithTypeSpecified()
        {
            FinanceDateTime near = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 59, 58));
            FinanceDateTime nearIndex = new FinanceDateTime(new DateTime(2008, 12, 24, 13, 15, 2));
            FinanceDateTime before = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 0, 0));
            FinanceDateTime after = new FinanceDateTime(new DateTime(2008, 12, 24, 14, 0, 0));

            Assert.IsTrue(near.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsTrue(near.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsFalse(near.IsNearMarketClose(MarketCloseType.IndexSecurity));

            Assert.IsFalse(nearIndex.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsFalse(nearIndex.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsTrue(nearIndex.IsNearMarketClose(MarketCloseType.IndexSecurity));

            Assert.IsFalse(before.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsFalse(before.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsFalse(before.IsNearMarketClose(MarketCloseType.IndexSecurity));

            Assert.IsFalse(after.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsFalse(after.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsFalse(after.IsNearMarketClose(MarketCloseType.IndexSecurity));
        }

        /// <summary>
        /// A test for IsNearMarketClose
        /// </summary>
        [TestMethod]
        public void IsNearMarketCloseTest_FullDayWithTypeSpecified()
        {
            FinanceDateTime near = new FinanceDateTime(new DateTime(2008, 11, 13, 15, 59, 58));
            FinanceDateTime nearIndex = new FinanceDateTime(new DateTime(2008, 11, 13, 16, 14, 58));
            FinanceDateTime before = new FinanceDateTime(new DateTime(2008, 11, 13, 15, 0, 0));
            FinanceDateTime after = new FinanceDateTime(new DateTime(2008, 11, 13, 17, 0, 0));

            Assert.IsTrue(near.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsTrue(near.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsFalse(near.IsNearMarketClose(MarketCloseType.IndexSecurity));

            Assert.IsFalse(nearIndex.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsFalse(nearIndex.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsTrue(nearIndex.IsNearMarketClose(MarketCloseType.IndexSecurity));

            Assert.IsFalse(before.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsFalse(before.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsFalse(before.IsNearMarketClose(MarketCloseType.IndexSecurity));

            Assert.IsFalse(after.IsNearMarketClose(MarketCloseType.Equity));
            Assert.IsFalse(after.IsNearMarketClose(MarketCloseType.Option));
            Assert.IsFalse(after.IsNearMarketClose(MarketCloseType.IndexSecurity));
        }

        /// <summary>
        /// A test for IsMarketOpen
        /// </summary>
        [TestMethod]
        public void IsMarketOpenTest_WithDefaultTypeEquity()
        {
            FinanceDateTime beforeOpen = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            beforeOpen.SetToMarketOpen(-1, 0);
            FinanceDateTime afterOpen = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            afterOpen.SetToMarketOpen(1, 0);
            FinanceDateTime afterClose = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            afterClose.SetToMarketClose(1, 0);

            Assert.IsFalse(beforeOpen.IsMarketOpen());
            Assert.IsTrue(afterOpen.IsMarketOpen());
            Assert.IsFalse(afterClose.IsMarketOpen());
        }

        /// <summary>
        /// A test for IsMarketOpen
        /// </summary>
        [TestMethod]
        public void IsMarketOpenTest_WithSpecifiedType()
        {
            FinanceDateTime beforeOpen = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            beforeOpen.SetToMarketOpen(-1, 0);
            FinanceDateTime afterOpen = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            afterOpen.SetToMarketOpen(1, 0);
            FinanceDateTime beforeIndexClose = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            beforeIndexClose.SetToMarketClose(0, 10);
            FinanceDateTime afterClose = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            afterClose.SetToMarketClose(1, 0);

            Assert.IsFalse(beforeOpen.IsMarketOpen(MarketCloseType.Equity));
            Assert.IsTrue(afterOpen.IsMarketOpen(MarketCloseType.Equity));
            Assert.IsFalse(beforeIndexClose.IsMarketOpen(MarketCloseType.Equity));
            Assert.IsFalse(afterClose.IsMarketOpen(MarketCloseType.Equity));

            Assert.IsFalse(beforeOpen.IsMarketOpen(MarketCloseType.Option));
            Assert.IsTrue(afterOpen.IsMarketOpen(MarketCloseType.Option));
            Assert.IsFalse(beforeIndexClose.IsMarketOpen(MarketCloseType.Option));
            Assert.IsFalse(afterClose.IsMarketOpen(MarketCloseType.Option));

            Assert.IsFalse(beforeOpen.IsMarketOpen(MarketCloseType.IndexSecurity));
            Assert.IsTrue(afterOpen.IsMarketOpen(MarketCloseType.IndexSecurity));
            Assert.IsTrue(beforeIndexClose.IsMarketOpen(MarketCloseType.IndexSecurity));
            Assert.IsFalse(afterClose.IsMarketOpen(MarketCloseType.IndexSecurity));
        }

        /// <summary>
        /// A test for IsMarketDay
        /// </summary>
        [TestMethod]
        public void IsMarketDayTest()
        {
            FinanceDateTime valid = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 0, 0));
            FinanceDateTime invalid = new FinanceDateTime(new DateTime(2008, 11, 1, 9, 0, 0));
            FinanceDateTime invalid1 = new FinanceDateTime(new DateTime(2008, 11, 2, 9, 0, 0));
            FinanceDateTime invalid2 = new FinanceDateTime(new DateTime(2008, 11, 27, 9, 0, 0));

            Assert.IsTrue(valid.IsMarketDay());
            Assert.IsFalse(invalid.IsMarketDay());
            Assert.IsFalse(invalid1.IsMarketDay());
            Assert.IsFalse(invalid2.IsMarketDay());
        }

        /// <summary>
        /// A test for IsMarket15MinutesToOpen
        /// </summary>
        [TestMethod]
        public void IsMarket15MinutesToOpenTest()
        {
            FinanceDateTime valid = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 20, 0));
            FinanceDateTime invalid = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 10, 0));
            FinanceDateTime invalid1 = new FinanceDateTime(new DateTime(2008, 11, 13, 9, 31, 0));
            FinanceDateTime invalid2 = new FinanceDateTime(new DateTime(2008, 11, 15, 9, 31, 0));

            Assert.IsTrue(valid.IsWithinFifteenMinutesToOpen());
            Assert.IsFalse(invalid.IsWithinFifteenMinutesToOpen());
            Assert.IsFalse(invalid1.IsWithinFifteenMinutesToOpen());
            Assert.IsFalse(invalid2.IsWithinFifteenMinutesToOpen());
        }

        /// <summary>
        /// A test for IsHolidayHalfDay
        /// </summary>
        [TestMethod]
        public void IsHolidayHalfDayTest()
        {
            // from config
            FinanceDateTime halfDay = new FinanceDateTime(new DateTime(2008, 12, 31, 9, 0, 0));
            FinanceDateTime notHalfDay = new FinanceDateTime(new DateTime(2008, 11, 11, 9, 0, 0));

            Assert.IsTrue(halfDay.IsHalfDayHoliday());
            Assert.IsFalse(notHalfDay.IsHalfDayHoliday());
        }

        /// <summary>
        /// A test for IsHoliday
        /// </summary>
        [TestMethod]
        public void IsHolidayTest()
        {
            FinanceDateTime thanksgiving = new FinanceDateTime(new DateTime(2008, 11, 27, 9, 0, 0));
            FinanceDateTime independence = new FinanceDateTime(new DateTime(2008, 7, 4, 9, 0, 0));
            FinanceDateTime christmas = new FinanceDateTime(new DateTime(2008, 12, 25, 9, 0, 0));
            FinanceDateTime newYears = new FinanceDateTime(new DateTime(2009, 1, 1, 9, 0, 0));
            FinanceDateTime mlkDay = new FinanceDateTime(new DateTime(2008, 1, 21, 9, 0, 0));
            FinanceDateTime presidents = new FinanceDateTime(new DateTime(2008, 2, 18, 9, 0, 0));
            FinanceDateTime memorial = new FinanceDateTime(new DateTime(2008, 5, 26, 9, 0, 0));
            FinanceDateTime labor = new FinanceDateTime(new DateTime(2008, 9, 1, 9, 0, 0));
            FinanceDateTime goodFriday = new FinanceDateTime(new DateTime(2008, 3, 21, 9, 0, 0));
            FinanceDateTime notHoliday = new FinanceDateTime(new DateTime(2008, 11, 1, 9, 0, 0));

            // from config
            FinanceDateTime specialHoliday = new FinanceDateTime(new DateTime(2008, 12, 11, 9, 0, 0));

            Assert.IsTrue(labor.IsHoliday());
            Assert.IsTrue(thanksgiving.IsHoliday());
            Assert.IsTrue(independence.IsHoliday());
            Assert.IsTrue(christmas.IsHoliday());
            Assert.IsTrue(newYears.IsHoliday());
            Assert.IsTrue(mlkDay.IsHoliday());
            Assert.IsTrue(presidents.IsHoliday());
            Assert.IsTrue(memorial.IsHoliday());
            Assert.IsTrue(goodFriday.IsHoliday());
            Assert.IsTrue(specialHoliday.IsHoliday());
            Assert.IsFalse(notHoliday.IsHoliday());
        }

        /// <summary>
        /// A test for IsExtendedHoursMarketOpen
        /// </summary>
        [TestMethod]
        public void IsExtendedHoursMarketOpenTest()
        {
            FinanceDateTime valid = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            valid.SetToMarketClose(0, 20); // extended open from 4:15 to 8
            FinanceDateTime notMarketDay = new FinanceDateTime(new DateTime(2008, 11, 1, 9, 0, 0));
            FinanceDateTime invalidTime = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            invalidTime.SetToMarketClose(0, -1);
            FinanceDateTime closed = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            closed.SetToMarketClose(5, 0);

            Assert.IsTrue(valid.IsExtendedHoursMarketOpen());
            Assert.IsFalse(notMarketDay.IsExtendedHoursMarketOpen());
            Assert.IsFalse(invalidTime.IsExtendedHoursMarketOpen());
            Assert.IsFalse(closed.IsExtendedHoursMarketOpen());
        }

        /// <summary>
        /// A test for IsDuringWideSpread
        /// </summary>
        [TestMethod]
        public void IsDuringWideSpreadTest()
        {
            FinanceDateTime afterClose = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            afterClose.SetToMarketClose(0, 2);
            FinanceDateTime notMarketDay = new FinanceDateTime(new DateTime(2008, 11, 1, 9, 0, 0));
            FinanceDateTime afterOpen = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            afterOpen.SetToMarketOpen(0, 10);
            FinanceDateTime beforeOpen = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            beforeOpen.SetToMarketOpen(0, -10);
            FinanceDateTime beforePreOpen = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            beforePreOpen.SetToMarketOpen(0, -20);

            Assert.IsTrue(afterClose.IsDuringWideSpread());
            Assert.IsTrue(notMarketDay.IsDuringWideSpread());
            Assert.IsTrue(beforePreOpen.IsDuringWideSpread());
            Assert.IsFalse(afterOpen.IsDuringWideSpread());
            Assert.IsFalse(beforeOpen.IsDuringWideSpread());
        }

        /// <summary>
        /// A test for GetThanksgiving
        /// </summary>
        [TestMethod]
        public void GetThanksgivingTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 11, 27));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetThanksgiving(2008));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetPresidentsDay
        /// </summary>
        [TestMethod]
        public void GetPresidentsDayTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 2, 18));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetPresidentsDay(2008));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetNewYearsDay
        /// </summary>
        [TestMethod]
        public void GetNewYearsDayTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 1, 1));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetNewYearsDay(2008));
            FinanceDateTime expected1 = new FinanceDateTime(new DateTime(2006, 1, 2));
            FinanceDateTime actual1 = new FinanceDateTime(FinanceDateTime.GetNewYearsDay(2006));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
            Assert.AreEqual((object) expected1.DateTimeStamp, actual1.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetMLKDay
        /// </summary>
        [TestMethod]
        public void GetMlkDayTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 1, 21));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetMartinLutherKingDay(2008));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetMemorialDay
        /// </summary>
        [TestMethod]
        public void GetMemorialDayTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 5, 26));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetMemorialDay(2008));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetLaborDay
        /// </summary>
        [TestMethod]
        public void GetLaborDayTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2009, 9, 7));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetLaborDay(2009));
            FinanceDateTime expected1 = new FinanceDateTime(new DateTime(2008, 9, 1));
            FinanceDateTime actual1 = new FinanceDateTime(FinanceDateTime.GetLaborDay(2008));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
            Assert.AreEqual((object) expected1.DateTimeStamp, actual1.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetIndependenceDay
        /// </summary>
        [TestMethod]
        public void GetIndependenceDayTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 7, 4));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetIndependenceDay(2008));
            FinanceDateTime expected1 = new FinanceDateTime(new DateTime(2009, 7, 3));
            FinanceDateTime actual1 = new FinanceDateTime(FinanceDateTime.GetIndependenceDay(2009));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
            Assert.AreEqual((object) expected1.DateTimeStamp, actual1.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetGoodFriday
        /// </summary>
        [TestMethod]
        public void GetGoodFridayTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 3, 21));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetGoodFriday(2008));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
        }

        /// <summary>
        /// A test for GetChristmas
        /// </summary>
        [TestMethod]
        public void GetChristmasTest()
        {
            FinanceDateTime expected = new FinanceDateTime(new DateTime(2008, 12, 25));
            FinanceDateTime actual = new FinanceDateTime(FinanceDateTime.GetChristmas(2008));
            FinanceDateTime expected1 = new FinanceDateTime(new DateTime(2005, 12, 26));
            FinanceDateTime actual1 = new FinanceDateTime(FinanceDateTime.GetChristmas(2005));
            FinanceDateTime expected2 = new FinanceDateTime(new DateTime(2010, 12, 24));
            FinanceDateTime actual2 = new FinanceDateTime(FinanceDateTime.GetChristmas(2010));

            Assert.AreEqual((object) expected.DateTimeStamp, actual.DateTimeStamp);
            Assert.AreEqual((object) expected1.DateTimeStamp, actual1.DateTimeStamp);
            Assert.AreEqual((object) expected2.DateTimeStamp, actual2.DateTimeStamp);
        }

        /// <summary>
        /// A test for ExtendedHoursHasClosed
        /// </summary>
        [TestMethod]
        public void ExtendedHoursHasOpenedTest_FullDay()
        {
            FinanceDateTime beforeExtOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 15, 59, 0));
            FinanceDateTime beforeHalfExtOpen1 = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 0, 0));
            FinanceDateTime afterExtOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 16, 16, 0));
            FinanceDateTime afterExtClose = new FinanceDateTime(new DateTime(2008, 11, 13, 20, 1, 0));

            Assert.IsFalse(beforeExtOpen.HasExtendedHoursOpened());
            Assert.IsFalse(beforeHalfExtOpen1.HasExtendedHoursOpened());
            Assert.IsTrue(afterExtOpen.HasExtendedHoursOpened());
            Assert.IsTrue(afterExtClose.HasExtendedHoursOpened());
        }

        /// <summary>
        /// A test for ExtendedHoursHasClosed
        /// </summary>
        [TestMethod]
        public void ExtendedHoursHasOpenedTest_HalfDay()
        {
            FinanceDateTime beforeHalfExtOpen = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 59, 0));
            FinanceDateTime beforeHalfExtOpen1 = new FinanceDateTime(new DateTime(2008, 12, 24, 12, 0, 0));
            FinanceDateTime afterHalfExtOpen = new FinanceDateTime(new DateTime(2008, 12, 24, 13, 1, 0));
            FinanceDateTime afterHalfExtClose = new FinanceDateTime(new DateTime(2008, 12, 24, 17, 1, 0));

            Assert.IsFalse(beforeHalfExtOpen.HasExtendedHoursOpened());
            Assert.IsFalse(beforeHalfExtOpen1.HasExtendedHoursOpened());
            Assert.IsTrue(afterHalfExtOpen.HasExtendedHoursOpened());
            Assert.IsTrue(afterHalfExtClose.HasExtendedHoursOpened());
        }

        /// <summary>
        /// A test for ExtendedHoursHasClosed
        /// </summary>
        [TestMethod]
        public void ExtendedHoursHasClosedTest_FullDay()
        {
            FinanceDateTime beforeExtOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 16, 0, 0));
            FinanceDateTime afterExtOpen = new FinanceDateTime(new DateTime(2008, 11, 13, 16, 16, 0));
            FinanceDateTime afterExtClose = new FinanceDateTime(new DateTime(2008, 11, 13, 20, 1, 0));

            Assert.IsFalse(afterExtOpen.HasExtendedHoursClosed());
            Assert.IsTrue(afterExtClose.HasExtendedHoursClosed());
            Assert.IsFalse(beforeExtOpen.HasExtendedHoursClosed());
        }

        /// <summary>
        /// A test for ExtendedHoursHasClosed
        /// </summary>
        [TestMethod]
        public void ExtendedHoursHasClosedTest_HalfDay()
        {
            FinanceDateTime beforeHalfExtOpen = new FinanceDateTime(new DateTime(2008, 12, 24, 13, 0, 0));
            FinanceDateTime afterHalfExtOpen = new FinanceDateTime(new DateTime(2008, 12, 24, 13, 16, 0));
            FinanceDateTime afterHalfExtClose = new FinanceDateTime(new DateTime(2008, 12, 24, 17, 1, 0));

            Assert.IsFalse(afterHalfExtOpen.HasExtendedHoursClosed());
            Assert.IsTrue(afterHalfExtClose.HasExtendedHoursClosed());
            Assert.IsFalse(beforeHalfExtOpen.HasExtendedHoursClosed());
        }

        /// <summary>
        /// A test for DoesMeetAllGtdCriteria
        /// </summary>
        [TestMethod]
        public void DoesMeetAllGtdCriteriaTest()
        {
            FinanceDateTime valid = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            valid.SetToMarketOpen(2, 0);
            FinanceDateTime invalid = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            invalid.SetToMarketOpen(1, 13); // not 30 or 0
            FinanceDateTime invalid1 = new FinanceDateTime(new DateTime(2008, 11, 1, 9, 0, 0)); 
            FinanceDateTime invalid2 = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            invalid2.DateTimeStamp = invalid2.DateTimeStamp.AddDays(90);
            FinanceDateTime invalid3 = new FinanceDateTime(); // in past
            invalid3.DateTimeStamp = invalid3.DateTimeStamp.AddDays(-3);
            FinanceDateTime invalid4 = new FinanceDateTime(FinanceDateTime.GetNextMarketOpen());
            invalid4.SetToMarketClose(0, 30);

            Assert.IsTrue(valid.IsValidGtdExpiration());
            Assert.IsFalse(invalid.IsValidGtdExpiration());
            Assert.IsFalse(invalid1.IsValidGtdExpiration());
            Assert.IsFalse(invalid2.IsValidGtdExpiration());
            Assert.IsFalse(invalid3.IsValidGtdExpiration());
            Assert.IsFalse(invalid4.IsValidGtdExpiration());
        }

        /// <summary>
        /// A test for DaysSinceLastMarketDay
        /// </summary>
        [TestMethod]
        public void DaysSinceLastMarketDayTest_StaticWithDatePassed()
        {
            Assert.AreEqual(0, 
				FinanceDateTime.GetDaysSinceLastMarketDay(new DateTime(2008, 11, 13, 10, 0, 0)));
            Assert.AreEqual(2, 
				FinanceDateTime.GetDaysSinceLastMarketDay(new DateTime(2008, 11, 9, 10, 0, 0)));
        }

        /// <summary>
        /// A test for DaysSinceLastMarketDay
        /// </summary>
        [TestMethod]
        public void DaysSinceLastMarketDayTest_UsingInternalDate()
        {
            FinanceDateTime marketDay = new FinanceDateTime(new DateTime(2008, 11, 13, 10, 0, 0));
            FinanceDateTime nonMarketDay = new FinanceDateTime(new DateTime(2008, 11, 9, 10, 0, 0));

            Assert.AreEqual(0, marketDay.GetDaysSinceLastMarketDay());
            Assert.AreEqual(2, nonMarketDay.GetDaysSinceLastMarketDay());
        }

        /// <summary>
        /// A test for FinanceDateTime Constructor
        /// </summary>
        [TestMethod]
        public void FinanceDateTimeConstructorTest_Default()
        {
            FinanceDateTime fdt = new FinanceDateTime();

            Assert.AreEqual(DateTime.Now, fdt.DateTimeStamp);
        }

        /// <summary>
        /// A test for FinanceDateTime Constructor
        /// </summary>
        [TestMethod]
        public void FinanceDateTimeConstructorTest_WithDateTimePassed()
        {
            FinanceDateTime fdt = new FinanceDateTime(new DateTime(2008, 10, 11));
            var expected = new DateTime(2008, 10, 11);

            Assert.AreEqual(expected, fdt.DateTimeStamp);
        }

        /// <summary>
        /// A test for FinanceDateTime Constructor
        /// </summary>
        [TestMethod]
        public void FinanceDateTimeConstructorTest_WithDeltaPassedAndNoType()
        {
            FinanceDateTime fdt = new FinanceDateTime(2, 0);
            var expected = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0);

            Assert.AreEqual(expected, fdt.DateTimeStamp);
        }

        /// <summary>
        /// A test for FinanceDateTime Constructor
        /// </summary>
        [TestMethod]
        public void FinanceDateTimeConstructorTest_WithDeltaAndTypePassed()
        {
            FinanceDateTime fdt = new FinanceDateTime(2, 0, MarketTimeType.MarketOpen);
            var expected = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 30, 0);

            Assert.AreEqual(expected, fdt.DateTimeStamp);
        }

        /// <summary>
        /// A test for the static loading of the config
        /// </summary>
        [TestMethod]
        public void ConfigTest()
        {
            var christmasEve = new FinanceDateTime.HoursConfiguration.Date(2008, 12, 24);
            var newYearsEve = new FinanceDateTime.HoursConfiguration.Date(2008, 12, 31);
            var specialHoliday = new FinanceDateTime.HoursConfiguration.Date(2008, 12, 11);
            var halfDayMarketClose = new FinanceDateTime.HoursConfiguration.Time(13, 0);
            var halfDayExtendedMarketOpen = new FinanceDateTime.HoursConfiguration.Time(13, 0);
            var halfDayExtendedMarketClose = new FinanceDateTime.HoursConfiguration.Time(17, 0);
            var preMarketOpen = new FinanceDateTime.HoursConfiguration.Time(7, 0);
            var preMarketClose = new FinanceDateTime.HoursConfiguration.Time(9, 30);
            var marketOpen = new FinanceDateTime.HoursConfiguration.Time(9, 30);
            var marketClose = new FinanceDateTime.HoursConfiguration.Time(16, 0);
            var extendedMarketOpen = new FinanceDateTime.HoursConfiguration.Time(16, 0);
            var extendedMarketClose = new FinanceDateTime.HoursConfiguration.Time(20, 0);

            Assert.AreEqual(halfDayMarketClose, FinanceDateTime.Configuration.HalfDayMarketClose);
            Assert.AreEqual(halfDayExtendedMarketOpen, 
				FinanceDateTime.Configuration.HalfDayExtendedMarketOpen);
            Assert.AreEqual(halfDayExtendedMarketClose, 
				FinanceDateTime.Configuration.HalfDayExtendedMarketClose);
            Assert.AreEqual(preMarketOpen, FinanceDateTime.Configuration.PreMarketOpen);
            Assert.AreEqual(preMarketClose, FinanceDateTime.Configuration.PreMarketClose);
            Assert.AreEqual(marketOpen, FinanceDateTime.Configuration.MarketOpen);
            Assert.AreEqual(marketClose, FinanceDateTime.Configuration.MarketClose);
            Assert.AreEqual(extendedMarketOpen, FinanceDateTime.Configuration.ExtendedMarketOpen);
            Assert.AreEqual(extendedMarketClose, FinanceDateTime.Configuration.ExtendedMarketClose);
            Assert.AreEqual(2, FinanceDateTime.Configuration.HalfDays.Count);
            Assert.AreEqual(christmasEve, FinanceDateTime.Configuration.HalfDays[0]);
            Assert.AreEqual(newYearsEve, FinanceDateTime.Configuration.HalfDays[1]);
            Assert.AreEqual(1, FinanceDateTime.Configuration.SpecialHolidays.Count);
            Assert.AreEqual(specialHoliday, FinanceDateTime.Configuration.SpecialHolidays[0]);
        }

        /// <summary>
        /// A test for GetGtcExpireDateTimeFromOrderDate
        /// </summary>
        [TestMethod]
        public void GetGtcExpireDateTimeFromOrderDateTest_HalfDayExpireDate()
        {
            var actual = FinanceDateTime.GetGtcExpirationFromOrderDate(
				new DateTime(2008, 11, 12), MarketCloseType.Equity);
            var expected = new DateTime(2008, 12, 31, 13, 0, 0);
            Assert.AreEqual(expected, actual);

            var actual1 = FinanceDateTime.GetGtcExpirationFromOrderDate(
				new DateTime(2008, 11, 12), MarketCloseType.IndexSecurity);
            var expected1 = new DateTime(2008, 12, 31, 13, 15, 0);
            Assert.AreEqual(expected1, actual1);
        }

        /// <summary>
        /// A test for GetGtcExpireDateTimeFromOrderDate
        /// </summary>
        [TestMethod]
        public void GetGtcExpireDateTimeFromOrderDateTest_FullDayExpireDate()
        {
            var actual = FinanceDateTime.GetGtcExpirationFromOrderDate(
				new DateTime(2008, 12, 12), MarketCloseType.Equity);
            var expected = new DateTime(2009, 1, 30, 16, 0, 0);
            Assert.AreEqual(expected, actual);

            var actual1 = FinanceDateTime.GetGtcExpirationFromOrderDate(
				new DateTime(2008, 12, 12), MarketCloseType.IndexSecurity);
            var expected1 = new DateTime(2009, 1, 30, 16, 15, 0);
            Assert.AreEqual(expected1, actual1);
        }

        /// <summary>
        /// A test for IsLastMarketDay
        /// </summary>
        [TestMethod]
        public void IsLastMarketDayTest_OverWeekend()
        {
            var then = new DateTime(2008, 11, 7, 12, 0, 0);
            var notThen = new DateTime(2008, 11, 9, 12, 0, 0);
            FinanceDateTime fdt = new FinanceDateTime(new DateTime(2008, 11, 10, 10, 0, 0));

            Assert.IsTrue(fdt.IsLastMarketDay(then));
            Assert.IsFalse(fdt.IsLastMarketDay(notThen));
        }

        /// <summary>
        /// A test for IsLastMarketDay
        /// </summary>
        [TestMethod]
        public void IsLastMarketDayTest_PreviousDay()
        {
            var then = new DateTime(2008, 11, 12, 12, 0, 0);
            var notThen = new DateTime(2008, 11, 11, 12, 0, 0);
            FinanceDateTime fdt = new FinanceDateTime(new DateTime(2008, 11, 13, 10, 0, 0));

            Assert.IsTrue(fdt.IsLastMarketDay(then));
            Assert.IsFalse(fdt.IsLastMarketDay(notThen));
        }


		/// <summary>
		/// A test for GetNextMarketOpen()
		/// </summary>
		[TestMethod]
		public void GetNextMarketOpen_RetrunsNextOpen()
		{
			var before = new DateTime(2010, 6, 17, 1, 0, 0);
			var during = new DateTime(2010, 6, 17, 11, 0, 0);
			var after = new DateTime(2010, 6, 17, 20, 0, 0);
			var onOpen = new DateTime(2010, 6, 17, 9, 30, 0);
			var onClose = new DateTime(2010, 6, 17, 16, 0, 0); 
			var expectedTomorrow = new DateTime(2010, 6, 18, 9, 30, 0);
			var expectedToday = new DateTime(2010, 6, 17, 9, 30, 0);

			Assert.AreEqual(expectedToday, FinanceDateTime.GetNextMarketOpen(before));
			Assert.AreEqual(expectedTomorrow, FinanceDateTime.GetNextMarketOpen(onOpen));
			Assert.AreEqual(expectedTomorrow, FinanceDateTime.GetNextMarketOpen(onClose));
			Assert.AreEqual(expectedTomorrow, FinanceDateTime.GetNextMarketOpen(during));
			Assert.AreEqual(expectedTomorrow, FinanceDateTime.GetNextMarketOpen(after));
		}
    }
}