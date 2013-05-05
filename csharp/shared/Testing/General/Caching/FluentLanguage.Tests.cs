using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Caching.Cleaning;
using System;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// FluentLanguage DSL.
	/// </summary>
	[TestClass]
	public class FluentLanguageTests
	{
		[TestMethod]
		public void Initialize_Janitor_Fluently()
		{
			var strategy = Expires.Never<object>();
			var cache = CheckAndSetCache.Create<string, object>(strategy)
				.CleanWith(trash => Cleanup.Expired(trash));

			Assert.IsNotNull(cache);
		}

		[TestMethod]
		public void Initialize_JanitorWithOptions_Fluently()
		{
			var options = new CleanupOptions
			{
				Frequency = TimeSpan.FromDays(1),
			};
			var strategy = Expires.Never<object>();
			var cache = CheckAndSetCache.Create<string, object>(strategy)
				.CleanWith(trash => Cleanup.Expired(trash), options);

			Assert.IsNotNull(cache);
		}
	}
}
