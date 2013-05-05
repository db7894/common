using log4net;

namespace System.Diagnostics
{
	/// <summary>
	/// <para>
	/// Implementatin of the <see cref="TraceListener"/> using log4net
	/// as the backing logger. This was created so we could take advantage of 
	/// the internal WCF message and trace logging but use log4net so we have
	/// the power of its advanced appenders (the trace loggers could only input
	/// to a single file or console without rotation).
	/// </para><para>
	/// The reason that we have the namespace as System.Diagnostics is so we
	/// can filter all the log4net messages to namespace to a specific
	/// log file since we don't want to pollute our main log file with _tons_
	/// of extra debug information.
	/// </para>
	/// </summary>
	public class Log4NetTextTraceListener : TextWriterTraceListener
	{
		/// <summary>
		/// Handle to the logger for this type
		/// </summary>
		private static readonly ILog _log =
			LogManager.GetLogger(typeof(Log4NetTextTraceListener));

		/// <summary>
		/// Implementation of the TraceListener write method
		/// </summary>
		/// <param name="message">The message to write</param>
		public override void Write(string message)
		{
			if (_log.IsDebugEnabled)
			{
				_log.Debug(message);
			}
		}

		/// <summary>
		/// Implementation of the TraceListener write listener method
		/// </summary>
		/// <param name="message">The message to write</param>
		public override void WriteLine(string message)
		{
			if (_log.IsDebugEnabled)
			{
				_log.Debug(message);
			}
		}
	}
}
