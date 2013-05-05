using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web.Configuration;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SharedAssemblies.Security.Encryption
{
    /// <summary>
    /// <para>
    /// This class is used for ASP.NET or Web Services that have a web.config file.
    /// Use this class to retrieve the MachineKeySection fields: DecryptionKey, ValidationKey, 
    /// and Validation type. Both keys must be Hex base strings
	/// (e.g. 5F086488C72145A27341EDDBE3D72730706859273512550A46D9A34E809F3760).
    /// </para>
    /// <para>
    /// Note: You can also use an app.config file as long as it contains the following:
    /// </para>
    /// <code>
    /// <system.web>
    ///   <machineKey validationKey="64 Character Hex String"
    ///        decryptionKey="64 Character Hex String"
    ///        validation="SHA1"/>
    /// </system.web>
    /// </code>
    /// </summary>
    public static class WebConfigMachineKey
    {
        /// <summary>
        /// The key section from web config
        /// </summary>
		private const string _keySection = "system.web/machineKey";

        /// <summary>
        /// Retrieve the Decryption Key from either a web.config file
		/// or from an app.config file.
        /// </summary>
        /// <returns>binary data or null if error.</returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5005:NoEmptyCatchBlocks",
			Justification = "Initialization of the key manager failures should not bleed information.")]
		public static byte[] GetMachineKeyDecryptionKey()
        {
            byte[] key = null;

			try
			{
				MachineKeySection machineKey = (MachineKeySection)
					ConfigurationManager.GetSection(_keySection);
				key = SoapHexBinary.Parse(machineKey.DecryptionKey).Value;
			}
			catch
			{
				// This is allowed to prevent secure informatio from escaping
			}

            return key;
        }

        /// <summary>
        /// Retrieve the Validation Key from either a web.config file
		/// or from an app.config file.
        /// </summary>
        /// <returns>binary data or null if error.</returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5005:NoEmptyCatchBlocks",
			Justification = "Initialization of the key manager failures should not bleed information.")]
		public static byte[] GetMachineKeyValidationKey()
        {
            byte[] key = null;

			try
			{
				MachineKeySection machineKey = (MachineKeySection)
					ConfigurationManager.GetSection(_keySection);
				key = SoapHexBinary.Parse(machineKey.ValidationKey).Value;
			}
			catch
			{
				// This is allowed to prevent secure informatio from escaping
			}

            return key;
        }
    }
}
