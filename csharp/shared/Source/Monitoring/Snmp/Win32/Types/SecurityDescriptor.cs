using System;
using System.Runtime.InteropServices;

namespace SharedAssemblies.Monitoring.Snmp.Win32.Types
{
    /// <summary>
    /// Structure that represents security descriptor requests in Win32 API
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct SecurityDescriptor
    {
		/// <summary>
		/// Revision of the descriptor
		/// </summary>
        public byte Revision;

		/// <summary>
		/// Size of the descriptor
		/// </summary>
        public byte Size;

		/// <summary>
		/// Control flags
		/// </summary>
        public short Control;

		/// <summary>
		/// The owner of the item
		/// </summary>
        public IntPtr Owner;

		/// <summary>
		/// The group of the item
		/// </summary>
        public IntPtr Group;

		/// <summary>
		/// The System Access Control List (SACL)
		/// </summary>
        public IntPtr SystemAccessControlList;

		/// <summary>
		/// The Discretionary Access Control List (DACL)
		/// </summary>
        public IntPtr DiscretionaryAccessControlList;
    }
}
