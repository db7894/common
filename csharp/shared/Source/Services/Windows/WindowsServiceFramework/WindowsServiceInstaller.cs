using System;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Linq;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Core.Installers;

namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// A generic windows service installer
	/// </summary>
	public partial class WindowsServiceInstaller : Installer
	{
		// the list of other installers to run
		private readonly Installer[] _otherInstallers;

		/// <summary>
		/// Gets or sets the type of the windows service to install.
		/// </summary>
		private readonly WindowsServiceAttribute _configuration;


		/// <summary>
		/// Creates a blank windows service installer with no configuration.
		/// </summary>
		public WindowsServiceInstaller()
		{
		}


		/// <summary>
		/// Creates a windows service installer using the type specified.
		/// </summary>
		/// <param name="windowsServiceType">The type of the windows service to install.</param>
		/// <param name="otherInstallers">Other installers to configure as well.</param>
		public WindowsServiceInstaller(Type windowsServiceType, params Installer[] otherInstallers)
		{
			if (!windowsServiceType.GetInterfaces().Contains(typeof(IWindowsService)))
			{
				HorizontalRule();
				WriteToConsole(ConsoleColor.Red, "{0} must implement IWindowsService.",
				               windowsServiceType.FullName);
				HorizontalRule();

				throw new ArgumentException("Type to install must implement IWindowsService.",
				                            "windowsServiceType");
			}

			var attribute = windowsServiceType.GetAttribute<WindowsServiceAttribute>();

			if (attribute == null)
			{
				HorizontalRule();
				WriteToConsole(ConsoleColor.Red, "{0} must be marked with WindowsServiceAttribute.",
							   windowsServiceType.FullName);
				HorizontalRule();

				throw new ArgumentException("Type to install must be marked with a WindowsServiceAttribute.",
				                            "windowsServiceType");
			}

			_otherInstallers = otherInstallers;
			_configuration = attribute;
		}


		/// <summary>
		/// Performs a transacted installation at run-time of the AutoCounterInstaller and any other listed installers.
		/// </summary>
		/// <param name="otherInstallers">The other installers to include in the transaction</param>
		/// <typeparam name="T">The IWindowsService implementer to install.</typeparam>
		public static void RuntimeInstall<T>(params Installer[] otherInstallers)
			where T : IWindowsService
		{
			var installer = new WindowsServiceInstaller(typeof(T));
			RuntimeInstaller.Install(installer, otherInstallers);
		}


		/// <summary>
		/// Performs a transacted un-installation at run-time of the AutoCounterInstaller and any other listed installers.
		/// </summary>
		/// <param name="otherInstallers">The other installers to include in the transaction</param>
		/// <typeparam name="T">The IWindowsService implementer to install.</typeparam>
		public static void RuntimeUnInstall<T>(params Installer[] otherInstallers)
			where T : IWindowsService
		{
			var installer = new WindowsServiceInstaller(typeof(T));
			RuntimeInstaller.UnInstall(installer, otherInstallers);
		}


		/// <summary>
		/// Installer class, to use run InstallUtil against this .exe
		/// </summary>
		/// <param name="savedState">The saved state for the installation.</param>
		public override void Install(System.Collections.IDictionary savedState)
		{
			try
			{
				HorizontalRule();
				WriteToConsole(ConsoleColor.White, "Installing service {0}.", _configuration.Name);

				// install the service 
				ConfigureInstallers();
				base.Install(savedState);

				HorizontalRule();
			}
			catch (Exception ex)
			{
				WriteToConsole(ConsoleColor.Red, "Exception: {0}", ex);
			}
		}


		/// <summary>
		/// Removes the counters, then calls the base uninstall.
		/// </summary>
		/// <param name="savedState">The saved state for the installation.</param>
		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			try
			{
				HorizontalRule();
				WriteToConsole(ConsoleColor.White, "Un-Installing service {0}.", _configuration.Name);

				// load the assembly file name and the config
				ConfigureInstallers();
				base.Uninstall(savedState);

				HorizontalRule();
			}
			catch (Exception ex)
			{
				WriteToConsole(ConsoleColor.Red, "Exception: {0}", ex);
			}
		}


		/// <summary>
		/// Rolls back to the state of the counter, and performs the normal rollback.
		/// </summary>
		/// <param name="savedState">The saved state for the installation.</param>
		public override void Rollback(System.Collections.IDictionary savedState)
		{
			try
			{
				HorizontalRule();

				WriteToConsole(ConsoleColor.White, "Rolling back service {0}.", _configuration.Name);

				// load the assembly file name and the config
				ConfigureInstallers();
				base.Rollback(savedState);

				HorizontalRule();
			}
			catch (Exception ex)
			{
				WriteToConsole(ConsoleColor.Red, "Exception: {0}", ex);
			}
		}


		/// <summary>
		/// Method to configure the installers
		/// </summary>
		private void ConfigureInstallers()
		{
			// load the assembly file name and the config
			try
			{
				Installers.Add(ConfigureProcessInstaller());
				Installers.Add(ConfigureServiceInstaller());

				// add additional installers if specified
				if (_otherInstallers != null && _otherInstallers.Length > 0)
				{
					Installers.AddRange(_otherInstallers);
				}
			}
			catch(Exception ex)
			{
				WriteToConsole(ConsoleColor.Red, "Error in configuration of service {0}.",
					_configuration.Name);
				WriteToConsole(ConsoleColor.Red, "{0}", ex);
				throw;
			}
		}


		/// <summary>
		/// Helper method to configure a process installer for this windows service
		/// </summary>
		/// <returns>Process installer for this service</returns>
		private ServiceProcessInstaller ConfigureProcessInstaller()
		{
			var result = new ServiceProcessInstaller();

			// if a user name is not provided, will run under local service acct
			if (string.IsNullOrEmpty(_configuration.UserName))
			{
				result.Account = ServiceAccount.LocalService;
				result.Username = null;
				result.Password = null;
			}
			else
			{
				// otherwise, runs under the specified user authority
				result.Account = ServiceAccount.User;
				result.Username = _configuration.UserName;
				result.Password = _configuration.Password;
			}

			return result;
		}


		/// <summary>
		/// Helper method to configure a service installer for this windows service
		/// </summary>
		/// <returns>Process installer for this service</returns>
		private ServiceInstaller ConfigureServiceInstaller()
		{
			// create and config a service installer
			var result = new ServiceInstaller
			                          	{
											ServiceName = _configuration.Name,
											DisplayName = _configuration.DisplayName,
											Description = _configuration.Description,
											StartType = _configuration.StartMode,
			                          	};

			return result;
		}


		/// <summary>
		/// Just display a horizontal line
		/// </summary>
		private static void HorizontalRule()
		{
			WriteToConsole(ConsoleColor.Yellow, "--------------------------------------------------"
				+ "-----------------------------");
		}


		/// <summary>
		/// Displays some text to the console in a different color
		/// </summary>
		/// <param name="foregroundColor">The color to use for the foreground</param>
		/// <param name="formatString">The format string to use for the output</param>
		/// <param name="arguments">The array of objects to output</param>
		private static void WriteToConsole(ConsoleColor foregroundColor, string formatString,
			params object[] arguments)
		{
			// save old color, change it, output, and then restore old color
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine(formatString, arguments);
			Console.ForegroundColor = originalColor;
		}
	}
}