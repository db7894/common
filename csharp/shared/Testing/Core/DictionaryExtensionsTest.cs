using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// The set of unit tests for the DictionaryExtensions class
	/// </summary>
	[TestClass]
	public class DictionaryExtensionsTest
	{
		[TestMethod]
		public void SafeGetWithMatchingValueReturnsSuccess()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "key", "value" }
			};

			var actual = dictionary.SafeGet("key");

			Assert.AreEqual(true, actual.IsSuccessful);
			Assert.AreEqual("value", actual.Value);
		}

		[TestMethod]
		public void SafeGetWithNoMatchingValueReturnsFailure()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{ "key", new object() }
			};

			var actual = dictionary.SafeGet("nonexistant");

			Assert.AreEqual(false, actual.IsSuccessful);
			Assert.AreEqual(null, actual.Value);

		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SafeGetOnNullDictionaryThrows()
		{
			Dictionary<string, string> dictionary = null;
			dictionary.SafeGet("key");
		}

		[TestMethod]
		public void GetWithMatchingValueReturns()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "key", "value" }
			};

			var expected = "value";
			var actual = dictionary.Get("key", "key not found");

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetWithNoMatchingValueReturnsDefault()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "key", "value" }
			};

			var expected = "key not found";
			var actual = dictionary.Get("nonexistant", "key not found");

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void GetWithNoMatchingValueDefaults()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{ "key", new object() }
			};

			var expected = (object)null;
			var actual = dictionary.Get("nonexistant");

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetOnNullDictionaryThrows()
		{
			Dictionary<string, string> dictionary = null;
			dictionary.Get("key", "key not found");
		}
	}
}
