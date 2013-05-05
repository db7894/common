using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Caching.Tests
{
	/// <summary>
	/// A collection of tests that exercise the functionality of the
	/// CachedValue.
	/// </summary>
	[TestClass]
	public class CachedValueTests
	{
		[TestMethod]
		public void CachedValue_ExpiredHandle_IsAlwaysExpired()
		{
			Assert.IsTrue(CachedValue<string>.Expired.IsExpired);
		}

		[TestMethod]
		public void CachedValue_TimeStamp_IsInitialized()
		{
			var value = new CachedValue<string>("value", Expires.Never<string>(), true);
			Assert.IsTrue(DateTime.Now.Ticks >= value.Created);
		}

		[TestMethod]
		public void CachedValue_AlreadyExpired_Test()
		{
			var value = new CachedValue<string>("value", Expires.Never<string>(), true);
			Assert.IsTrue(value.IsExpired);
		}

		[TestMethod]
		public void CachedValue_ExpiredStrategy_Works()
		{
			var value = new CachedValue<string>("value", Expires.Always<string>());
			Assert.IsTrue(value.IsExpired);

			value = new CachedValue<string>("value", Expires.Never<string>());
			Assert.IsFalse(value.IsExpired);
		}

		[TestMethod]
		public void CachedValue_HitsIncrement_Test()
		{
			var value = new CachedValue<string>("value", Expires.Never<string>());
			Assert.AreEqual("value", value.Value);
			Assert.AreEqual("value", value.Value);
			Assert.AreEqual("value", value.Value);
			Assert.AreEqual(3, value.Hits);
		}

		[TestMethod]
		public void CachedValue_LastTouchedIncremented_Test()
		{
			var value = new CachedValue<string>("value", Expires.Never<string>());
			var current = value.LastTouched;
			Thread.Sleep(TimeSpan.FromMilliseconds(100));

			Assert.AreEqual("value", value.Value);
			Assert.AreNotEqual(current, value.LastTouched);
		}
	}
}
