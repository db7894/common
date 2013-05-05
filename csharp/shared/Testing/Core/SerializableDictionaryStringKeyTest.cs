using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.Core.Xml;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// Test fixture for serializer dictionary
	/// </summary>
	[TestClass]
	public class SerializableDictionaryOfStringTest
	{
		/// <summary>
		/// private test instance of dictionary
		/// </summary>
		private readonly SerializableDictionary<int> _sampleDictionary = new SerializableDictionary<int>
                  {
                      { "Key1", 1 },
                      { "Key2", 2 },
                      { "Key3", 3 }
                  };

		/// <summary>
		/// expected serialized xml
		/// </summary>
		private const string _serializedXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary>" +
			"<Entry Key=\"Key1\"><Value><int>1</int></Value></Entry>" +
			"<Entry Key=\"Key2\"><Value><int>2</int></Value></Entry>" +
			"<Entry Key=\"Key3\"><Value><int>3</int></Value></Entry>" +
			"</Dictionary>";

		/// <summary>
		/// expected serialized xml
		/// </summary>
		private const string _emptyXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary></Dictionary>";

		/// <summary>
		/// expected serialized xml
		/// </summary>
		private const string _emptySelfClosingXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary/>";

		/// <summary>
		/// expected serialized xml
		/// </summary>
		private const string _serializedNonEmptyXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary>" +
			"<Entry Key=\"Key1\"><Value><int>1</int></Value></Entry>" +
			"<Entry Key=\"Key2\"><Value><int>2</int></Value></Entry>" +
			"<Entry Key=\"Key3\"><Value><int>3</int></Value></Entry>" +
			"</Dictionary>";

		/// <summary>
		/// expected serialized xml
		/// </summary>
		private const string _serializedMixedXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary>" +
			"<Entry Key=\"Key1\"><Value><int>1</int></Value></Entry>" +
			"<Entry Key=\"Key2\"><Value><int>2</int></Value></Entry>" +
			"<Entry Key=\"Key3\"><Value><int>3</int></Value></Entry>" +
			"</Dictionary>";

		/// <summary>
		/// expected serialized xml with duplicates
		/// </summary>
		private const string _duplicateSerializedXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?><Dictionary>" +
			"<Entry Key=\"Key1\"><Value><int>1</int></Value></Entry>" +
			"<Entry Key=\"Key2\"><Value><int>2</int></Value></Entry>" +
			"<Entry Key=\"Key2\"><Value><int>4</int></Value></Entry>" +
			"<Entry Key=\"Key3\"><Value><int>3</int></Value></Entry>" +
			"</Dictionary>";

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
			var d = XmlUtility.TypeFromXml<SerializableDictionary<int>>(_duplicateSerializedXml);

			Assert.AreEqual(3, d.Count);
			Assert.AreEqual(1, d["Key1"]);
			Assert.AreEqual(4, d["Key2"]);
			Assert.AreEqual(3, d["Key3"]);
		}


		/// <summary>
		/// Test that deserialization works
		/// </summary>
		[TestMethod]
		public void TestEmptyDeserializationWorks()
		{
			var d = XmlUtility.TypeFromXml<SerializableDictionary<int>>(_emptyXml);

			Assert.AreEqual(0, d.Count);
		}


		/// <summary>
		/// Test that deserialization works
		/// </summary>
		[TestMethod]
		public void TestEmptySelfClosingDeserializationWorks()
		{
			var d = XmlUtility.TypeFromXml<SerializableDictionary<int>>(_emptySelfClosingXml);

			Assert.AreEqual(0, d.Count);
		}


		/// <summary>
		/// Test that deserialization works
		/// </summary>
		[TestMethod]
		public void TestDeserializationWorks()
		{
			var d = XmlUtility.TypeFromXml<SerializableDictionary<int>>(_serializedXml);

			Assert.AreEqual(3, d.Count);
			Assert.AreEqual(1, d["Key1"]);
			Assert.AreEqual(2, d["Key2"]);
			Assert.AreEqual(3, d["Key3"]);
		}


		/// <summary>
		/// Test that deserialization works
		/// </summary>
		[TestMethod]
		public void TestNotSelfClosingDeserializationWorks()
		{
			var d = XmlUtility.TypeFromXml<SerializableDictionary<int>>(_serializedNonEmptyXml);

			Assert.AreEqual(3, d.Count);
			Assert.AreEqual(1, d["Key1"]);
			Assert.AreEqual(2, d["Key2"]);
			Assert.AreEqual(3, d["Key3"]);
		}


		/// <summary>
		/// Test that deserialization works
		/// </summary>
		[TestMethod]
		public void TestMixedDeserializationWorks()
		{
			var d = XmlUtility.TypeFromXml<SerializableDictionary<int>>(_serializedMixedXml);

			Assert.AreEqual(3, d.Count);
			Assert.AreEqual(1, d["Key1"]);
			Assert.AreEqual(2, d["Key2"]);
			Assert.AreEqual(3, d["Key3"]);
		}
	}
}