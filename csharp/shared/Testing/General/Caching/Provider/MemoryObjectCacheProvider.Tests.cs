using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Provider.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// MemoryObjectCacheProvider.
	/// </summary>
	[TestClass]
	public class MemoryObjectCacheProviderTests
	{
		[TestMethod]
		public void Provider_Initializes_Correctly()
		{
			using (var provider = new MemoryObjectCacheProvider<string>("initial"))
			{
				Assert.IsNotNull(provider);
			}
		}

		[TestMethod]
		public void Factory_Initializes_Correctly()
		{
			using (var provider = MemoryObjectCacheProvider.Create("initial"))
			{
				Assert.IsNotNull(provider);
			}
		}

		[TestMethod]
		public void GetAndSet_Functions_Correctly()
		{
			using (var provider = MemoryObjectCacheProvider.Create("initial"))
			{
				Assert.AreEqual("initial", provider.Value);
				provider.Value = "secondary";
				Assert.AreEqual("secondary", provider.Value);
			}
		}
	}
}
