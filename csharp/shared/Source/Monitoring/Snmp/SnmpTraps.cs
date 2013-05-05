using System;
using System.Text;
using Microsoft.Win32.SafeHandles;
using SharedAssemblies.Monitoring.Snmp.Win32;
using SharedAssemblies.Monitoring.Snmp.Win32.Types;

namespace SharedAssemblies.Monitoring.Snmp
{
    /// <summary>
    /// This class is a utility class to enable sending of SNMP traps
    /// </summary>
	/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
	[Obsolete("Use $/SharedAssemblies/ThirdParty/SharpSnmp instead", true)]
	public static class SnmpTraps
    {
        /// <summary>The default trap agent server name</summary>
        private const string _defaultTrapAgentServer = "\\\\.\\mailslot\\TRAPAGT6";

        /// <summary>The default trap target server name</summary>
        private const string _defaultTrapTargetServer = "\\\\.\\mailslot\\TRAPTRG6";

        /// <summary>The default trap error text</summary>
        private const string _defaultTrapErrorText = "NAQ_FILTER";

		/// <summary>
		/// Get/set the trap agent server
		/// </summary>
		public static string TrapAgentServer { get; set; }

		/// <summary>
		/// Get/set the trap target server
		/// </summary>
		public static string TrapTargetServer { get; set; }

		/// <summary>
		/// Get/set the trap error text
		/// </summary>
		public static string TrapErrorText { get; set; }

		/// <summary>
        /// Static constructor to init the autoproperties
        /// </summary>
        static SnmpTraps()
        {
            TrapAgentServer = _defaultTrapAgentServer;
            TrapTargetServer = _defaultTrapTargetServer;
            TrapErrorText = _defaultTrapErrorText;
        }

        /// <summary>
        /// Send an SNMP trap given a trap number
        /// </summary>
        /// <param name="trapNumber">SNMP Trap Number</param>
        /// <returns>True if sent successfully</returns>
        public static bool SendTrap(int trapNumber)
        {
            // send trap with default error text
            return SendTrap(trapNumber, TrapErrorText);
        }

        /// <summary>
        /// Send an SNMP trap given a trap number and error text
        /// </summary>
        /// <param name="trapNumber">SNMP Trap Number</param>
        /// <param name="errorText">Description of error condition</param>
        /// <returns>True if sent successfully</returns>
        public static bool SendTrap(int trapNumber, string errorText)
        {
            var msa = SecurityAttributesFactory.Create(true);

            // create mailslot for acks coming back
            using (MailslotsApi.CreateMailslot(TrapTargetServer, 0,
                                               MailslotsApi.MailslotWaitForever, ref msa))
            {
                // create mailslot for destination
                using (SafeFileHandle trapAgentMailslot = MailslotsApi.OpenMailslot(TrapAgentServer))
                {
                    if (!trapAgentMailslot.IsInvalid)
                    {
                        // write message to mailbox
                        uint bytesWritten;
                        byte[] textBytes = Encoding.ASCII.GetBytes(
                            string.Format("{0}|68|{1}", trapNumber, errorText));

                        return FileHandlesApi.WriteFile(trapAgentMailslot, textBytes,
							(uint) textBytes.Length, out bytesWritten, IntPtr.Zero);
                    }

                    // if failed to get handle, return false
                    return false;
                }
            }
        }
    }
}