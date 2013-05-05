using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Financial.UnitTests
{
    /// <summary>
    /// Test fixture for Date
    /// </summary>
    [TestClass]
    public class DateTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Test the constructor
        /// </summary>
        [TestMethod]
        public void Constructor_SetsProperties_OnConstruction()
        {
            FinanceDateTime.HoursConfiguration.Date d = new FinanceDateTime.HoursConfiguration.Date(1, 2, 3);

            Assert.AreEqual(1, d.Year);
            Assert.AreEqual(2, d.Month);
            Assert.AreEqual(3, d.Day);
        }


        /// <summary>
        /// Test constructor
        /// </summary>
        [TestMethod]
        public void Constructor_DefaultsProperties_OnConstruction()
        {
            FinanceDateTime.HoursConfiguration.Date d = new FinanceDateTime.HoursConfiguration.Date();

            Assert.AreEqual(0, d.Year);
            Assert.AreEqual(0, d.Month);
            Assert.AreEqual(0, d.Day);            
        }


        /// <summary>
        /// Test propery sets and gets
        /// </summary>
        [TestMethod]
        public void Year_SetsAndGets_OnUse()
        {
            FinanceDateTime.HoursConfiguration.Date d = new FinanceDateTime.HoursConfiguration.Date();
            d.Year = 1942;

            Assert.AreEqual(1942, d.Year);
        }


        /// <summary>
        /// test month sets and gets
        /// </summary>
        [TestMethod]
        public void Month_SetsAndGets_OnUse()
        {
            FinanceDateTime.HoursConfiguration.Date d = new FinanceDateTime.HoursConfiguration.Date();
            d.Month = 12;

            Assert.AreEqual(12, d.Month);
        }


        /// <summary>
        /// Test day sets and gets
        /// </summary>
        [TestMethod]
        public void Day_SetsAndGets_OnUse()
        {
            FinanceDateTime.HoursConfiguration.Date d = new FinanceDateTime.HoursConfiguration.Date();
            d.Day = 31;

            Assert.AreEqual(31, d.Day);
        }
    }
}