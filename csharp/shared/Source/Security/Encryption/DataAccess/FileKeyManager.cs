using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// <para>
	/// Implementation of <see cref="AbstractKeyManager"/> which allows a user
	/// retrieve bashwork encryption keys from one or more file locations.
    /// </para>
    /// <para>
	/// To use simply specify a list of files to examine for keys. The format
	/// of the file should be an xml serialized KeyContainer.
    /// </para>
	/// </summary>
	public class FileKeyManager : AbstractKeyManager
	{
		/// <summary>
		/// Handle to the locations in the registry of the key components
		/// </summary>
		private readonly IEnumerable<string> _locations;

		/// <summary>
		/// Initialize a new instance of the FileKeyManager class
		/// </summary>
		/// <param name="locations">The locations in the filesystem of key pieces</param>
		public FileKeyManager(IEnumerable<string> locations)
		{
			_locations = locations;
		}

		/// <summary>
		/// Perform any initialization needed to retrieve the keys
		/// </summary>
		/// <returns>A collection of key components to create final key</returns>
		protected override IEnumerable<KeyContainer> InitializeKeys()
		{
			IEnumerable<KeyContainer> result = null;

			if (_locations.IsNotNullOrEmpty())
			{
				result = _locations.Select(location =>
					File.ReadAllText(location).ToType<KeyContainer>());
			}

			return result;
		}
	}
}

