using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;


namespace SharedAssemblies.Core.Installers
{
	/// <summary>
	/// Static class that can invoke and perform a transacted install on one or more installers.
	/// </summary>
	public static class RuntimeInstaller
	{
		/// <summary>
		/// Attempts to invoke the windows installer programmatically, thus eliminating need for InstalUtil.exe
		/// </summary>
		/// <param name="mainInstaller">The main installer.</param>
		/// <param name="additionalInstallers">Additional supplemental installers.</param>
		public static void Install(Installer mainInstaller, params Installer[] additionalInstallers)
		{
			if (mainInstaller == null)
			{
				throw new ArgumentNullException("mainInstaller", "Main installer cannot be null.");
			}

			var installers = new List<Installer> { mainInstaller };

			if (additionalInstallers != null)
			{
				installers.AddRange(additionalInstallers);
			}

			Install(installers);
		}


		/// <summary>
		/// Attempts to invoke the windows installer programmatically, thus eliminating need for InstalUtil.exe
		/// </summary>
		/// <param name="installers">a list of the installers to install.</param>
		public static void Install(IEnumerable<Installer> installers)
		{
			if (installers == null)
			{
				throw new ArgumentNullException("installers", "Installers cannot be null.");
			}

			string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

			using (var ti = new TransactedInstaller())
			{
				ti.Installers.AddRange(installers.ToArray());
				ti.Context = new InstallContext(null, new string[] { path });
				ti.Install(new Hashtable());
			}
		}


		/// <summary>
		/// Attempts to invoke the windows uninstaller programmatically, thus eliminating need for InstalUtil.exe
		/// </summary>
		/// <param name="mainInstaller">The main installer.</param>
		/// <param name="additionalInstallers">Additional supplemental installers.</param>
		public static void UnInstall(Installer mainInstaller, params Installer[] additionalInstallers)
		{
			if (mainInstaller == null)
			{
				throw new ArgumentNullException("mainInstaller", "Main installer cannot be null.");
			}

			var installers = new List<Installer> { mainInstaller };

			if (additionalInstallers != null)
			{
				installers.AddRange(additionalInstallers);
			}

			UnInstall(installers);
		}


		/// <summary>
		/// Attempts to invoke the windows uninstaller programmatically, thus eliminating need for InstalUtil.exe
		/// </summary>
		/// <param name="installers">The installers to invoke to uninstall.</param>
		public static void UnInstall(IEnumerable<Installer> installers)
		{
			if (installers == null)
			{
				throw new ArgumentNullException("installers", "Installers cannot be null.");
			}

			string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

			using (var ti = new TransactedInstaller())
			{
				ti.Installers.AddRange(installers.ToArray());
				ti.Context = new InstallContext(null, new string[] { path });
				ti.Uninstall(null);
			}
		}
	}
}
