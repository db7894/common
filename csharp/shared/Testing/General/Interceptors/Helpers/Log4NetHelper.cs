using System;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace SharedAssemblies.General.Interceptors.UnitTests.Helpers
{
	/// <summary>
	/// A collection of helper methods to make testing log4net messages easier
	/// </summary>
	/// <remarks>
	/// See any of the interceptor unit tests for a portable copy and paste useage
	/// </remarks>
	public static class Log4NetHelper
	{
		/// <summary>
		/// Helper to create a root memory appender
		/// </summary>
		/// <remarks>
		/// http://dhvik.blogspot.com/2008/08/adding-appender-to-log4net-in-runtime.html
		/// </remarks>
		/// <param name="level">The level the logs should run at</param>
		/// <returns>The created memory appender</returns>
		public static MemoryAppender CreateMemoryAppender(Level level)
		{
			MemoryAppender appender = new MemoryAppender { Name = "MemoryAppender", };
			appender.ActivateOptions();

			Hierarchy repository = LogManager.GetRepository() as Hierarchy;
			if (repository != null)
			{
				repository.Root.AddAppender(appender);
				repository.Root.Level = level;
				repository.Configured = true;
				repository.RaiseConfigurationChanged(EventArgs.Empty);
			}
			return appender;
		}
	}
}
