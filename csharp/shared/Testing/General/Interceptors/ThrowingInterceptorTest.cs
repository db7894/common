using System.Reflection;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedAssemblies.General.Testing;
using SharedAssemblies.General.Interceptors.UnitTests.Helpers;

namespace SharedAssemblies.General.Interceptors.UnitTests
{
    /// <summary>
    /// This is a test class for ThrowingInterceptorTest and is intended
    /// to contain all ThrowingInterceptorTest Unit Tests
    /// </summary>
	[TestClass]
	public class ThrowingInterceptorTest
	{
		#region Test Setup

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		private TestContext TestContextInstance { get; set; }

		/// <summary>
		/// Handle to our log4net appender
		/// </summary>
		private static log4net.Appender.MemoryAppender _appender;

		/// <summary>
		/// Test set up method
		/// </summary>
		[TestInitialize]
		public void TestSetUp()
		{
			_appender = Log4NetHelper.CreateMemoryAppender(log4net.Core.Level.All);
		}

		/// <summary>
		/// Test tear down method
		/// </summary>
		[TestCleanup]
		public void TestTearDown()
		{
			_appender.Clear();
		}

		#endregion

		/// <summary>
		/// A test for a blocking interception
		/// </summary>
		[TestMethod]
		public void TestBlockAll_InstanceConstructor()
		{
			var mock = new Mock<IWrapped>();

			IWrapped handle = ThrowingInterceptor.Create(mock.Object);
			AssertEx.Throws(handle.NoReturnMethod);
			AssertEx.Throws(() => handle.ReturnMethod("INPUT"));

			mock.Verify(imp => imp.NoReturnMethod(), Times.Never());
			mock.Verify(imp => imp.ReturnMethod(It.IsAny<string>()), Times.Never());
		}

		/// <summary>
		/// A test for a blocking interception
		/// </summary>
		[TestMethod]
		public void TestBlockAll_DefaultConstructor()
		{
			IWrapped handle = ThrowingInterceptor.Create<IWrapped, Wrapped>();
			AssertEx.Throws(handle.NoReturnMethod);
			AssertEx.Throws(() => handle.ReturnMethod("INPUT"));
		}

		/// <summary>
		///A test for a nonblocking interception
		///</summary>
		[TestMethod]
		public void TestBlockNone_InstanceConstructor()
		{
			var mock = new Mock<IWrapped>();
			mock.Setup(imp => imp.ReturnMethod(It.IsAny<string>()))
				.Returns((string s) => s.ToLower());

			IWrapped handle = ThrowingInterceptor.Create(mock.Object,
				new List<MethodInfo>());
			handle.NoReturnMethod();
			string result = handle.ReturnMethod("INPUT");
			Assert.AreEqual("input", result);

			mock.Verify(imp => imp.NoReturnMethod(), Times.Once());
			mock.Verify(imp => imp.ReturnMethod(It.IsAny<string>()), Times.Once());
		}

		/// <summary>
		///A test for a blocking interception
		///</summary>
		[TestMethod]
		public void TestBlockNone_DefaultConstructor()
		{
			IWrapped handle = ThrowingInterceptor.Create<IWrapped, Wrapped>(
				new List<MethodInfo>());
			AssertEx.DoesNotThrow(() => handle.NoReturnMethod("INPUT"));
			string result = handle.ReturnMethod("INPUT");
			Assert.AreEqual("input", result);
		}
	}
}
