using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Caching.Tests.Types;

namespace SharedAssemblies.General.Caching.Serialization.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// XmlCacheSerializer.
	/// </summary>
	[TestClass]
	public class XmlCacheSerializerTests
	{
		[TestMethod]
		public void Serializer_Initailizes_Correctly()
		{
			var serializer = new XmlCacheSerializer();

			Assert.IsNotNull(serializer);
		}

		[TestMethod]
		public void Serializer_Serializes_Correctly()
		{
			var serializer = new XmlCacheSerializer();
			var instance = new SerializeType
			{
				Age = 25,
				Birthday = new DateTime(1, 2, 3),
				Name = "Some Name",
			};
			var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
				+ "<SerializeType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">"
				+ "<Name>Some Name</Name><Age>25</Age><Birthday>0001-02-03T00:00:00</Birthday></SerializeType>";
			var actual = serializer.Serialize(instance);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Serializer_Deserializes_Correctly()
		{
			var serializer = new XmlCacheSerializer();
			var instance = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
				+ "<SerializeType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">"
				+ "<Name>Some Name</Name><Age>25</Age><Birthday>0001-02-03T00:00:00</Birthday></SerializeType>";
			var actual = serializer.Deserialize<SerializeType>(instance);

			Assert.AreEqual(25, actual.Age);
			Assert.AreEqual(new DateTime(1, 2, 3), actual.Birthday);
			Assert.AreEqual("Some Name", actual.Name);
		}
	}
}
