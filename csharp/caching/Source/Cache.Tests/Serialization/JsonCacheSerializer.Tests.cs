using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Caching.Tests.Types;

namespace SharedAssemblies.General.Caching.Serialization.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// JsonCacheSerializer.
	/// </summary>
	[TestClass]
	public class JsonCacheSerializerTests
	{
		[TestMethod]
		public void Serializer_Initailizes_Correctly()
		{
			var serializer = new JsonCacheSerializer();

			Assert.IsNotNull(serializer);
		}

		[TestMethod]
		public void Serializer_Serializes_Correctly()
		{
			var serializer = new JsonCacheSerializer();
			var instance = new SerializeType
			{
				Age = 25,
				Birthday = new DateTime(1, 2, 3),
				Name = "Some Name",
			};
			var expected = "{\"Age\":25,\"Birthday\":\"\\/Date(-62132724000000-0600)\\/\",\"Name\":\"Some Name\"}";
			var actual = serializer.Serialize(instance);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Serializer_Deserializes_Correctly()
		{
			var serializer = new JsonCacheSerializer();
			var instance = "{\"Age\":25,\"Birthday\":\"\\/Date(-62132724000000-0600)\\/\",\"Name\":\"Some Name\"}";
			var actual = serializer.Deserialize<SerializeType>(instance);

			Assert.AreEqual(25, actual.Age);
			Assert.AreEqual(new DateTime(1, 2, 3), actual.Birthday);
			Assert.AreEqual("Some Name", actual.Name);
		}
	}
}
