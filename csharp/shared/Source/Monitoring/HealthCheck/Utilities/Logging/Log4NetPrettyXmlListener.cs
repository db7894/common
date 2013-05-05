using System.IO;
using System.Xml;
using log4net;

namespace System.Diagnostics
{
	/// <summary>
	/// Implemenation of the trace listener for logging WCF requests and 
	/// responses.  This listener pretty prints the input WCF data to
	/// the log4net log file.   
	/// <note>This will make for LARGE log files.</note>
	/// <note>This logging should only used for debugging in DEV.</note>
	/// To use this logger with your WCF service, add a rolling file appender (in the log4net
	/// section of the configuration) to store the trace data and then associate a log manager 
	/// name that points to the new appender.
	/// <code>
	/// <![CDATA[
	/// <log4net>
	///		...
	/// 	<!-- Log file for the transport log (all WCF messages). -->
	///		<appender name="TransportLogMessages" type="log4net.Appender.RollingFileAppender">
	///			<file value="C:\Inetpub\Logs\AlertManagementService\TransportLog"/>
	///			<appendToFile value="true"/>
	///			<datePattern value=".yyyy-MM-dd.\l\o\g"/>
	///			<rollingStyle value="Date"/>
	///			<maximumFileSize value="10MB" />
	///			<MaxSizeRollBackups value="14"/>
	///			<param name="StaticLogFileName" value="false"/>
	///			<layout type="log4net.Layout.PatternLayout">
	///				<conversionPattern value="%d{HH:mm:ss.fff} [%thread] %-5level %logger{1} - %m%n"/>
	///			</layout>
	///		</appender>
	///		...
	///		<!-- Logger just for diagnostic messages from WCF -->
	///		<logger name="System.Diagnostics.Log4NetPrettyXmlListener" additivity="false">
	///			<level value="Debug" />
	///			<appender-ref ref="TransportLogMessages"/>
	///		</logger>
	///		...
	/// </log4net>
	/// ]]>
	/// </code>
	/// This will setup a log file that the Log4NetPrettyXmlListener can write to to store
	/// WCF messages.  The next step is to setup the WCF service to get rid of the default 
	/// listener and use this logging listener instead.
	/// <code>
	/// <![CDATA[
	///	...
	///	<system.diagnostics>
	///		<sources>
	///			<source name="System.ServiceModel.MessageLogging" switchValue="Verbose">
	///				<listeners>
	///					<remove name="Default" />
	///					<add name="ServiceModelMessageLoggingListener" />
	///				</listeners>
	///			</source>
	///		</sources>
	///		<sharedListeners>
	///			<add name="ServiceModelMessageLoggingListener"
	///				 type="System.Diagnostics.Log4NetPrettyXmlListener, MTNT.Shared.Utilities" />
	///		</sharedListeners>
	///		<trace autoflush="true"/>
	///	</system.diagnostics>
	/// ...
	/// ]]>
	/// </code>
	/// The last step is to enable WCF diagnostics and specify the level of messages to print
	/// out.  
	/// <code>
	/// <![CDATA[
	/// ...
	/// <system.serviceModel>
	///		<diagnostics performanceCounters="All">
	///			<messageLogging logEntireMessage="true" logMalformedMessages="true"
	///			  logMessagesAtServiceLevel="true" logMessagesAtTransportLevel="true"
	///			  maxMessagesToLog="100000" />
	///		</diagnostics>	
	///  ]]>
	/// </code>
	/// </summary>
	public class Log4NetPrettyXmlListener : TraceListener
	{
		#region Private Members

		/// <summary>
		/// Handle to the logger for this type
		/// </summary>
		private static ILog Log { get; set; }

		/// <summary>
		/// Settings for the output format of the trace log.
		/// </summary>
		private static XmlWriterSettings WriterSettings { get; set; }
		
		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static Log4NetPrettyXmlListener()
		{
			Log = LogManager.GetLogger(typeof(Log4NetPrettyXmlListener));

			// The settings for the XmlWriter.
			WriterSettings = new XmlWriterSettings
			{
				Indent = true,
				OmitXmlDeclaration = true,
			};
		}

		#endregion

		#region TraceListener overrides

		/// <summary>
		/// Implementation of the TraceListener write method
		/// </summary>
		/// <param name="message">The message to write</param>
		public override void Write(string message)
		{
			if (Log.IsDebugEnabled)
			{
				Log.Debug(message);
			}
		}

		/// <summary>
		/// Implementation of the TraceListener write listener method
		/// </summary>
		/// <param name="message">The message to write</param>
		public override void WriteLine(string message)
		{
			if (Log.IsDebugEnabled)
			{
				// Load the XmlDocument with the XML string.
				var xmlDom = new XmlDocument();
				try
				{
					xmlDom.LoadXml(message);
				}
				// invalid XML, just write whatever we have and exit.
				catch (Exception)
				{
					Log.Debug(message);
					return;
				}

				// The memory stream for the backing.
				using(var ms = new MemoryStream())
				{
					// Create the XmlWriter.
					using (var xmlWriter = XmlWriter.Create(ms, WriterSettings))
					{
						// perform a write, this will do the pretty-print.
						xmlDom.WriteContentTo(xmlWriter);
					}
					// Have to rewind the MemoryStream in order to read
					// its contents.
					ms.Flush();
					ms.Position = 0;

					// Read MemoryStream contents into a StreamReader.
					using(var streamReader = new StreamReader(ms))
					{
						// Extract the text from the StreamReader.
						Log.Debug(streamReader.ReadToEnd());
					}
				}
			}
		}
		#endregion
	}
}