using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Core.Xml;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Series of unit tests for the XmlExtensions static class
    /// </summary>
    [TestClass]
    public class XmlExtensionsTest
    {
        /// <summary>
        /// The testing context for MS Test
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Test ToXml() creates xml from type
        /// </summary>
        [TestMethod]
        public void TypeToXml_CreatesXml_FromType()
        {
            XmlUtilityTestClass target = new XmlUtilityTestClass(42, 3.14, "Test Me!");
            string expected = XmlUtility.XmlFromType(target);

            string actual = target.ToXml();

            Assert.AreEqual(expected, actual);            
        }
    }
}
