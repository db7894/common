using System;


namespace SharedAssemblies.Services.Windows.WindowsServiceFramework
{
	/// <summary>
	/// This class handles the main routine if running as console app.
	/// </summary>
	internal static class ConsoleMain
	{
		/// <summary>
		/// Run a service from the console given a config filename.
		/// </summary>
		/// <param name="commandLineArguments">The command line arguments.</param>
		/// <param name="service">The service to wrap and run.</param>
		internal static void Run(string[] commandLineArguments, IWindowsService service)
		{
			string serviceName = service.GetType().Name;
			bool isRunning = true;

			// simulate starting the windows service
			service.OnStart(commandLineArguments);

			// let it run as long as Q is not pressed
			while (isRunning)
			{
				WriteToConsole(ConsoleColor.Yellow, 
					"Enter either [Q]uit, [P]ause, [R]esume, [C]ommand : ");
				isRunning = HandleConsoleInput(service, Console.ReadLine());
			}

			// simulate stopping the windows service
			service.OnStop();
		}


		/// <summary>
		/// Private input handler for console commands.
		/// </summary>
		/// <param name="service">The windows service to wrap.</param>
		/// <param name="line">Input line from keyboard.</param>
		/// <returns>Returns true if should continue.</returns>
		private static bool HandleConsoleInput(IWindowsService service, string line)
		{
			bool canContinue = true;

			// check input
			if (line != null)
			{
				switch (line.ToUpper())
				{
					case "Q":
						// quit command
						canContinue = false;
						break;

					case "P":
						// pause service command
						service.OnPause();
						break;

					case "R":
						// resume service command
						service.OnContinue();
						break;

					case "C":
						// custom command id
						WriteToConsole(ConsoleColor.Yellow, "Enter command id:");
						int command;
						if (int.TryParse(Console.ReadLine(), out command))
						{
							service.OnCustomCommand(command);
						}
						break;

					default:
						WriteToConsole(ConsoleColor.Red, "Did not understand that input, try again.");
						break;
				}
			}

			return canContinue;
		}


		/// <summary>
		/// Helper method to write a message to the console at the given foreground color.
		/// </summary>
		/// <param name="foregroundColor">Color to write text foreground.</param>
		/// <param name="format">Format string for text.</param>
		/// <param name="formatArguments">Arguments for format string.</param>
		private static void WriteToConsole(ConsoleColor foregroundColor, string format, 
			params object[] formatArguments)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = foregroundColor;

			Console.WriteLine(format, formatArguments);
			Console.Out.Flush();

			Console.ForegroundColor = originalColor;
		}
	}
}