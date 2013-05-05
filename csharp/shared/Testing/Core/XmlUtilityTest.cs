using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Xml;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Unit tests relating to XmlUtility
    /// </summary>
    [TestClass]
    public class XmlUtilityTest
    {
        /// <summary>
        /// MSTest testing context
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Test XmlFromType returns string
        /// </summary>
        [TestMethod]
        public void XmlFromType_ReturnsXmlString_WhenPassedObject()
        {
            string xml = XmlUtility.XmlFromType(new XmlUtilityTestClass(42, 3.14, "Hello World!"));

            Assert.AreEqual(xml, 
				"<?xml version=\"1.0\" encoding=\"utf-8\"?><XmlUtilityTestClass xmlns:xsi=\"http://" 
				+ "www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchem" 
				+ "a\"><MyInt>42</MyInt><MyDouble>3.14</MyDouble><MyString>Hello World!</MyString><" 
				+ "/XmlUtilityTestClass>");
        }


        /// <summary>
        /// Test TypeFromXml returns object
        /// </summary>
        [TestMethod]
        public void TypeFromXml_ReturnsObject_WhenPassedXmlString()
        {
            var oldItem = new XmlUtilityTestClass(42, 3.14, "Hello World!");

            var xml = XmlUtility.XmlFromType(oldItem);

            var newItem = XmlUtility.TypeFromXml<XmlUtilityTestClass>(xml);

            Assert.AreEqual(oldItem, newItem);
        }


		/// <summary>
		/// Tests TypeFromXml to see if will load from an xml reader
		/// </summary>
		[TestMethod]
		public void TypeFromXml_ReturnsObject_WhenPassedXmlTestReader()
		{
			var oldItem = new XmlUtilityTestClass(42, 3.14, "Hello World!");

			var xs = new XmlSerializer(oldItem.GetType());

			// use new UTF8Encoding here, not Encoding.UTF8.  The later includes
			// the BOM which screws up subsequent reads, the former does not.
			using (var memoryStream = new MemoryStream())
			using (var xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding()))
			{
				xs.Serialize(xmlTextWriter, oldItem);
				memoryStream.Position = 0;

				using (var xmlTextReader = new XmlTextReader(memoryStream))
				{
					Assert.AreEqual(oldItem, XmlUtility.TypeFromXml<XmlUtilityTestClass>(xmlTextReader));
				}
			}
		}

	
		/// <summary>
		/// Tests TypeFromXml to see if will load from a stream
		/// </summary>
		[TestMethod]
		public void TypeFromXml_ReturnsObject_WhenPassedStream()
		{
			var oldItem = new XmlUtilityTestClass(42, 3.14, "Hello World!");

			var xs = new XmlSerializer(oldItem.GetType());

			// use new UTF8Encoding here, not Encoding.UTF8.  The later includes
			// the BOM which screws up subsequent reads, the former does not.
			using (var memoryStream = new MemoryStream())
			{
				xs.Serialize(memoryStream, oldItem);
				memoryStream.Position = 0;

				Assert.AreEqual(oldItem, XmlUtility.TypeFromXml<XmlUtilityTestClass>(memoryStream));
			}
		}


		/// <summary>
		/// Check to see if XmlDocumentFromType returns a document.
		/// </summary>
		[TestMethod]
		public void XmlDocumentFromType_ReturnsXmlDocument_WhenPassedObject()
		{
			var oldItem = new XmlUtilityTestClass(42, 3.14, "Hello World!");

			var document = XmlUtility.XmlDocumentFromType(oldItem);

			Assert.AreEqual(document.OuterXml, XmlUtility.XmlFromType(oldItem));
		}


		/// <summary>
		/// Check to see if XmlDocumentFromType returns a document.
		/// </summary>
		[TestMethod]
        [Obsolete("System.Xml.XmlDataDocument is obsolete.  This will be removed in later versions.", false)]
		public void XmlDataDocumentFromType_ReturnsXmlDocument_WhenPassedObject()
		{
			var oldItem = new XmlUtilityTestClass(42, 3.14, "Hello World!");

			var document = XmlUtility.XmlDataDocumentFromType(oldItem);

			Assert.AreEqual(document.OuterXml, XmlUtility.XmlFromType(oldItem));
		}
	}
}