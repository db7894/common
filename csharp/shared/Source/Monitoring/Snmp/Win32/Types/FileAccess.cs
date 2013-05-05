using System;

namespace SharedAssemblies.Monitoring.Snmp.Win32.Types
{
    /// <summary>
    /// File access bit flags
    /// </summary>
    [Flags]
    internal enum FileAccess : uint
    {
        /// <summary>
        /// Read access
        /// </summary>
        GenericRead = 0x80000000,

        /// <summary>
        /// Write access
        /// </summary>
        GenericWrite = 0x40000000,

        /// <summary>
        /// Execute access
        /// </summary>
        GenericExecute = 0x20000000,

        /// <summary>
        /// All access
        /// </summary>
        GenericAll = 0x10000000
    }
}