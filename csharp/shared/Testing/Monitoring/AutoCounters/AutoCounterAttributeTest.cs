using SharedAssemblies.Monitoring.AutoCounters.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Monitoring.AutoCounters;


namespace SharedAssemblies.Monitoring.AutoCounters.UnitTests
{
    /// <summary>
    /// This is a test class for AutoCounterAttributeTest and is intended
    /// to contain all AutoCounterAttributeTest Unit Tests
    /// </summary>
	[TestClass]
	public class AutoCounterAttributeTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		/// <summary>
		/// A test for AutoCounterAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterAttributeConstructorTest()
		{
			var target = new AutoCounterAttribute();

			Assert.AreEqual(target.AutoCounterType, AutoCounterType.Unknown);
			Assert.AreEqual(":", target.AbbreviatedName);
			Assert.AreEqual("", target.Category);
			Assert.AreEqual("", target.Description);
			Assert.IsFalse(target.IsReadOnly);
			Assert.AreEqual("", target.Name);
			Assert.AreEqual(":", target.UniqueName);
			Assert.AreEqual("?", target.Units);
		}

		/// <summary>
		/// A test for AutoCounterAttribute Constructor
		/// </summary>
		[TestMethod]
		public void AutoCounterAttributeConstructorWithArgsTest()
		{
			var target = new AutoCounterAttribute(AutoCounterType.ElapsedTime, "My Category", "My Counter");

			Assert.AreEqual(AutoCounterType.ElapsedTime, target.AutoCounterType);
			Assert.AreEqual("My Category:My Counter", target.AbbreviatedName);
			Assert.AreEqual("My Category", target.Category);
			Assert.AreEqual("My Counter", target.Description);
			Assert.IsFalse(target.IsReadOnly);
			Assert.AreEqual("My Counter", target.Name);
			Assert.AreEqual("My Category:My Counter", target.UniqueName);
			Assert.AreEqual("sec", target.Units);
		}

		/// <summary>
		/// A test for AbbreviatedName
		/// </summary>
		[TestMethod]
		public void AbbreviatedNameTest()
		{
			var target = new AutoCounterAttribute() { AbbreviatedName = "Short" };

			Assert.AreEqual("Short", target.AbbreviatedName);
		}

		/// <summary>
		/// A test for AutoCounterType
		/// </summary>
		[TestMethod]
		public void AutoCounterTypeTest()
		{
			var target = new AutoCounterAttribute() { AutoCounterType = AutoCounterType.TotalCount };

			Assert.AreEqual(AutoCounterType.TotalCount, target.AutoCounterType);
		}

		/// <summary>
		/// A test for Category
		/// </summary>
		[TestMethod]
		public void CategoryTest()
		{
			var target = new AutoCounterAttribute() { Category = "Short" };

			Assert.AreEqual("Short", target.Category);
		}

		/// <summary>
		/// A test for Description
		/// </summary>
		[TestMethod]
		public void DescriptionTest()
		{
			var target = new AutoCounterAttribute() { Description = "Short" };

			Assert.AreEqual("Short", target.Description);
		}

		/// <summary>
		/// A test for IsReadOnly
		/// </summary>
		[TestMethod]
		public void IsReadOnlyTest()
		{
			var target = new AutoCounterAttribute() { IsReadOnly = true };

			Assert.IsTrue(target.IsReadOnly);
		}

		/// <summary>
		/// A test for Name
		/// </summary>
		[TestMethod]
		public void NameTest()
		{
			var target = new AutoCounterAttribute() { Name = "Short" };

			Assert.AreEqual("Short", target.Name);
		}

		/// <summary>
		/// A test for UniqueName
		/// </summary>
		[TestMethod]
		public void UniqueNameTest()
		{
			var target = new AutoCounterAttribute() { UniqueName = "Short" };

			Assert.AreEqual("Short", target.UniqueName);
		}

		/// <summary>
		/// A test for Units
		/// </summary>
		[TestMethod]
		public void UnitsTest()
		{
			var target = new AutoCounterAttribute() { Units = "Short" };

			Assert.AreEqual("Short", target.Units);
		}
	}
}
