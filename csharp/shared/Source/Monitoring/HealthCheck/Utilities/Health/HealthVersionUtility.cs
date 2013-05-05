using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A helper utility to retrieve version information for dlls
	/// from a number of locations.
	/// </summary>
	public static class HealthVersionUtility
	{
		/// <summary>
		/// Retrieve the version information for every dll in the specified directory
		/// </summary>
		/// <param name="directory">The directory to search for dlls</param>
		/// <returns>The version information for the dlls in the specified directory</returns>
		public static IDictionary<string, string> GetFromDirectory(string directory)
		{
			if (string.IsNullOrEmpty(directory))
			{
				throw new ArgumentNullException("directory");
			}
			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException(directory);
			}

			return Directory.GetFiles(directory, @"*.dll").NullSafe().ToDictionary(Path.GetFileName,
				file => FileVersionInfo.GetVersionInfo(file).ToString());
		}

		/// <summary>
		/// Retrieve the version information for every dll in the current directory
		/// </summary>
		/// <returns>The version information for the dlls in the current directory</returns>
		public static IDictionary<string, string> GetFromCurrentDirectory()
		{
			return GetFromDirectory(AppDomain.CurrentDomain.BaseDirectory);
		}

		/// <summary>
		/// Retrieve the version information for every dll in the current directory
		/// </summary>
		/// <param name="domain">The domain to pull the running assemblies from</param>
		/// <returns>The version information for the dlls in the current directory</returns>
		public static IDictionary<string, string> GetFromAppDomain(AppDomain domain)
		{
			var result = new Dictionary<string, string>();

			foreach (var assembly in domain.GetAssemblies().NullSafe())
			{
				var handle = assembly.GetName();
				result[handle.Name] = handle.FullName;
			}

			return result;
		}

		/// <summary>
		/// Retrieve the version information for every dll in the current directory
		/// </summary>
		/// <returns>The version information for the dlls in the current directory</returns>
		public static IDictionary<string, string> GetFromCurrentAppDomain()
		{
			return GetFromAppDomain(AppDomain.CurrentDomain);
		}
	}
}
