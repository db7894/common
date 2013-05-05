using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Database.Exceptions;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// QueueDataParsingExceptionTest test fixture
    /// </summary>
    [TestClass]
    public class QueueDataParsingExceptionTest
    {
        /// <summary>
        /// Constructor defaults correctly on construction
        /// </summary>
        [TestMethod]
        public void Constructor_Defaults_OnConstruction()
        {
            QueueDataParsingException ex = new QueueDataParsingException("x", "y");

            Assert.AreEqual(ex.RecordString, "y");
            Assert.AreEqual(ex.Message, "x");
            Assert.IsNull(ex.InnerException);
        }
    }
}
