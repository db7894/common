using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using SharedAssemblies.Core.Resources;

namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Extension methods for dealing with an assembly
	/// </summary>
	public static class AssemblyExtensions
	{
		private const int _readBufferLength = 65536;

		/// <summary>
		/// Retrieves embedded resource data from the assembly.
		/// </summary>
		/// <param name="assembly">The assembly in which the resource is embedded.</param>
		/// <param name="resourceName">The full name of the embedded resource.</param>
		/// <returns>Byte array containing data from embedded resource.</returns>
		/// <exception cref="ArgumentException">ResourceName is an empty string.</exception>
		/// <exception cref="ArgumentNullException">Specified argument was null.</exception>
		/// <exception cref="AssemblyResourceNotFoundException">The resource name was not found in the specified assembly.</exception>
		/// <exception cref="AssemblyResourceUnreadableException">The specified assembly or resource was not readable.</exception>
		public static byte[] GetManifestResourceData(this Assembly assembly, string resourceName)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			if (resourceName == string.Empty)
			{
				throw new ArgumentException("ResourceName parameter cannot be an empty string.");
			}

			byte[] resourceBuffer;

			try
			{
				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				{
					if (stream == null)
					{
						throw new AssemblyResourceNotFoundException(assembly, resourceName);
					}
					resourceBuffer = ReadResourceStream(stream);
				}
			}
			catch (BadImageFormatException excep)
			{
				throw new AssemblyResourceUnreadableException(assembly, resourceName, excep);
			}
			catch (FileLoadException excep)
			{
				throw new AssemblyResourceUnreadableException(assembly, resourceName, excep);
			}
			catch (NotImplementedException excep)
			{
				throw new AssemblyResourceUnreadableException(assembly, resourceName, excep);
			}
			catch (IOException excep)
			{
				throw new AssemblyResourceUnreadableException(assembly, resourceName, excep);
			}
			return resourceBuffer;
		}

		/// <summary>
		/// Retrieves the text of a resource embedded in the calling assembly.
		/// </summary>
		/// <param name="assembly">The assembly in which the resource is embedded.</param>
		/// <param name="resourceName">The full name of the embedded resource.</param>
		/// <param name="encoding">
		/// The text encoding used by the embedded resource.
		/// If not specified, UTF8 is used by default.
		/// </param>
		/// <returns>The text of a resource embedded in the calling assembly.</returns>
		public static string GetManifestResourceText(this Assembly assembly, string resourceName, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			string text = encoding.GetString(assembly.GetManifestResourceData(resourceName));
			string bom = encoding.GetString(encoding.GetPreamble());
			if (text.StartsWith(bom))
			{
				text = text.Replace(bom, string.Empty);
			}
			return text;
		}

		/// <summary>
		/// Saves embedded resource from the specified assembly to disk. Existing file is overwritten.
		/// </summary>
		/// <param name="assembly">The assembly in which the resource is embedded.</param>
		/// <param name="resourceName">The full name of the embedded resource.</param>
		/// <param name="fileName">The file name to which the embedded resource data is saved.</param>
		/// <exception cref="ArgumentException">File name or path is invalid.</exception>
		/// <exception cref="ArgumentNullException">Specified argument was null.</exception>
		/// <exception cref="AssemblyResourceNotFoundException">The resource name was not found in the specified assembly.</exception>
		/// <exception cref="PathTooLongException">File name or path exceeded maximum length.</exception>
		/// <exception cref="DirectoryNotFoundException">The specified path is invalid.</exception>
		/// <exception cref="IOException">An I/O error occurred while saving the file.</exception>
		/// <exception cref="UnauthorizedAccessException">Specified file is read-only or a directory, or caller does not have the required permission.</exception>
		/// <exception cref="FileNotFoundException">The specified file could not be located.</exception>
		/// <exception cref="NotSupportedException">The specified path is an invalid format.</exception>
		/// <exception cref="SecurityException">The caller does not have the required permission.</exception>
		public static void SaveManifestResourceData(this Assembly assembly, string resourceName, string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			File.WriteAllBytes(fileName, assembly.GetManifestResourceData(resourceName));
		}

		/// <summary>
		/// Loads the configuration from assembly attributes
		/// </summary>
		/// <typeparam name="T">The type of the custom attribute to find.</typeparam>
		/// <param name="callingAssembly">The calling assembly to search.</param>
		/// <returns>The custom attribute of type T, if found.</returns>
		public static T GetAttribute<T>(this Assembly callingAssembly) 
			where T : Attribute
		{
			T result = null;

			// Try to find the configuration attribute for the default logger if it exists
			object[] configAttributes = Attribute.GetCustomAttributes(callingAssembly,
				typeof(T), false);

			// get just the first one
			if (!configAttributes.IsNullOrEmpty())
			{
				result = (T)configAttributes[0];
			}

			return result;
		}
		
		/// <summary>
		/// Loads the configuration from assembly attributes
		/// </summary>
		/// <typeparam name="T">The type of the custom attribute to find.</typeparam>
		/// <param name="callingAssembly">The calling assembly to search.</param>
		/// <returns>An enumeration of attributes of type T that were found.</returns>
		public static IEnumerable<T> GetAttributes<T>(this Assembly callingAssembly)
			where T : Attribute
		{
			// Try to find the configuration attribute for the default logger if it exists
			object[] configAttributes = Attribute.GetCustomAttributes(callingAssembly,
				typeof(T), false);

			// get just the first one
			if (!configAttributes.IsNullOrEmpty())
			{
				foreach (T attribute in configAttributes)
				{
					yield return attribute;
				}
			}
		}

		/// <summary>
		/// Gets all of the attributes for the current assembly.
		/// </summary>
		/// <param name="callingAssembly">The assembly to get the attributes from.</param>
		/// <returns>An enumeration of all attributes in the assembly.</returns>
		public static IEnumerable<Attribute> GetAttributes(this Assembly callingAssembly)
		{
			return Attribute.GetCustomAttributes(callingAssembly, false);
		}

		/// <summary>
		/// This helper method loads the attributes reflectively to account for them possibly being used with a
		/// different version of shared assemblies.  The type provided must be an exact match and not a super-class!
		/// </summary>
		/// <typeparam name="T">The type of attribute to load, must be exact and not a super-class.</typeparam>
		/// <param name="target">The target assembly to find attributes in.</param>
		/// <returns>An enumeration of the attributes.</returns>
		public static IEnumerable<T> GetAttributesReflectively<T>(this Assembly target) where T : Attribute, new()
		{
			var searchType = typeof(T);

			foreach (var attribute in target.GetAttributes())
			{
				var foundType = attribute.GetType();

				// if the attribute is the appropriate type already, just cast it and return it.
				if (foundType == searchType)
				{
					yield return (T)attribute;
				}

				// otherwise, if the attribute matches by name, but is not the right type, we can
				// try to reflectively convert it.
				else if (foundType.FullName == searchType.FullName)
				{
					var newAttribute = new T();
					foreach (var prop in foundType.GetProperties())
					{
						try
						{
							// make sure not an indexer
							if (prop.GetIndexParameters().Length == 0 && !prop.IsSpecialName
								&& prop.CanRead)
							{
								// find the property in the new attribute
								var newProperty = searchType.GetProperty(prop.Name);

								// if it exists and is also not an indexer, assign the value.
								if (newProperty != null && newProperty.GetIndexParameters().Length == 0
									&& prop.CanWrite)
								{
									newProperty.SetValue(newAttribute, prop.GetValue(attribute, null), null);
								}
							}
						}

						catch (Exception)
						{
							// if there are any exceptions, do not crash, just ignore
						}
					}

					// return the attribute
					yield return newAttribute;
				}
			}
		}

		/// <summary>
		/// Gets an array of bytes from the resource stream.
		/// </summary>
		/// <param name="stream">The resource stream from the assembly.</param>
		/// <returns>Array of bytes of resource data.</returns>
		private static byte[] ReadResourceStream(Stream stream)
		{
			int writePosition = 0;
			byte[] resourceBuffer = new byte[stream.Length];
			byte[] readBuffer = new byte[_readBufferLength];

			int readCount = stream.Read(readBuffer, 0, readBuffer.Length);
			while (readCount > 0)
			{
				Array.Copy(readBuffer, 0, resourceBuffer, writePosition, readCount);
				writePosition += readCount;
				readCount = stream.Read(readBuffer, 0, readBuffer.Length);
			}
			return resourceBuffer;
		}
	}
}