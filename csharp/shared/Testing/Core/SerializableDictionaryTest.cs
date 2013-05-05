using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.Core.Xml;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Test fixture for serializer dictionary
    /// </summary>
    [TestClass]
    public class SerializableDictionaryTest
    {
		/// <summary>
		/// private test instance of dictionary
		/// </summary>
		private readonly SerializableDictionary<string, double> _sampleDictionary
			= new SerializableDictionary<string, double>
                  {
                      { "Key1", 1.1 },
                      { "Key2", 2.2 },
                      { "Key3", 3.3 }
                  };

		/// <summary>
		/// expected serialized xml
		/// </summary>
		private const string _serializedXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary><Entry><Key>" +
			"<string>Key1</string></Key><Value><double>1.1</double></Value></Entry>" +
			"<Entry><Key><string>Key2</string></Key><Value><double>2.2</double></Value>" +
			"</Entry><Entry><Key><string>Key3</string></Key><Value><double>3.3</double>" +
			"</Value></Entry></Dictionary>";

		/// <summary>
		/// expected serialized xml with duplicates
		/// </summary>
		private const string _duplicateSerializedXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary>" +
			"<Entry><Key><string>Key1</string></Key><Value><double>1.1</double></Value></Entry>" +
			"<Entry><Key><string>Key2</string></Key><Value><double>2.2</double></Value></Entry>" +
			"<Entry><Key><string>Key2</string></Key><Value><double>2.9</double></Value></Entry>" +
			"<Entry><Key><string>Key3</string></Key><Value><double>3.3</double>" +
			"</Value></Entry></Dictionary>";
		
		/// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Test that serialization works
        /// </summary>
        [TestMethod]
        public void TestSerializationWorks()
        {
            string s = XmlUtility.XmlFromType(_sampleDictionary);

            Assert.AreEqual(s, _serializedXml);
        }


        /// <summary>
        /// Test that duplicate serialization works
        /// </summary>
        [TestMethod]
        public void DuplicateSerializationWorks()
        {
            var d = XmlUtility.TypeFromXml<SerializableDictionary<string, double>>(_duplicateSerializedXml);

			Assert.AreEqual(3, d.Count);
            Assert.AreEqual(1.1, d["Key1"]);
            Assert.AreEqual(2.9, d["Key2"]);
            Assert.AreEqual(3.3, d["Key3"]);
        }


        /// <summary>
        /// Test that deserialization works
        /// </summary>
        [TestMethod]
        public void TestDeserializationWorks()
        {
            var d = XmlUtility.TypeFromXml<SerializableDictionary<string, double>>(_serializedXml);

            Assert.AreEqual(3, d.Count);
            Assert.AreEqual(1.1, d["Key1"]);
            Assert.AreEqual(2.2, d["Key2"]);
            Assert.AreEqual(3.3, d["Key3"]);
        }
    }
}