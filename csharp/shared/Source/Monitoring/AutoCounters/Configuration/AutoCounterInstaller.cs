using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using SharedAssemblies.Core.Installers;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Monitoring.AutoCounters.Configuration
{
	/// <summary>
	/// Class used to install performance counters on current machine, needs admin authority.
	/// </summary>
	[RunInstaller(true)]
	public class AutoCounterInstaller : Installer
	{
		/// <summary>
		/// The switch to specify target to scan
		/// </summary>
		public const string TargetSwitch = "target";


		/// <summary>
		/// Help text for the installer
		/// </summary>
		public override string HelpText
		{
			get
			{
				return string.Format("use /{0}= to specify a file or directory to check for AutoCounters, "
					+ "otherwise the installer will attempt to scan current directory for assemblies.",
					TargetSwitch);
			}
		}


		/// <summary>
		/// Performs a transacted installation at run-time of the AutoCounterInstaller and any other listed installers.
		/// </summary>
		/// <param name="otherInstallers">The other installers to include in the transaction</param>
		public static void RuntimeInstall(params Installer[] otherInstallers)
		{
			var installer = new AutoCounterInstaller();
			RuntimeInstaller.Install(installer, otherInstallers);
		}


		/// <summary>
		/// Performs a transacted un-installation at run-time of the AutoCounterInstaller and any other listed installers.
		/// </summary>
		/// <param name="otherInstallers">The other installers to include in the transaction</param>
		public static void RuntimeUnInstall(params Installer[] otherInstallers)
		{
			var installer = new AutoCounterInstaller();
			RuntimeInstaller.UnInstall(installer, otherInstallers);
		}


		/// <summary>
		/// When installing, get the counters to install from our context and install them using
		/// the base installer (which invokes install on all Installers.
		/// </summary>
		/// <param name="stateSaver">The state saved by installer</param>
		public override void Install(System.Collections.IDictionary stateSaver)
		{
			try
			{
				HorizontalRule();

				GetCountersToInstall(GetTarget(Context)).ForEach(installer => Installers.Add(installer));

				base.Install(stateSaver);

				HorizontalRule();
			}
			catch (Exception ex)
			{
				WriteToConsole(ConsoleColor.Red, "Exception: {0}", ex);
			}
		}


		/// <summary>
		/// When installing, get the counters to install from our context and install them using
		/// the base installer (which invokes install on all Installers.
		/// </summary>
		/// <param name="savedState">The saved state for installers</param>
		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			try
			{
				HorizontalRule();

				GetCountersToInstall(GetTarget(Context)).ForEach(installer => Installers.Add(installer));

				base.Uninstall(savedState);

				HorizontalRule();
			}
			catch (Exception ex)
			{
				WriteToConsole(ConsoleColor.Red, "Exception: {0}", ex);
			}
		}


		/// <summary>
		/// Gets the target command line switch from this install context
		/// </summary>
		/// <param name="context">The context for the installer</param>
		/// <returns>String for the target location</returns>
		private static string GetTarget(InstallContext context)
		{
			string path = null;

			if (context != null && context.Parameters != null)
			{
				path = context.Parameters[TargetSwitch];
			}

			return Path.GetFullPath(path ?? Directory.GetCurrentDirectory());
		}


		/// <summary>
		/// Gets the list of installers to run
		/// </summary>
		/// <param name="targetLocation">Target location for counters to find</param>
		/// <returns>Enumeration of counter installers</returns>
		private static IEnumerable<PerformanceCounterInstaller> GetCountersToInstall(string targetLocation)
		{
			var results = new AutoCounterRegistry();

			// if the target is a file, treat as an assembly and load
			if (File.Exists(targetLocation))
			{
				WriteToConsole(ConsoleColor.White, "Searching for AutoCounters in Assembly {0}.",
					targetLocation);

				GetCountersFromAssembly(targetLocation, results);
			}

			// if the target is a directory, try every dll and exe in that directory
			else if (Directory.Exists(targetLocation))
			{
				WriteToConsole(ConsoleColor.White, "Searching for AutoCounters in directory {0}",
					targetLocation);

				GetCountersFromDirectory(targetLocation, results);
			}

			// otherwise, the location must not exist
			else
			{
				WriteToConsole(ConsoleColor.Red, "Could not find file or directory {0}.", targetLocation);
			}

			// the keys in the dictionary are category names, and the values are the installers,
			// we want to convert the installers from our specialized auto-counters to .NET Perf Counters.
			return PerformanceCounterInstallerAdapter.GetInstallers(results.Categories.Values);
		}


		/// <summary>
		/// loads installers from an assembly
		/// </summary>
		/// <param name="location">Location to load assembly from</param>
		/// <param name="registry">Registry for counter information</param>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "If any auto-counter fails, don't want to halt others.")]
		private static void GetCountersFromAssembly(string location, AutoCounterRegistry registry)
		{
			string displayName = Path.GetFileName(location);

			try
			{
				Assembly counterAssembly = Assembly.LoadFrom(location);

				// if we can load it as an assembly, find the counters in it
				if (counterAssembly != null)
				{
					var localRegistry = AutoCounterAssemblyLoader.LoadCounterRegistry(counterAssembly, 
																		CreateFailedAction.CreateStub, null);

					WriteToConsole(ConsoleColor.Green,
						"- Assembly {0} has {1} counter categories to install.",
						displayName, localRegistry.Categories.Count);

					// copy from the local into our big list of categories
					localRegistry.Categories.ForEach(pair => registry.Categories.Add(pair.Key, pair.Value));
				}

				// otherwise, must have not been an assembly or not found
				else
				{
					WriteToConsole(ConsoleColor.Red, "- {0} could not be loaded.", displayName);
				}
			}

			catch (BadImageFormatException)
			{
				WriteToConsole(ConsoleColor.Red, "- {0} not an assembly; skipping.", displayName);
			}

			catch (Exception ex)
			{
				WriteToConsole(ConsoleColor.Red, "- {0} had exception during load: {1}",
					displayName, ex.Message);
			}
		}


		/// <summary>
		/// If we were given a directory, then try all DLLs and EXEs as assemblies
		/// </summary>
		/// <param name="directory">The directory to get counters from</param>
		/// <param name="registry">The registry to store counter registrations in</param>
		private static void GetCountersFromDirectory(string directory, AutoCounterRegistry registry)
		{
			Directory.GetFiles(directory, "*.dll")
				.ForEach(assembly => GetCountersFromAssembly(assembly, registry));

			Directory.GetFiles(directory, "*.exe")
				.ForEach(assembly => GetCountersFromAssembly(assembly, registry));
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