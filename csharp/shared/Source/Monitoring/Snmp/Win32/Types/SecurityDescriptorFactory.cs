using System;

namespace SharedAssemblies.Monitoring.Snmp.Win32.Types
{
    /// <summary>
    /// A class that creates instances of the Win32 SecurityDescriptor class
    /// </summary>
    internal static class SecurityDescriptorFactory
    {
        /// <summary>
        /// Create a null descriptor, not to be confused with a null ptr to a descriptor.
        /// The former gives unlimited access, the latter gives no access.
        /// </summary>
        /// <returns>A security descriptor</returns>
        public static SecurityDescriptor Create()
        {
            var sd = new SecurityDescriptor();

            // init all values to correct defaults
            SecurityDescriptorsApi.InitializeSecurityDescriptor(ref sd, 
                SecurityDescriptorsApi.SecurityDescriptorRevision);

            // set the DACL to a null DACL
            SecurityDescriptorsApi.SetSecurityDescriptorDacl(ref sd, true, IntPtr.Zero, false);

            return sd;
        }
    }
}
