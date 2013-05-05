using System.Runtime.InteropServices;

namespace SharedAssemblies.Monitoring.Snmp.Win32.Types
{
    /// <summary>
    /// A class that creates instances of the Win32 SecurityAttributes class
    /// </summary>
    internal static class SecurityAttributesFactory
    {
        /// <summary>
        /// Create a managed SA struct with a given SD and inherit handle
        /// </summary>
        /// <param name="sd">The security descriptor to use</param>
        /// <param name="inheritHandle">True if should inherit parent handle</param>
        /// <returns>A set of security attributes</returns>
        public static SecurityAttributes Create(SecurityDescriptor sd, bool inheritHandle)
        {
            // create the SA struct and init the members
            var attributes = new SecurityAttributes
            {
                Length = Marshal.SizeOf(typeof(SecurityAttributes)),
                DoesInheritHandle = inheritHandle,
                SecurityDescriptor = sd
            };

            return attributes;
        }

        /// <summary>
        /// Create a managed SA using an empty SD and inherit the handle
        /// </summary>
        /// <param name="inheritHandle">True if should inherit parent handle</param>
        /// <returns>A set of security attributes</returns>
        public static SecurityAttributes Create(bool inheritHandle)
        {
            return Create(SecurityDescriptorFactory.Create(), inheritHandle);
        }

        /// <summary>
        /// Create a managed SA using an empty SD and inherit the handle
        /// </summary>
        /// <returns>A set of security attributes</returns>
        public static SecurityAttributes Create()
        {
            return Create(true);
        }
    }
}
