using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Financial.UnitTests
{
    /// <summary>
    /// Test fixture for time
    /// </summary>
    [TestClass]
    public class TimeTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Test for constructor
        /// </summary>
        [TestMethod]
        public void Constructor_SetsProperties_OnConstruction()
        {
            FinanceDateTime.HoursConfiguration.Time d = new FinanceDateTime.HoursConfiguration.Time(1, 2);

            Assert.AreEqual(1, d.Hour);
            Assert.AreEqual(2, d.Minute);
        }


        /// <summary>
        /// Test constructor defaults
        /// </summary>
        [TestMethod]
        public void Constructor_DefaultsProperties_OnConstruction()
        {
            FinanceDateTime.HoursConfiguration.Time d = new FinanceDateTime.HoursConfiguration.Time();

            Assert.AreEqual(0, d.Hour);
            Assert.AreEqual(0, d.Minute);
        }


        /// <summary>
        /// Test hour sets and gets
        /// </summary>
        [TestMethod]
        public void Hour_SetsAndGets_OnUse()
        {
            FinanceDateTime.HoursConfiguration.Time d = new FinanceDateTime.HoursConfiguration.Time();
            d.Hour = 12;

            Assert.AreEqual(12, d.Hour);
        }
        

        /// <summary>
        /// Test minute sets and gets
        /// </summary>
        [TestMethod]
        public void Minute_SetsAndGets_OnUse()
        {
            FinanceDateTime.HoursConfiguration.Time d = new FinanceDateTime.HoursConfiguration.Time();
            d.Minute = 30;

            Assert.AreEqual(30, d.Minute);
        }

		/// <summary>
		/// Tests the ToString() method
		/// </summary>
		[TestMethod]
		public void ToStringTest()
		{
			FinanceDateTime.HoursConfiguration.Time d = new FinanceDateTime.HoursConfiguration.Time(3, 15);

			Assert.AreEqual("3:15 AM", d.ToString());
		}
    }
}