using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.DataContracts;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
	internal class SomeThing : Dictionary<string, Point>
	{
	}

	/// <summary>
	/// Unit tests related to the DataContractUtility class
	/// </summary>
	[TestClass]
	public class DataContractUtilityTest
	{
		/// <summary>
		/// The test context property
		/// </summary>
		public TestContext TestContext { get; set; }
		
		/// <summary>
		/// Test ToJson
		/// </summary>
		[TestMethod]
		public void JsonSerializationWorks()
		{
			var stuff = new SomeThing
			            	{
			            		{ "A", new Point { X = 1, Y = 1 } },
			            		{ "Z", new Point { X = 5, Y = 2 } },
			            		{ "B", new Point { X = 2, Y = 3 } },
			            		{ "D", new Point { X = 3, Y = 4 } },
			            		{ "F", new Point { X = 4, Y = 5 } },
			            	};

			var json = DataContractUtility.ToJson(stuff);

			var reconstituted = DataContractUtility.FromJson<Dictionary<string, Point>>(json);

			Assert.AreEqual(stuff.Count, reconstituted.Count);

			foreach (var expected in stuff)
			{
				Assert.IsTrue(reconstituted.ContainsKey(expected.Key));
				Assert.AreEqual(expected.Value.X, reconstituted[expected.Key].X);
				Assert.AreEqual(expected.Value.Y, reconstituted[expected.Key].Y);
			}
		}

		/// <summary>
		/// Test ToJXml
		/// </summary>
		[TestMethod]
		public void XmlSerializationWorks()
		{
			var stuff = new Dictionary<string, Point>
			            	{
			            		{ "A", new Point { X = 1, Y = 1 } },
			            		{ "Z", new Point { X = 5, Y = 2 } },
			            		{ "B", new Point { X = 2, Y = 3 } },
			            		{ "D", new Point { X = 3, Y = 4 } },
			            		{ "F", new Point { X = 4, Y = 5 } },
			            	};

			var xml = DataContractUtility.ToXml(stuff);

			var reconstituted = DataContractUtility.FromXml<Dictionary<string, Point>>(xml);

			Assert.AreEqual(stuff.Count, reconstituted.Count);

			foreach (var expected in stuff)
			{
				Assert.IsTrue(reconstituted.ContainsKey(expected.Key));
				Assert.AreEqual(expected.Value.X, reconstituted[expected.Key].X);
				Assert.AreEqual(expected.Value.Y, reconstituted[expected.Key].Y);
			}
		}

		/// <summary>
		/// Test ToJson
		/// </summary>
		[TestMethod]
		public void ToJsonExtensionSerializationWorks()
		{
			var stuff = new SomeThing
			            	{
			            		{ "A", new Point { X = 1, Y = 1 } },
			            		{ "Z", new Point { X = 5, Y = 2 } },
			            		{ "B", new Point { X = 2, Y = 3 } },
			            		{ "D", new Point { X = 3, Y = 4 } },
			            		{ "F", new Point { X = 4, Y = 5 } },
			            	};

			var json = stuff.ToDataContractJson();

			var reconstituted = DataContractUtility.FromJson<Dictionary<string, Point>>(json);

			Assert.AreEqual(stuff.Count, reconstituted.Count);

			foreach (var expected in stuff)
			{
				Assert.IsTrue(reconstituted.ContainsKey(expected.Key));
				Assert.AreEqual(expected.Value.X, reconstituted[expected.Key].X);
				Assert.AreEqual(expected.Value.Y, reconstituted[expected.Key].Y);
			}
		}

		/// <summary>
		/// Test ToJXml
		/// </summary>
		[TestMethod]
		public void ToXmlExtensionSerializationWorks()
		{
			var stuff = new Dictionary<string, Point>
			            	{
			            		{ "A", new Point { X = 1, Y = 1 } },
			            		{ "Z", new Point { X = 5, Y = 2 } },
			            		{ "B", new Point { X = 2, Y = 3 } },
			            		{ "D", new Point { X = 3, Y = 4 } },
			            		{ "F", new Point { X = 4, Y = 5 } },
			            	};

			var xml = stuff.ToDataContractXml();

			var reconstituted = DataContractUtility.FromXml<Dictionary<string, Point>>(xml);

			Assert.AreEqual(stuff.Count, reconstituted.Count);

			foreach (var expected in stuff)
			{
				Assert.IsTrue(reconstituted.ContainsKey(expected.Key));
				Assert.AreEqual(expected.Value.X, reconstituted[expected.Key].X);
				Assert.AreEqual(expected.Value.Y, reconstituted[expected.Key].Y);
			}
		}
	}
}
