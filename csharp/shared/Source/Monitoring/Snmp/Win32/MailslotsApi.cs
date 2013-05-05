using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SharedAssemblies.Monitoring.Snmp.Win32
{
    /// <summary>
    /// <para>
    /// Static class for using Win32 API to handle mailslot requests
    /// </para>
    /// <para>
    /// WARNING:
    /// </para>
    /// <para>
    /// This is a very RAW API that is meant to directly access the Win32 DLLs for tasks that 
    /// .NET has no way to currently perform.  
    /// </para>
    /// <para>
    /// If at all possible use a .NET equivalent if one exists before using these APIs.
    /// </para>
    /// <para>
    /// When using these APIs in your own classes, always wrap their functionality into
    /// private methods and hide the details.
    /// </para>
    /// </summary>
    internal static class MailslotsApi
    {
        /// <summary>
        /// Constant to tell create to wait forever on reads
        /// </summary>
        public static readonly int MailslotWaitForever = -1;

        /// <summary>
        /// Create an OS mailslot
        /// </summary>
        /// <param name="name">Name of the mailslot</param>
        /// <param name="maxMessageSize">The max Size of the message</param>
        /// <param name="readTimeout">The timeout on reads</param>
        /// <param name="securityAttributes">The security attributes for the mailslot</param>
        /// <returns>A safe file handle to the mailslot</returns>
        [DllImport("kernel32.dll")]
        public static extern SafeFileHandle CreateMailslot(string name, uint maxMessageSize,
           int readTimeout, ref Types.SecurityAttributes securityAttributes);

        /// <summary>
        /// Open a mailslot given a name
        /// </summary>
		/// <param name="mailslotName">The name of the mail-slot to open</param>
		/// <returns>A handle to the requested mail-slot</returns>
        public static SafeFileHandle OpenMailslot(string mailslotName)
        {
            // create the OS file handle for the mailslot
            return FileHandlesApi.CreateFile(mailslotName,
				Types.FileAccess.GenericRead | Types.FileAccess.GenericWrite,
				Types.FileShare.Read | Types.FileShare.Write,
				IntPtr.Zero,
				Types.FileDisposition.OpenExisting,
				Types.FileAttributes.None,
				IntPtr.Zero);
        }
    }
}
