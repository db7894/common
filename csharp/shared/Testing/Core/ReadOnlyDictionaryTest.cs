using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// This is a test class for ReadOnlyDictionaryTest and is intended
	/// to contain all ReadOnlyDictionaryTest Unit Tests
	/// </summary>
	[TestClass]
	public class ReadOnlyDictionaryTest
	{
		private ReadOnlyDictionary<string, string> _target;
		private Dictionary<string, string> _wrapped;

		/// <summary>
		/// Set up the target test dictionary
		/// </summary>
		[TestInitialize]
		public void SetUpDictionary()
		{
			_wrapped = new Dictionary<string, string>
						{
							{ "A", "Apple" },
							{ "B", "Boat" },
							{ "C", "Cat" },
							{ "D", "Dog" }
						};

			_target = new ReadOnlyDictionary<string, string>(_wrapped);
		}

		/// <summary>
		/// Test the constructor
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorThrowsIfWrappedInstanceNull()
		{
			new ReadOnlyDictionary<string, string>(null);
		}

		/// <summary>
		/// Test write operations throw
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AddThrows()
		{
			_target.Add("E", "Elephant");
		}

		/// <summary>
		/// Test write operations throw
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ClearThrows()
		{
			_target.Clear();
		}

		/// <summary>
		/// Test write operations throw
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void RemoveThrows()
		{
			_target.Remove("A");
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void IndexerSetThrows()
		{
			_target["A"] = "Hi, mom!";
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void ContainsPassesThroughToWrappedDictionary()
		{
			Assert.IsTrue(_target.Contains(new KeyValuePair<string, string>("A", "Apple")));
			Assert.IsTrue(_target.Contains(new KeyValuePair<string, string>("B", "Boat")));
			Assert.IsTrue(_target.Contains(new KeyValuePair<string, string>("C", "Cat")));
			Assert.IsTrue(_target.Contains(new KeyValuePair<string, string>("D", "Dog")));
			Assert.IsFalse(_target.Contains(new KeyValuePair<string, string>("Z", "Zebra")));
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void ContainsKeyPassesThroughToWrappedDictionary()
		{
			Assert.IsTrue(_target.ContainsKey("A"));
			Assert.IsTrue(_target.ContainsKey("B"));
			Assert.IsTrue(_target.ContainsKey("C"));
			Assert.IsTrue(_target.ContainsKey("D"));
			Assert.IsFalse(_target.ContainsKey("Z"));
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void CopyToPassesThroughToWrappedDictionary()
		{
			var array = new KeyValuePair<string, string>[4];

			_target.CopyTo(array, 0);

			foreach (var element in array)
			{
				Assert.IsTrue(_target.Contains(element));
			}
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void IsReadOnlyIsTrue()
		{
			Assert.IsTrue(_target.IsReadOnly);
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void KeysPassesThroughToWrappedDictionary()
		{
			Assert.IsTrue(_wrapped.Keys.SequenceEqual(_target.Keys));
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void ValuesPassesThroughToWrappedDictionary()
		{
			Assert.IsTrue(_wrapped.Values.SequenceEqual(_target.Values));
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void TryGetValuePassesThroughToWrappedDictionary()
		{
			string result = null;

			_target.TryGetValue("A", out result);

			Assert.AreEqual(_wrapped["A"], result);
		}

		/// <summary>
		/// Test read operations succeed
		/// </summary>
		[TestMethod]
		public void IndexerGetPassesThroughToWrappedDictionary()
		{
			Assert.AreEqual(_wrapped["A"], _target["A"]);
		}
	}
}
