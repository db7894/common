using System.Configuration.Install;
using SharedAssemblies.Core.Installers;


namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Static class with extension methods for invoking installers directly.
	/// </summary>
	public static class InstallerExtensions
	{
		/// <summary>
		/// Extension method on Installer that makes it easier to invoke an installer in code instead of through
		/// InstallUtil.exe.  
		/// </summary>
		/// <param name="mainInstaller">The main installer you are invoking.</param>
		/// <param name="others">Any supplemental installers you wish to invoke.</param>
		public static void RuntimeInstall(this Installer mainInstaller, params Installer[] others)
		{
			RuntimeInstaller.Install(mainInstaller, others);
		}


		/// <summary>
		/// Extension method on unInstaller that makes it easier to invoke an uninstaller in code instead of through
		/// InstallUtil.exe.  
		/// </summary>
		/// <param name="mainInstaller">The main installer you are invoking.</param>
		/// <param name="others">Any supplemental installers you wish to invoke.</param>
		public static void RuntimeUnInstall(this Installer mainInstaller, params Installer[] others)
		{
			RuntimeInstaller.UnInstall(mainInstaller, others);
		}
	}
}
