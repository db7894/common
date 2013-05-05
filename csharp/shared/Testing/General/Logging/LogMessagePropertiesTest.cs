using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Logging.UnitTests
{
	/// <summary>
	/// A test class for LogMessageProperties
	/// </summary>
	[TestClass]
	public class LogMessagePropertiesTest
	{
		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void ConstructorCreatesEmptyCollection()
		{
			var target = new LogMessageProperties();

			Assert.AreEqual(0, target.Count());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void ConstructedObjectYieldsEmptyBraces()
		{
			var target = new LogMessageProperties();

			Assert.AreEqual("{}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void AddedObjectYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target.Add("A", 123.45);

			Assert.AreEqual("{A=\"123.45\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void InitializerWithOneItemYieldsItemInCurlies()
		{
			var target = new LogMessageProperties { { "A", 123.45 } };

			Assert.AreEqual("{A=\"123.45\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddedNullKeyYieldsException()
		{
			var target = new LogMessageProperties();
			target.Add(null, 123.45);
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InitializerNullKeyYieldsException()
		{
			new LogMessageProperties { { null, 123.45 } };
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void IndexedObjectYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target["A"] = 123.45;

			Assert.AreEqual("{A=\"123.45\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void PropertyAccountYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target.Account = "11111178";

			Assert.AreEqual("{Account=\"11111178\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void IndexerWithAccountNameObjectYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target[LogMessageProperties.AccountKey] = "11111178";

			Assert.AreEqual("{Account=\"11111178\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void PropertyUserYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target.User = "11111178";

			Assert.AreEqual("{User=\"11111178\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void IndexerWithUserNameObjectYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target[LogMessageProperties.UserKey] = "11111178";

			Assert.AreEqual("{User=\"11111178\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void PropertyIpYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target.Ip = "11111178";

			Assert.AreEqual("{Ip=\"11111178\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void IndexerWithIpNameObjectYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target[LogMessageProperties.IpKey] = "11111178";

			Assert.AreEqual("{Ip=\"11111178\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void PropertyEventYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target.Event = "11111178";

			Assert.AreEqual("{Event=\"11111178\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void IndexerWithEventNameObjectYieldsItemInCurlies()
		{
			var target = new LogMessageProperties();
			target[LogMessageProperties.EventKey] = "11111178";

			Assert.AreEqual("{Event=\"11111178\"}", target.ToString());
		}


		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void ItemWithNullValueYieldsNullItemInCurlies()
		{
			var target = new LogMessageProperties();
			target["A"] = null;

			Assert.AreEqual("{A=null}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void MultipleIndexItemsYieldCurliedList()
		{
			var target = new LogMessageProperties();
			target["A"] = null;
			target["B"] = "Hello";
			target["C"] = "Nuts";
			target["D"] = 13;

			Assert.AreEqual("{A=null|B=\"Hello\"|C=\"Nuts\"|D=\"13\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void MultipleAddItemsYieldCurliedList()
		{
			var target = new LogMessageProperties();
			target.Add("A", null);
			target.Add("B", "Hello");
			target.Add("C", "Nuts");
			target.Add("D", 13);

			Assert.AreEqual("{A=null|B=\"Hello\"|C=\"Nuts\"|D=\"13\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void MultipleInitializerItemsYieldCurliedList()
		{
			var target = new LogMessageProperties { { "A", null }, { "B", "Hello" }, { "C", "Nuts" }, { "D", 13 } };

			Assert.AreEqual("{A=null|B=\"Hello\"|C=\"Nuts\"|D=\"13\"}", target.ToString());
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void GetterReturnsAssignedValue()
		{
			var target = new LogMessageProperties();
			target["A"] = 13;

			Assert.AreEqual(13, target["A"]);
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void GetterWithAddReturnsAssignedValue()
		{
			var target = new LogMessageProperties();
			target.Add("A", 13);

			Assert.AreEqual(13, target["A"]);
		}

		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetterWithNoAddYieldsExceptionValue()
		{
			var target = new LogMessageProperties();

			Assert.IsNull(target["A"]);
		}


		/// <summary>
		/// A test method for the LogMessageProperties class
		/// </summary>
		[TestMethod]
		public void ToStringScrubsInvalidCharacters()
		{
			var target = new LogMessageProperties();
			target.Add("Key|=\"{}", "Value|=\"{}");

			Assert.AreEqual("{Key/-'[]=\"Value/-'[]\"}", target.ToString());
		}
	}
}
