using System.Runtime.InteropServices;


namespace SharedAssemblies.Monitoring.Snmp.Win32.Types
{
    /// <summary>
    /// Attributes used for modifying Security Descriptors in Win32 API
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityAttributes
    {
        /// <summary>
        /// The length of the security attributes structure
        /// </summary>
        public int Length;

        /// <summary>
        /// The security descriptor for the security attributes
        /// </summary>
        public SecurityDescriptor SecurityDescriptor;

        /// <summary>
        /// True if inherits the handle of its parent
        /// </summary>
        public bool DoesInheritHandle;
    }
}
