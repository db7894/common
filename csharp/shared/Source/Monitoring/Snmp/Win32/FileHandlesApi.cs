using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SharedAssemblies.Monitoring.Snmp.Win32
{
    /// <summary>
    /// <para>
    /// Static class representing Win32 API for accessing OS "file" handles
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
    internal static class FileHandlesApi
    {
        /// <summary>
        /// Constant to represent an invalid handle value
        /// </summary>
        public static readonly IntPtr InvalidHandle = new IntPtr(-1);

        /// <summary>
        /// Constant to represent an invalid handle value
        /// </summary>
        public static readonly IntPtr NullHandle = IntPtr.Zero;

        /// <summary>
        /// Create an OS File Handle
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="fileAccess">Access level of file</param>
        /// <param name="fileShare">File share permissions</param>
        /// <param name="securityAttributes">File security attributes</param>
        /// <param name="creationDisposition">How to create the file</param>
        /// <param name="flags">File create flags</param>
        /// <param name="template">File create template</param>
        /// <returns>A safe handle to the file created</returns>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] Types.FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] Types.FileShare fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] Types.FileDisposition creationDisposition,
            [MarshalAs(UnmanagedType.U4)] Types.FileAttributes flags,
            IntPtr template);


        /// <summary>
        /// Write to an OS File Handle
        /// </summary>
        /// <param name="fileHandle">handle to the file</param>
        /// <param name="buffer">buffer to use for writing the file</param>
        /// <param name="numberOfBytesToWrite">Number of bytes to write to file</param>
        /// <param name="numberOfBytesWritten">Number of bytes actually written</param>
        /// <param name="overlappedPointer">Pointer to overlapped</param>
        /// <returns>True if successful</returns>
        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(SafeFileHandle fileHandle, byte[] buffer,
           uint numberOfBytesToWrite,
           out uint numberOfBytesWritten,
           IntPtr overlappedPointer);


        /// <summary>
        /// Close an OS File Handle
        /// </summary>
        /// <param name="handle">Handle to the object to close</param>
        /// <returns>True if handle closed successfully</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);
    }
}
