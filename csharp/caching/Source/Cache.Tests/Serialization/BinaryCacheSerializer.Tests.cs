using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Caching.Tests.Types;

namespace SharedAssemblies.General.Caching.Serialization.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// BinaryCacheSerializer.
	/// </summary>
	[TestClass]
	public class BinaryCacheSerializerTests
	{
		[TestMethod]
		public void Serializer_Initailizes_Correctly()
		{
			var serializer = new BinaryCacheSerializer();

			Assert.IsNotNull(serializer);
		}

		[TestMethod]
		public void Serializer_Serializes_Correctly()
		{
			var serializer = new BinaryCacheSerializer();
			var instance = new SerializeType
			{
				Age = 25,
				Birthday = new DateTime(1,2,3),
				Name = "Some Name",
			};
			var expected = @"AAEAAAD/////AQAAAAAAAAAMAgAAAF1TaGFyZWRBc3NlbWJsaWVzLkdlbmVyYWwuQ2FjaGluZy"
				+ @"5UZXN0cywgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwF"
				+ @"AQAAADpTaGFyZWRBc3NlbWJsaWVzLkdlbmVyYWwuQ2FjaGluZy5UZXN0cy5UeXBlcy5TZXJpYWxpemVUeXB"
				+ @"lAwAAAAROYW1lA0FnZQhCaXJ0aGRheQEAAAgNAgAAAAYDAAAACVNvbWUgTmFtZRkAAAAAwKF37hkAAAs=";

			var actual = serializer.Serialize(instance);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Serializer_Deserializes_Correctly()
		{
			var serializer = new BinaryCacheSerializer();
			var instance = @"AAEAAAD/////AQAAAAAAAAAMAgAAAF1TaGFyZWRBc3NlbWJsaWVzLkdlbmVyYWwuQ2FjaGluZy"
				+ @"5UZXN0cywgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwF"
				+ @"AQAAADpTaGFyZWRBc3NlbWJsaWVzLkdlbmVyYWwuQ2FjaGluZy5UZXN0cy5UeXBlcy5TZXJpYWxpemVUeXB"
				+ @"lAwAAAAROYW1lA0FnZQhCaXJ0aGRheQEAAAgNAgAAAAYDAAAACVNvbWUgTmFtZRkAAAAAwKF37hkAAAs=";

			var actual = serializer.Deserialize<SerializeType>(instance);

			Assert.AreEqual(25, actual.Age);
			Assert.AreEqual(new DateTime(1, 2, 3), actual.Birthday);
			Assert.AreEqual("Some Name", actual.Name);
		}
	}
}
