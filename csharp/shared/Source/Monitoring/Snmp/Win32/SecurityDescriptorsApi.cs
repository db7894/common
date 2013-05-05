using System;
using System.Runtime.InteropServices;

namespace SharedAssemblies.Monitoring.Snmp.Win32
{
    /// <summary>
    /// <para>
    /// Static class that accesses Win32 APIs related to manipulating/creating security descriptors
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
    internal static class SecurityDescriptorsApi
    {
        /// <summary>
        /// The minimum Length of a security descriptor
        /// </summary>
        public static readonly uint SecurityDescriptorMinLength = 20;

        /// <summary>
        /// The Revision number of a security descriptor
        /// </summary>
        public static readonly uint SecurityDescriptorRevision = 1;

        /// <summary>
        /// Set the DACL (Declarative Access Control List) in a SecurityDescriptor
        /// </summary>
        /// <param name="securityDescriptor">Security descriptor to set DACL in</param>
        /// <param name="daclPresent">True if DACL is present</param>
        /// <param name="dacl">The dacl to set</param>
        /// <param name="daclDefaulted">True if DACL is defaulted</param>
        /// <returns>True if DACL set successfully</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(
			ref Types.SecurityDescriptor securityDescriptor, bool daclPresent,
			IntPtr dacl, bool daclDefaulted);

        /// <summary>
        /// Initialize a SecurityDescriptor
        /// </summary>
        /// <param name="securityDescriptor">The security descriptor to initialize</param>
        /// <param name="revision">The revision of the descriptor</param>
        /// <returns>True if initialized</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(
			ref Types.SecurityDescriptor securityDescriptor, uint revision);
    }
}

