using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SharedAssemblies.General.Caching.Provider.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// MemoryCacheProvider.
	/// </summary>
	[TestClass]
	public class MemoryCacheProviderTests
	{
		[TestMethod]
		public void Provider_Initializes_Correctly()
		{
			using (var provider = new MemoryCacheProvider<string, string>())
			{
				Assert.IsNotNull(provider);
			}
		}

		[TestMethod]
		public void Adds_Function_Correctly()
		{
			var collection = new List<KeyValuePair<string, CachedValue<string>>>
			{
				new KeyValuePair<string, CachedValue<string>>("key2", new CachedValue<string>("value2")),
				new KeyValuePair<string, CachedValue<string>>("key3", new CachedValue<string>("value3")),
			};

			using (var provider = new MemoryCacheProvider<string, string>())
			{
				provider.Add("key1", new CachedValue<string>("value2"));
				provider.Add("key1", new CachedValue<string>("value1"));
				provider.Add(collection);
				Assert.AreEqual(3, provider.Count());
				Assert.AreEqual("value1", provider.Get("key1").Value);
				Assert.IsTrue(provider.Get("non-existant").IsExpired);
				foreach (var value in provider.Get(new List<string> { "key2", "key3" }))
				{
					Assert.IsTrue(collection.Any(item => item.Value.Value == value.Value));
				}
			}
		}

		[TestMethod]
		public void Enumerable_Function_Correctly()
		{
			var provider = new MemoryCacheProvider<string, string>()
			{
				{ "key1", new CachedValue<string>("value1") },
				{ "key2", new CachedValue<string>("value2") },
				{ "key3", new CachedValue<string>("value3") },
			};

			foreach (var item in provider)
			{
				Assert.AreEqual(item.Value, provider.Get(item.Key));
			}

			// for code coverage sake
			var enumerator = ((System.Collections.IEnumerable)provider).GetEnumerator();
		}

		[TestMethod]
		public void Removes_Function_Correctly()
		{
			var provider = new MemoryCacheProvider<string, string>()
			{
				{ "key1", new CachedValue<string>("value1") },
				{ "key2", new CachedValue<string>("value2") },
				{ "key3", new CachedValue<string>("value3") },
			};

			Assert.AreEqual(3, provider.Count());
			provider.Remove("key1");
			Assert.AreEqual(2, provider.Count());
			provider.Remove(new List<string> { "key2", "key3" });
			Assert.AreEqual(0, provider.Count());
		}
	}
}
