using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// <para>
	/// Implementation of <see cref="AbstractKeyManager"/> which allows a user
	/// retrieve bashwork encryption keys from various point in the registry
    /// </para>
    /// <para>
	/// To use simply specify a list of registry keys to look under for keys.
	/// It will then attempt to open every binary value of EncryptionKey and
	/// SigningKey it finds and adds it to the hash list.
    /// </para>
	/// </summary>
	public class RegistryKeyManager : AbstractKeyManager
	{
		/// <summary>
		/// Handle to the locations in the registry of the key components
		/// </summary>
		private readonly IEnumerable<string> _locations;

		/// <summary>
		/// Initialize a new instance of the RegistryKeyManager class
		/// </summary>
		/// <param name="locations">The locations in the registry of key pieces</param>
		public RegistryKeyManager(IEnumerable<string> locations)
		{
			_locations = locations;
		}

		/// <summary>
		/// Perform any initialization needed to retrieve the keys
		/// </summary>
		/// <returns>A collection of key components to create final key</returns>
		protected override IEnumerable<KeyContainer> InitializeKeys()
		{
			const string encryptionField = "EncryptionKey";
			const string signingField = "SigningKey";

			IEnumerable<KeyContainer> result = null;

			if (_locations.IsNotNullOrEmpty())
			{
				result = _locations.Select(location => new KeyContainer
				{
					EncryptionKey = (byte[])Registry.GetValue(location,
						encryptionField, new byte[] { }),

					SigningKey = (byte[])Registry.GetValue(location,
						signingField, new byte[] { }),
				});
			}

			return result;
		}
	}
}

