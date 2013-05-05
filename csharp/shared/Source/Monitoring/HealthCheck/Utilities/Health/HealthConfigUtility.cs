using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A helper utility to retrieve configuation information for
	/// an application from a number of locations.
	/// </summary>
	public static class HealthConfigUtility
	{
		/// <summary>
		/// A collection of the configuration settings to parse from the
		/// supplied app or web config.
		/// </summary>
		private static readonly Dictionary<string, Func<Configuration, IDictionary<string, string>>>
			_sections = new Dictionary<string, Func<Configuration, IDictionary<string, string>>>
			{
				{ "ApplicationSettings", GetApplicationSettings },
				{ "ConfigurationStrings", GetConfigurationStrings },
			};

		/// <summary>
		/// Helper method that returns all the configuration values from the
		/// current or specified app or web config.
		/// </summary>
		/// <param name="config">The file to open, if null use current in app domain</param>
		/// <returns>A collection of the current configurations</returns>
		public static List<ApplicationConfiguration> GetFromAppConfig(string config)
		{
			var current = (string.IsNullOrEmpty(config))
				? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming)
				: ConfigurationManager.OpenExeConfiguration(config);

			return GetConfigurationFromConfig(current);
		}

		/// <summary>
		/// Helper method that returns all the configuration values from the
		/// specified registry key (two levels).
		/// </summary>
		/// <param name="root">The root registry key to open</param>
		/// <returns>A collection of the current configurations</returns>
		public static List<ApplicationConfiguration> GetFromRegistry(string root)
		{
			var handle = Registry.LocalMachine.OpenSubKey(root);
			if (handle == null)
			{
				throw new ApplicationException("The registry path does not exist: " + root);
			}
			
			return handle.GetSubKeyNames().NullSafe().Select(key => new ApplicationConfiguration
			{
				SectionName = key,
				Configuration = GetRegistryKeys(handle, key),
			}).ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		private static IDictionary<string, string> GetRegistryKeys(RegistryKey root, string section)
		{
			IDictionary<string, string> result = null;

			var handle = root.OpenSubKey(section);
			if (handle != null)
			{
				result = handle.GetValueNames().NullSafe().ToDictionary(key => key,
					key => handle.GetValue(key, string.Empty).ToString());
			}

			return result ?? new Dictionary<string, string>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config">The configuration handle to pull settings from</param>
		/// <returns>A collection of application configurations for the config sections</returns>
		private static List<ApplicationConfiguration> GetConfigurationFromConfig(Configuration config)
		{
			return _sections.NullSafe().Select(configuration => new ApplicationConfiguration
			{
				SectionName = configuration.Key,
				Configuration = configuration.Value(config)
			}).ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config">The configuration handle to pull settings from</param>
		/// <returns></returns>
		private static IDictionary<string, string> GetApplicationSettings(Configuration config)
		{
			var dictionary = new Dictionary<string, string>();

			foreach (KeyValueConfigurationElement setting in config.AppSettings.Settings)
			{
				dictionary[setting.Key] = setting.Value;
			}

			return dictionary;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config">The configuration handle to pull settings from</param>
		/// <returns></returns>
		private static IDictionary<string, string> GetConfigurationStrings(Configuration config)
		{
			var dictionary = new Dictionary<string, string>();

			foreach (ConnectionStringSettings setting in config.ConnectionStrings.ConnectionStrings)
			{
				dictionary[setting.Name] = setting.ConnectionString;
			}

			return dictionary;
		}
	}
}
