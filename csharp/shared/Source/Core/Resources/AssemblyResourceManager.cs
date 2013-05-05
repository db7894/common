using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Core.Resources
{
	/// <summary>
	/// Manages assembly embedded resources
	/// </summary>
	public class AssemblyResourceManager
	{
		private string[] _resourceNames;

		/// <summary>
		/// Gets or sets the assembly from which resources will be retrieved.
		/// </summary>
		public Assembly Assembly { get; set; }

		/// <summary>
		/// Gets or sets the base path which prefixes all requested resource names
		/// </summary>
		public string BasePath { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceManager"/> class.
		/// </summary>
		/// <param name="assembly">
		/// The assembly from which resources should be requested.
		/// If not specified, the calling assembly is used by default.
		/// </param>
		/// <param name="resourceBasePath">
		/// The base path to prepend to requested resource names.
		/// If not specified, an empty string is used by default.
		/// </param>
		public AssemblyResourceManager(Assembly assembly = null, string resourceBasePath = null)
		{
			Assembly = assembly ?? Assembly.GetCallingAssembly();
			BasePath = resourceBasePath ?? string.Empty;
		}

		/// <summary>
		/// Retrieves embedded resource data.
		/// </summary>
		/// <param name="resourceName">The name of the embedded resource to be appended to the BasePath property.</param>
		/// <returns>Byte array containing data from embedded resource.</returns>
		/// <exception cref="ArgumentException">ResourceName is an empty string.</exception>
		/// <exception cref="ArgumentNullException">Specified argument was null.</exception>
		/// <exception cref="AssemblyResourceNotFoundException">The resource name was not found in the specified assembly.</exception>
		/// <exception cref="AssemblyResourceUnreadableException">The specified assembly or resource was not readable.</exception>
		public byte[] GetData(string resourceName)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			if (resourceName == string.Empty)
			{
				throw new ArgumentException("ResourceName parameter cannot be an empty string.");
			}
			return Assembly.GetManifestResourceData(BasePath + resourceName);
		}

		/// <summary>
		/// Retrieves a stream from which an embedded resource can be read.
		/// </summary>
		/// <param name="resourceName">The name of the embedded resource to be appended to the BasePath property.</param>
		/// <returns>A stream from which an embedded resource can be read.</returns>
		/// <exception cref="ArgumentException">ResourceName is an empty string.</exception>
		/// <exception cref="ArgumentNullException">Specified argument was null.</exception>
		public Stream GetStream(string resourceName)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			if (resourceName == string.Empty)
			{
				throw new ArgumentException("ResourceName parameter cannot be an empty string.");
			}
			return Assembly.GetManifestResourceStream(BasePath + resourceName);
		}

		/// <summary>
		/// Retrieves text from embedded resource using the specified encoding.
		/// </summary>
		/// <param name="resourceName">The name of the embedded resource.</param>
		/// <param name="encoding">
		/// The text encoding used by the embedded resource.
		/// If not specified, UTF8 is used by default.
		/// </param>
		/// <returns>String containing text from embedded resource.</returns>
		/// <exception cref="ArgumentException">ResourceName is an empty string.</exception>
		/// <exception cref="ArgumentNullException">Specified argument was null.</exception>
		/// <exception cref="AssemblyResourceNotFoundException">The resource name was not found in the specified assembly.</exception>
		/// <exception cref="AssemblyResourceUnreadableException">The specified assembly or resource was not readable.</exception>
		public string GetText(string resourceName, Encoding encoding = null)
		{
			return Assembly.GetManifestResourceText(BasePath + resourceName, encoding);
		}

		/// <summary>
		/// Saves embedded resource to disk. Existing file is overwritten.
		/// </summary>
		/// <param name="resourceName">The name of the embedded resource.</param>
		/// <param name="fileName">The file name to which the embedded resource data is saved.</param>
		/// <exception cref="ArgumentException">File name or path is invalid.</exception>
		/// <exception cref="ArgumentNullException">Specified argument was null.</exception>
		/// <exception cref="PathTooLongException">The resource name was not found in the specified assembly.</exception>
		/// <exception cref="DirectoryNotFoundException">File name or path exceeded maximum length.</exception>
		/// <exception cref="IOException">The specified path is invalid.</exception>
		/// <exception cref="UnauthorizedAccessException">An I/O error occurred while saving the file.</exception>
		/// <exception cref="FileNotFoundException">Specified file is read-only or a directory, or caller does not have the required permission.</exception>
		/// <exception cref="NotSupportedException">The specified file could not be located.</exception>
		/// <exception cref="SecurityException">The specified path is an invalid format.</exception>
		/// <exception cref="AssemblyResourceNotFoundException">The caller does not have the required permission.</exception>
		public virtual void Save(string resourceName, string fileName)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			if (resourceName == string.Empty)
			{
				throw new ArgumentException("ResourceName parameter cannot be an empty string.");
			}

			Assembly.SaveManifestResourceData(BasePath + resourceName, fileName);
		}

		/// <summary>
		/// Retrieves names of embedded resources
		/// </summary>
		/// <returns>String array containing names of all embedded resources</returns>
		public string[] GetNames()
		{
			return _resourceNames ?? (_resourceNames = Assembly.GetManifestResourceNames());
		}
	}
}
