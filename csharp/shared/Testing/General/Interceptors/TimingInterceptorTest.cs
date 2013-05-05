using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedAssemblies.General.Interceptors.UnitTests.Helpers;

namespace SharedAssemblies.General.Interceptors.UnitTests
{
    /// <summary>
    /// This is a test class for TimingInterceptorTest and is intended
    /// to contain all TimingInterceptorTest Unit Tests
    /// </summary>
	[TestClass]
	public class TimingInterceptorTest
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
		///A test for a blocking interception
		///</summary>
		[TestMethod]
		public void TestTiming_InstanceConstructor()
		{
			var mock = new Mock<IWrapped>();
			mock.Setup(imp => imp.ReturnMethod(It.IsAny<string>()))
				.Returns((string s) => s.ToLower());
			
			IWrapped handle = TimingInterceptor.Create(mock.Object);
			handle.NoReturnMethod();
			string result = handle.ReturnMethod("INPUT");
			Assert.AreEqual("input", result);

			log4net.Core.LoggingEvent[] events = _appender.GetEvents();
			Assert.IsTrue(events[0].RenderedMessage.Contains("Entering NoReturnMethod"));
			Assert.IsTrue(events[1].RenderedMessage.Contains("Leaving NoReturnMethod"));
			Assert.IsTrue(events[2].RenderedMessage.Contains("Entering ReturnMethod"));
			Assert.IsTrue(events[3].RenderedMessage.Contains("Leaving ReturnMethod"));

			mock.Verify(imp => imp.NoReturnMethod(), Times.Once());
			mock.Verify(imp => imp.ReturnMethod(It.IsAny<string>()), Times.Once());
		}

		/// <summary>
		/// A test for a blocking interception
		/// </summary>
		[TestMethod]
		public void TestTiming_DefaultConstructor()
		{
			IWrapped handle = TimingInterceptor.Create<IWrapped, Wrapped>();
			handle.NoReturnMethod("INPUT");
			string result = handle.ReturnMethod("INPUT");
			Assert.AreEqual("input", result);

			log4net.Core.LoggingEvent[] events = _appender.GetEvents();
			Assert.IsTrue(events[0].RenderedMessage.Contains("Entering NoReturnMethod"));
			Assert.IsTrue(events[1].RenderedMessage.Contains("Leaving NoReturnMethod"));
			Assert.IsTrue(events[2].RenderedMessage.Contains("Entering ReturnMethod"));
			Assert.IsTrue(events[3].RenderedMessage.Contains("Leaving ReturnMethod"));
		}
	}
}
