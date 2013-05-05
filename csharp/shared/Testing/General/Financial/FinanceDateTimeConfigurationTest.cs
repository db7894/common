using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.General.Financial.UnitTests
{
    /// <summary>
    /// This is a test class for FinanceDateTimeConfigurationTest and is intended
    /// to contain all FinanceDateTimeConfigurationTest Unit Tests
    /// </summary>
    [TestClass]
    public class FinanceDateTimeConfigurationTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for FinanceDateTime.HoursConfiguration Constructor
        /// </summary>
        [TestMethod]
        public void FinanceDateTimeConfigurationConstructorTest()
        {
            var halfDay = new FinanceDateTime.HoursConfiguration.Date(1970, 1, 1);
            var specialDay = new FinanceDateTime.HoursConfiguration.Date(2004, 6, 11);
            var halfDayMarketClose = new FinanceDateTime.HoursConfiguration.Time(13, 0);
            var halfDayExtendedMarketOpen = new FinanceDateTime.HoursConfiguration.Time(13, 0);
            var halfDayExtendedMarketClose = new FinanceDateTime.HoursConfiguration.Time(17, 0);
            var preMarketOpen = new FinanceDateTime.HoursConfiguration.Time(7, 0);
            var preMarketClose = new FinanceDateTime.HoursConfiguration.Time(9, 30);
            var marketOpen = new FinanceDateTime.HoursConfiguration.Time(9, 30);
            var marketClose = new FinanceDateTime.HoursConfiguration.Time(16, 0);
            var extendedMarketOpen = new FinanceDateTime.HoursConfiguration.Time(16, 0);
            var extendedMarketClose = new FinanceDateTime.HoursConfiguration.Time(20, 0);

            FinanceDateTime.HoursConfiguration config = new FinanceDateTime.HoursConfiguration(true);
            Assert.AreEqual(halfDayMarketClose, config.HalfDayMarketClose);
            Assert.AreEqual(halfDayExtendedMarketOpen, config.HalfDayExtendedMarketOpen);
            Assert.AreEqual(halfDayExtendedMarketClose, config.HalfDayExtendedMarketClose);
            Assert.AreEqual(preMarketOpen, config.PreMarketOpen);
            Assert.AreEqual(preMarketClose, config.PreMarketClose);
            Assert.AreEqual(marketOpen, config.MarketOpen);
            Assert.AreEqual(marketClose, config.MarketClose);
            Assert.AreEqual(extendedMarketOpen, config.ExtendedMarketOpen);
            Assert.AreEqual(extendedMarketClose, config.ExtendedMarketClose);
            Assert.AreEqual(0, config.HalfDays.Count);
            Assert.AreEqual(0, config.SpecialHolidays.Count);
        }

        /// <summary>
        /// A test for GetMinutesMarketIsOpenOnHalfDay
        /// </summary>
        [TestMethod]
        public void GetMinutesMarketIsOpenOnHalfDayTest()
        {
            FinanceDateTime.HoursConfiguration config = new FinanceDateTime.HoursConfiguration(true);
            double expectedMinutes = 3.5 * 60;
            double actualMinutes = config.GetMinutesMarketIsOpenOnHalfDay();

            Assert.AreEqual(expectedMinutes, actualMinutes);
        }

        /// <summary>
        /// A test for GetMinutesMarketIsClosedOnHalfDay
        /// </summary>
        [TestMethod]
        public void GetMinutesMarketIsClosedOnHalfDayTest()
        {
            FinanceDateTime.HoursConfiguration config = new FinanceDateTime.HoursConfiguration(true);
            double expectedMinutes = 3 * 60;
            double actualMinutes = config.GetMinutesMarketIsClosedOnHalfDay();

            Assert.AreEqual(expectedMinutes, actualMinutes);
        }


        /// <summary>
        /// test the Serialization to xml is same as legacy xml
        /// </summary>
        [TestMethod]
        public void SerializationToXml_IsSameAsLegacy_OnToXml()
        {
            FinanceDateTime.HoursConfiguration config = new FinanceDateTime.HoursConfiguration
                {
                    HalfDays = { new FinanceDateTime.HoursConfiguration.Date(1970, 1, 1) },
                    SpecialHolidays = { new FinanceDateTime.HoursConfiguration.Date(2004, 6, 11) },
                    HalfDayMarketClose = new FinanceDateTime.HoursConfiguration.Time(13, 0),
                    HalfDayExtendedMarketOpen = new FinanceDateTime.HoursConfiguration.Time(13, 15),
                    HalfDayExtendedMarketClose = new FinanceDateTime.HoursConfiguration.Time(17, 0),
                    PreMarketOpen = new FinanceDateTime.HoursConfiguration.Time(8, 0),
                    PreMarketClose = new FinanceDateTime.HoursConfiguration.Time(9, 15),
                    MarketOpen = new FinanceDateTime.HoursConfiguration.Time(9, 30),
                    MarketClose = new FinanceDateTime.HoursConfiguration.Time(16, 0),
                    ExtendedMarketOpen = new FinanceDateTime.HoursConfiguration.Time(16, 15),
                    ExtendedMarketClose = new FinanceDateTime.HoursConfiguration.Time(20, 0)
                };

            // this is a stringified example of the older configuration format.
            const string expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FinanceDateTimeConf" 
				+ "iguration xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"ht" 
				+ "tp://www.w3.org/2001/XMLSchema\"><HalfDays><Date><Year>1970</Year><Month>1</Mont" 
				+ "h><Day>1</Day></Date></HalfDays><SpecialHolidays><Date><Year>2004</Year><Month>6" 
				+ "</Month><Day>11</Day></Date></SpecialHolidays><HalfDayMarketClose><Hour>13</Hour" 
				+ "><Minute>0</Minute></HalfDayMarketClose><HalfDayExtendedMarketOpen><Hour>13</Hou" 
				+ "r><Minute>15</Minute></HalfDayExtendedMarketOpen><HalfDayExtendedMarketClose><Ho" 
				+ "ur>17</Hour><Minute>0</Minute></HalfDayExtendedMarketClose><PreMarketOpen><Hour>" 
				+ "8</Hour><Minute>0</Minute></PreMarketOpen><PreMarketClose><Hour>9</Hour><Minute>" 
				+ "15</Minute></PreMarketClose><MarketOpen><Hour>9</Hour><Minute>30</Minute></Marke" 
				+ "tOpen><MarketClose><Hour>16</Hour><Minute>0</Minute></MarketClose><ExtendedMarke" 
				+ "tOpen><Hour>16</Hour><Minute>15</Minute></ExtendedMarketOpen><ExtendedMarketClos" 
				+ "e><Hour>20</Hour><Minute>0</Minute></ExtendedMarketClose></FinanceDateTimeConfig" 
				+ "uration>";

            Assert.AreEqual(expected, config.ToXml(false));
        }
    }
}